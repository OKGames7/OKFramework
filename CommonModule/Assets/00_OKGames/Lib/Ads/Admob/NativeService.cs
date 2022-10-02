using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace OKGamesLib {

    public class NativeService {
        private static TaskCompletionSource<bool> AttTcs;
        private static SynchronizationContext Context;

        static NativeService() {
            Context = SynchronizationContext.Current;
        }

#if UNITY_IOS

        /// <summary>
        /// ATT 許可状態取得
        /// </summary>
        /// <returns></returns>
        [DllImport("__Internal")]
        private static extern int GetTrackingAuthorizationStatus();

        private delegate void OnCompleteStatusCallback(int status);

        /// <summary>
        /// ATT 許可要求
        /// </summary>
        /// <returns></returns>
        [DllImport("__Internal")]
        private static extern void RequestTrackingAuthorization(OnCompleteStatusCallback callback);

        /// <summary>
        /// ATT の同意状態を取得する
        /// </summary>
        /// <returns></returns>
        public static bool? GetIOSTrackingAuthorizationStatus() {
#if UNITY_EDITOR
            return true;
#else
            switch (GetTrackingAuthorizationStatus()) {
                case -1: // No Needs
                case 3: // Authorized
                    return true;

                case 0: // Not Determined
                    return null;

                default:
                    return false;
            }
#endif
        }

        /// <summary>
        /// ATTの使用許可をユーザーへ要求する.
        /// </summary>
        /// <returns></returns>
        public static Task<bool> RequestTrackingAuthorization() {
#if UNITY_EDITOR
            return Task.FromResult(true);
#else
            AttTcs = new TaskCompletionSource<bool>();

            RequestTrackingAuthorization(OnRequestTrackingAuthorizationComplete);
            return AttTcs.Task;
#endif
        }

        [AOT.MonoPInvokeCallback(typeof(OnCompleteStatusCallback))]
        private static void OnRequestTrackingAuthorizationComplete(int status) {
            if (AttTcs != null) {
                Context.Post(_ => {
                    switch (status) {
                        case -1: // No Needs
                        case 3: // Authorized
                            AttTcs.TrySetResult(true);
                            break;

                        default:
                            AttTcs.TrySetResult(false);
                            break;
                    }
                }, null);
            }
        }

#endif
    }
}
