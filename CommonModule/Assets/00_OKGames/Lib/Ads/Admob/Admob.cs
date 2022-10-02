using OKGamesLib;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;

namespace OKGamesLib {

    /// <summary>
    /// Admob広告機能へのアクセスクラス.
    /// </summary>
    public class Admob {

        private AdmobBanner _banner;
        private AdmobInterstitial _interstitial;

        public bool _isInit = false;

        // OSのApp Tracking Transparencyの使用許可があるか
        // Androidは確認なしでtrueでOK.
        // iOSはAppleのガイドラインで選択状態を取得する必要あり(一度も答えたことがなければ初回のみOSのダイアログで追跡機能を利用してよいかユーザーへ尋ねる).
        private bool _isATTPermission = false;


        /// <summary>
        /// 初期化処理
        /// この関数内の処理順番は意図的に記述していっている部分が多いのでむやみに変えないこと.
        /// </summary>
        public async UniTask InitAsync() {
            if (_isInit) {
                Log.Warning("【Admob】: 二度目の初期化を行おうとしています");
                return;
            }

#if UNITY_IOS && !UNITY_EDITOR
            // ユーザー好みの広告を表示させるためにOSの追跡機能を利用して良い状態か確認する。
            _isATTPermission = await GetTrackingEnable();
#else
            // iOS以外はtrueで良い.
            _isATTPermission = true;
#endif

            Log.Notice("【Admob】 広告SDKの初期化処理.");

            MobileAds.Initialize(initStatus => {
                _isInit = true;
            });

            await UniTask.WaitUntil(() => _isInit);

            _banner = new AdmobBanner();
            await _banner.InitAsync(_isATTPermission);

            _interstitial = new AdmobInterstitial();
            await _interstitial.InitAsync(_isATTPermission);

            Log.Notice("【Admob】 広告SDKの初期化終了.");
        }

        /// <summary>
        /// バナーを表示する.
        /// </summary>
        public void ShowBanner() {
            if (_isInit) {
                Log.Warning("【Admob】: 初期化処理がまだされていないので処理できません.");
                return;
            }
            _banner.Show();
        }

        /// <summary>
        /// バナーを非表示にする.
        /// </summary>
        public void HideBanner() {
            if (_isInit) {
                Log.Warning("【Admob】: 初期化処理がまだされていないので処理できません.");
                return;
            }
            _banner.Hide();
        }

        /// <summary>
        /// インタースティシャル広告をを表示する.
        /// 本広告は閉じた時の処理に破棄処理がされる作りになっていて非表示は外部公開していない.)
        /// </summary>
        public void ShowInterstitial() {
            if (_isInit) {
                Log.Warning("【Admob】: 初期化処理がまだされていないので処理できません.");
                return;
            }
            _interstitial.Show();
        }

#if UNITY_IOS && !UNITY_EDITOR
        /// <summary>
        /// 端末のTraking機能を使用しても良いかの情報を取得する.
        /// iOSはAppleガイドライン対応のために広告を出す際には追跡機能の使用許可をユーザーに確認しなければならない(でないとリジェクトされる.).
        /// ユーザーに追跡許可をNGにされた場合は広告はでるが、ユーザー好みの広告表示をAIですることができなくなる.
        /// </summary>
        /// <returns></returns>
        private async UniTask<bool> GetTrackingEnable() {
            bool isTrackinEnable = false;

            var status = NativeService.GetIOSTrackingAuthorizationStatus();

            if (!status.HasValue) {
                // 未回答の場合はATT許可要求ダイアログを表示する.
                status = await NativeService.RequestTrackingAuthorization() as bool?;
                isTrackinEnable = status.Value;
            }

            return isTrackinEnable;
        }
#endif

    }
}