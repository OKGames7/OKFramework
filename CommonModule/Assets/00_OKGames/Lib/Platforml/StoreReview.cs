using UnityEngine;

#if UNITY_IOS || UNITY_ANDROID
using Cysharp.Threading.Tasks;
using UniRx;
#endif


namespace OKGamesLib {

    /// <summary>
    /// ストアでのレビューを促す機能.
    /// </summary>
    public class StoreReview {

        public static void RequestReview() {
#if UNITY_IOS
            // iOS.
            ReqeusReviewIOS();
#elif UNITY_ANDROID
            // Android.
            // ReqeusReviewAndoirdAsync().Forget();
#else
            Log.Warning("ストアのレビュー促進機能は現在のプレイ環境では対応していません");
#endif
        }


#if UNITY_IOS
        /// <summary>
        /// ストアでのレビューを促すダイアログを表示する(iOS用)
        /// </summary>
        private static void ReqeusReviewIOS() {
            UnityEngine.iOS.Device.RequestStoreReview();
        }
#endif

#if UNITY_ANDROID
        /// <summary>
        /// ストアでのレビューを促すダイアログを表示する(Android用)
        /// </summary>
        private static async UniTask ReqeusReviewAndoirdAsync() {
            var reviewManager = new Google.Play.Review.ReviewManager();
            var requestFlowOperation = reviewManager.RequestReviewFlow();
            await requestFlowOperation;

            if (requestFlowOperation.Error != Google.Play.Review.ReviewErrorCode.NoError) {
                Log.Error("ストアでのレビュー促進機能失敗: 【requesrFlowOperation】:  " + requestFlowOperation.Error.ToString());
                return;
            }

            var playReviewInfo = requestFlowOperation.GetResult();
            var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
            await launchFlowOperation;

            playReviewInfo = null;
            if (launchFlowOperation.Error != Google.Play.Review.ReviewErrorCode.NoError) {
                Log.Error("ストアでのレビュー促進機能失敗: 【launchFlowOperation】: " + launchFlowOperation.Error.ToString());
                return;
            }
        }
#endif
    }
}
