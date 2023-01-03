using OKGamesFramework;
using OKGamesLib;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UniRx;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJ {

    /// <summary>
    /// PJ全体で扱う汎用的なモデュールのグローバルアクセスポイント.
    /// OKGamesのクラスをPJ側の各コードから直接参照するのではなくここを介するようにすることで保守しやすくしている.
    /// </summary>
    public class PJContext {

        private static FWBridge _bridge;

        public static ISceneDirector SceneDirector => _sceneDirector;
        private static ISceneDirector _sceneDirector => _bridge.SceneDirector;

        public static IResourceStore ResourceStore => _resourceStore;
        private static IResourceStore _resourceStore => _bridge.ResourceStore;

        public static IAudioPlayer BgmPlayer => _bgmPlayer;
        private static IAudioPlayer _bgmPlayer => _bridge.BgmPlayer;

        public static IAudioPlayer SePlayer => _sePlayer;
        private static IAudioPlayer _sePlayer => _bridge.SePlayer;

        /// <summary>
        /// ゲームプレイ時に一番初めに呼び出される処理.
        /// OKGamesの初期化が終わってから呼ばれれば良くタイミング的にはAfterSceneLoadで十分.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init() {
            _bridge = new FWBridge();
            InitAsync().Forget();
        }

        private static async UniTask InitAsync() {
            // OKGames側の初期化が終わるまで待つ.
            Func<bool> checkInitializedFW = (() => _bridge.GetBootType() != BootType.NotReady);
            await UniTask.WaitUntil(checkInitializedFW);

            if (_bridge.GetBootType() == BootType.CommonModuleDebug) {
                // 共通機能を確かめる用のプレイ設定のためPJ側の処理は何もしない.
                return;
            }

            // BGMロード.
            string address = AssetAddress.AssetAddressEnum.title_bgm.ToString();
            await LoadAsyncIfNotContains(address);
            // BGM再生
            var resourceStore = PJContext.ResourceStore;
            var bgmPlayer = PJContext.BgmPlayer;
            AudioClip clip = resourceStore.GetAudio(address);
            bgmPlayer.Play(clip, true);

            await UniTask.Delay(3000);

            bgmPlayer.FadeOut(3.0f).Forget();


            // バナー広告の表示.
            // 待たなくても他の処理は困らないので並列で呼ぶ.
            ShowBannerAd().Forget();

            // タイトルシーンへ遷移する.
            ISceneContext context = new TitleContext();
            _sceneDirector.GoToNextScene(context).Forget();
        }

        public static async UniTask ShowBannerAd() {
            await UniTask.WaitUntil(() => _bridge.Admob.IsInitialized());
            // バナー広告の表示
            _bridge.Admob.ShowBanner();
        }

        public static async UniTask ShowInterstitialAd(Action endCallback) {
            await UniTask.WaitUntil(() => _bridge.Admob.IsInitialized());
            // インタースティシャル広告の表示
            _bridge.Admob.ShowInterstitial(endCallback);
        }

        public static async UniTask LoadAsyncIfNotContains(params string[] addresses) {
            List<string> addressList = new List<string>();
            for (int i = 0; i < addresses.Length; ++i) {
                if (_resourceStore.Contains(addresses[i])) {
                    continue;
                }
                addressList.Add(addresses[i]);
            }

            if (0 < addressList.Count) {
                await _resourceStore.RetainGlobalWithAutoLoad(addressList.ToArray());
            }
        }
    }
}
