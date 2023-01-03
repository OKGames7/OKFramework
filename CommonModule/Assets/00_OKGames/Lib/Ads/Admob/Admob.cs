using OKGamesLib;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using System;

namespace OKGamesLib {

    /// <summary>
    /// Admob広告機能へのアクセスクラス.
    /// </summary>
    public class Admob : IAdmob {

        /// <summary>
        /// バナー型の広告制御クラス.
        /// </summary>
        private AdmobBanner _banner = new AdmobBanner();

        /// <summary>
        /// インタースティシャル型の広告制御クラス.
        /// </summary>
        private AdmobInterstitial _interstitial = new AdmobInterstitial();

        public bool _isInit = false;

        // OSのApp Tracking Transparencyの使用許可があるか
        // Androidは確認なしでtrueでOK.
        // iOSはAppleのガイドラインで選択状態を取得する必要あり(一度も答えたことがなければ初回のみOSのダイアログで追跡機能を利用してよいかユーザーへ尋ねる).
        private bool _isATTPermission = false;


        /// <summary>
        /// <see cref="IAdmob.IsInitialized"/>
        /// この関数内の処理順番は意図的に記述していっている部分が多いのでむやみに変えないこと.
        /// </summary>
        public bool IsInitialized() {
            return _isInit && _banner.IsBannerLoaded & _interstitial.IsInterstitialLoaded;
        }

        /// <summary>
        /// <see cref="IAdmob.InitAsync"/>
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

            await _banner.InitAsync(_isATTPermission);

            await _interstitial.InitAsync(_isATTPermission);

            Log.Notice("【Admob】 広告SDKの初期化終了.");
        }

        /// <summary>
        /// <see cref="IAdmob.ShowBanner"/>
        /// </summary>
        public void ShowBanner() {
            if (!IsInitialized()) {
                Log.Warning("【Admob】: 初期化処理がまだされていないので処理できません.");
                return;
            }
            _banner.Show();
        }

        /// <summary>
        /// <see cref="IAdmob.HideBanner"/>
        /// </summary>
        public void HideBanner() {
            if (!IsInitialized()) {
                Log.Warning("【Admob】: 初期化処理がまだされていないので処理できません.");
                return;
            }
            _banner.Hide();
        }

        /// <summary>
        /// <see cref="IAdmob.ShowInterstitial"/>
        /// 本広告は閉じた時の処理に破棄処理がされる作りになっていて非表示は外部公開していない.)
        /// </summary>
        public void ShowInterstitial(Action endCallback) {
            if (!IsInitialized()) {
                Log.Warning("【Admob】: 初期化処理がまだされていないので処理できません.");
                return;
            }
            _interstitial.Show(endCallback);
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
