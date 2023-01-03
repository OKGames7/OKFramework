using OKGamesLib;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;

namespace OKGamesLib {

    /// <summary>
    /// Admobのバナー広告機能.
    /// </summary>
    public class AdmobBanner {

        private BannerView _bannerView;
        public bool IsBannerLoaded = false;

        /// <summary>
        /// 初期化.
        /// </summary>
        /// <param name="isATTPermission">ユーザー好みの広告表示のためにOSのトラッキング機能を使って良いかの使用許可があるか.</param>
        public async UniTask InitAsync(bool isATTPermission) {

            if (IsBannerLoaded) {
                Log.Warning("【AdmobBanner】: 二度目の初期化を行おうとしています");
                return;
            }
            Log.Notice("【AdmobBanner】 広告SDKのバナー生成処理.");

#if UNITY_ANDROID
            string adUnitId = !string.IsNullOrEmpty(AppConst.AdmobBannerIDAndroid) ? AppConst.AdmobBannerIDAndroid : "ca-app-pub-3940256099942544/6300978111"; // 空の場合はAdmobのサンプル広告.
#elif UNITY_IPHONE
            string adUnitId = !string.IsNullOrEmpty(AppConst.AdmobBannerIDIOS) ? AppConst.AdmobBannerIDIOS : "ca-app-pub-3940256099942544/2934735716"; // 空の場合はAdmobのサンプル広告.
#else
            string adUnitId = "unexpected_platform";
#endif
            Log.Notice("【AdmobBanner】 UnitID:" + adUnitId);

            var builder = new AdRequest.Builder();
            if (!isATTPermission) {
                // iOSで情報追跡をNGにされた時でも広告表示するために設定が必要.
                builder = builder.AddExtra("npa", "1");
            }
            // リクエストの生成.
            AdRequest request = builder.Build();

            // バナーを表示するためのviewを生成.
            _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

            // ロードできたら非表示にする.
            _bannerView.OnAdLoaded += (_, _) => {
                Hide();
                IsBannerLoaded = true;
            };

            // 失敗時は原因が追跡できるように詳細にログを出す.
            _bannerView.OnAdFailedToLoad += (_, args) => {
                LoadAdError error = args.LoadAdError;
                string domain = error.GetDomain();
                int code = error.GetCode();
                string message = error.GetMessage();
                AdError underlyingError = error.GetCause();
                ResponseInfo info = error.GetResponseInfo();
                Log.Notice($"【AdmobBanner】 広告SDKのバナー生成失敗: {error}");
                Log.Notice($"【AdmobBanner】\n domain: {domain}\n code: {code}\n message: {message}\n resuponseInfo: {info}\n");
            };

            // バナーをリクエスト情報からロードする.
            _bannerView.LoadAd(request);

            await UniTask.WaitUntil(() => IsBannerLoaded);

            Log.Notice("【AdmobBanner】広告SDKのバナー生成終了.");
        }

        /// <summary>
        /// viewを表示する.
        /// </summary>
        public void Show() {
            if (_bannerView == null) {
                Log.Warning("【AdmobBanner】: 非表示にするバナーの参照がありません.");
                return;
            }

            _bannerView.Show();
        }

        /// <summary>
        /// viewを非表示にする.
        /// </summary>
        public void Hide() {
            if (_bannerView == null) {
                Log.Warning("【AdmobBanner】: 非表示にするバナーの参照がありません.");
                return;
            }

            _bannerView.Hide();
        }
    }
}
