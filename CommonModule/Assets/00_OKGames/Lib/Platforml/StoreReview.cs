using UnityEngine;

#if (UNITY_IOS && !UNITY_EDITOR) || (UNITY_ANDROID && !UNITY_EDITOR)
using Cysharp.Threading.Tasks;
using UniRx;
#endif


namespace OKGamesLib {

    /// <summary>
    /// ストアでのレビューを促す機能.
    /// </summary>
    public class StoreReview : MonoBehaviour {

        // どこからでもアクセスできるようにインスタンス化.
        private static StoreReview _instance;
        public static StoreReview Instance {
            get {
                if (!_instance) {
                    System.Type type = typeof(StoreReview);
                    _instance = (StoreReview)FindObjectOfType(type);
                    if (!_instance) {
                        GameObject obj = new GameObject(type.ToString(), type);
                        _instance = obj.GetComponent<StoreReview>();
                    }
                }

                if (!Instance) {
                    Log.Error("Not Found StoreReviewManager.");
                }

                return _instance;
            }
        }

        /// <summary>
        /// 初期化.
        /// </summary>
        private void Awake() {
            // シーンを跨いだときなど、外部の影響で破棄されないようにする.
            DontDestroyOnLoad(gameObject);
        }

        public void RequestReview() {
#if UNITY_IOS && !UNITY_EDITOR
            // iOS.
            ReqeusReviewIOS();
#elif UNITY_ANDROID && !UNITY_EDITOR
            // Android.
            ReqeusReviewAndoirdAsync().Forget();
#else
            Log.Warning("ストアのレビュー促進機能は現在のプレイ環境では対応していません");
#endif
        }


#if UNITY_IOS && !UNITY_EDITOR
        /// <summary>
        /// ストアでのレビューを促すダイアログを表示する(iOS用)
        /// </summary>
        private void ReqeusReviewIOS() {
            UnityEngine.iOS.Device.RequestStoreReview();
        }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        /// <summary>
        /// ストアでのレビューを促すダイアログを表示する(Android用)
        /// </summary>
        private async UniTask ReqeusReviewAndoirdAsync() {
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
