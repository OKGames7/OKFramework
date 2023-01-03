using OKGamesLib;
using System;
using GoogleMobileAds.Api;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// Admobのインタースティシャル広告機能.
    /// </summary>
    public class AdmobInterstitial {

        private InterstitialAd _interstitialView;
        public bool IsInterstitialLoaded = false;
        private bool _isAttPermission = false;

        /// <summary>
        /// 初期化.
        /// </summary>
        /// <param name="isATTPermission">ユーザー好みの広告表示のためにOSのトラッキング機能を使って良いかの使用許可があるか.</param>
        public async UniTask InitAsync(bool isATTPermission) {
            if (IsInterstitialLoaded) {
                Log.Warning("【AdmobInterstitial】: 二度目の初期化を行おうとしています");
                return;
            }
            Log.Notice("【AdmobInterstitial】 広告SDKのインタースティシャル広告生成処理.");

            _isAttPermission = isATTPermission;

            await Create(_isAttPermission);
        }


        /// <summary>
        /// 生成.
        /// </summary>
        /// <param name="isATTPermission">ユーザー好みの広告表示のためにOSのトラッキング機能を使って良いかの使用許可があるか.</param>
        private async UniTask Create(bool isATTPermission) {

#if UNITY_ANDROID
            string adUnitId = !string.IsNullOrEmpty(AppConst.AdmobInterstitialIDAndroid) ? AppConst.AdmobInterstitialIDAndroid : "ca-app-pub-3940256099942544/1033173712"; // 空の場合はAdmobのサンプル広告.
#elif UNITY_IPHONE
            string adUnitId = !string.IsNullOrEmpty(AppConst.AdmobInterstitialIDIOS) ? AppConst.AdmobInterstitialIDIOS : "ca-app-pub-3940256099942544/4411468910"; // 空の場合はAdmobのサンプル広告.
#else
            string adUnitId = "unexpected_platform";
#endif

            Log.Notice("【AdmobInterstitial】 UnitID:" + adUnitId);

            var builder = new AdRequest.Builder();
            if (!isATTPermission) {
                // iOSで情報追跡をNGにされた時でも広告表示するために設定が必要.
                builder = builder.AddExtra("npa", "1");
            }
            // リクエストの生成.
            AdRequest request = builder.Build();


            _interstitialView = new InterstitialAd(adUnitId);

            // ロードできたら非表示にする.
            _interstitialView.OnAdLoaded += (_, _) => {
                // インタースティシャルはバナーと違ってロード終了時に表示はされないので非表示にする処理は不要.
                IsInterstitialLoaded = true;
            };

            // 失敗時は原因が追跡できるように詳細にログを出す.
            _interstitialView.OnAdFailedToLoad += (_, args) => {
                LoadAdError error = args.LoadAdError;
                string domain = error.GetDomain();
                int code = error.GetCode();
                string message = error.GetMessage();
                AdError underlyingError = error.GetCause();
                ResponseInfo info = error.GetResponseInfo();
                Log.Notice($"【AdmobInterstitial】 広告SDKのバナー生成失敗: {error}");
                Log.Notice($"【AdmobInterstitial】\n domain: {domain}\n code: {code}\n message: {message}\n resuponseInfo: {info}\n");
            };

            _interstitialView.OnAdClosed += Close;

            _interstitialView.LoadAd(request);

            await UniTask.WaitUntil(() => IsInterstitialLoaded);

            Log.Notice("【AdmobInterstitial】広告SDKのバナー生成終了.");
        }

        /// <summary>
        /// viewを表示する.
        /// </summary>
        public void Show(Action endCallback) {
            if (_interstitialView == null) {
                Log.Warning("【AdmobInterstitial】インタースティシャル広告の参照がありません");
                return;
            }

            if (!_interstitialView.IsLoaded()) {
                Log.Warning("【AdmobInterstitial】インタースティシャル広告の用意(ロード)ができていません");
                return;
            }

            _interstitialView.OnAdFailedToLoad += (_, args) => {
                endCallback();
            };
            _interstitialView.OnAdClosed += (_, args) => {
                endCallback();
            };

            _interstitialView.Show();
        }

        private void Close(object sender, EventArgs args) {
            // インタースティシャルはクローズ時に破棄しないとメモリリークする.
            _interstitialView.Destroy();
            _interstitialView = null;

            // 閉じたタイミングで次回分の生成はしておく.
            Create(_isAttPermission).Forget();
        }
    }
}
