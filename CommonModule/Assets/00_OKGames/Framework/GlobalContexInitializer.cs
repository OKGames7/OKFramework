using OKGamesLib;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

namespace OKGamesFramework {

    /// <summary>
    /// GlobalContexの初期化処理を担当するクラス.
    /// </summary>
    public class GlobalContexInitializer {

        public class ResponseData {
            public GameObject ContextGameObject { get; set; }
            public IResourceStore ResourceStore { get; set; }
            public IUserDataStore UserDataStore { get; set; }
            public IUI UI { get; set; }
            public ISceneDirector SceneDirector { get; set; }
            public ITimeKeeper TimeKeeper { get; set; }
            public GameObject AudioSourceGameObj { get; set; }
            public IAudioPlayer BgmPlayer { get; set; }
            public IAudioPlayer SePlayer { get; set; }
            public ISignalHub SignalHub { get; set; }
            public ITweenerHub TweenHub { get; set; }
            public IObjectPoolHub ObjectPoolHub { get; set; }
            public IPrev Prev { get; set; }
            public IInputBlocker InputBlocker { get; set; }
            public IAdmob Ads { get; set; }
            public IIAP IAP { get; set; }
        }

        public ResponseData Init() {
            var res = new ResponseData();
            res.ContextGameObject = new GameObject("OKGamesGlobalContext");
            res.AudioSourceGameObj = new GameObject("AudioSource");
            res.ResourceStore = new ResourceStore();
            res.UserDataStore = new UserDataStore();
            res.UI = new UI();
            res.SceneDirector = res.ContextGameObject.AddComponent<SceneDirector>();
            res.TimeKeeper = new TimeKeeper(res.SceneDirector);
            res.BgmPlayer = new BgmPlayer();
            res.SePlayer = new SePlayer();
            res.SignalHub = new SignalHub(res.SceneDirector);
            res.TweenHub = new TweenerHub(res.SceneDirector, res.TimeKeeper);
            res.ObjectPoolHub = new ObjectPoolHub(res.SceneDirector);
            res.Prev = new Prev();
            res.InputBlocker = new InputBlocker();
            res.Ads = new Admob();
#if UNITY_EDITOR
            res.IAP = new DebugIAP();
#else
            res.IAP = new IAP();
#endif

            // Camera FlagがDepth Onlyだけのカメラだと描画箇所以外の更新がされない.
            // シーン遷移時など画面の描画を更新するためのカメラ.
            CreateInitCamera();

            return res;
        }

        /// <summary>
        /// シーン遷移時など画面全体の描画を更新するためのカメラ.
        /// </summary>
        private void CreateInitCamera() {
            // オブジェクト生成.
            GameObject initCamera = new GameObject("InitCamera");

            // コンポーネントの生成.
            Camera camera = initCamera.AddComponent<Camera>();

            // 必要な設定.
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.depth = -1; // 一番最後に描画されるようにしている.
            camera.cullingMask = 0; // Nothing.
            GameObject.DontDestroyOnLoad(initCamera);
        }


        public async UniTask InitAsync(IGlobalContext context, IBootConfig bootConfig) {
            if (bootConfig == null) {
                bootConfig = new DefaultBootConfig();
            }

            // ローカルマスターデータの読み込み.
            await LoadMasterDataAsync(context.ResourceStore);

            // UIシステム機能のセットアップ.
            await SetupUIAsync(context.UI, context.UserDataStore, context.ResourceStore, context.ObjectPoolHub, context.SePlayer, context.Prev);

            // サウンド管理系の初期化.
            context.BgmPlayer.Init(context.AudioSourceGameObj, bootConfig.numBgmSourcePool, context.SceneDirector, context.ResourceStore);
            context.SePlayer.Init(context.AudioSourceGameObj, bootConfig.numSeSourcePool, context.SceneDirector, context.ResourceStore);

            // 広告初期化.
            // 広告は初期化に時間がかかるのと、なくてもゲームは成立するので敢えて待っていない.
            context.Ads.InitAsync().Forget();

            // 課金システムのセットアップ.
            // 課金は初期化に時間がかかるのと、なくてもゲームは成立するので敢えて待っていない.
            SetupIPAAsync(context.IAP, context.ResourceStore).Forget();

            // 戻る機能のセットアップ.
            var textMaster = context.ResourceStore.GetObj<Entity_text>(AssetAddress.AssetAddressEnum.texts.ToString());
            context.Prev.Inject(context.UI.PopNotify, context.InputBlocker, textMaster);
            context.Prev.Bind(context.ContextGameObject);

            // boot設定に記述している内容で設定初期化する.
            bootConfig.OnGameBoot();

            // シーンディレクターの初期化.
            context.SceneDirector.Init(bootConfig, context.ResourceStore, context.UI.Fader, context.InputBlocker);

            if (bootConfig.useGlobalAudioListener) {
                // 立体サウンド用のListerをAdd.
                context.ContextGameObject.AddComponent<AudioListener>();
            }
        }

        /// <summary>
        /// マスターデータをロードする.
        /// </summary>
        /// <param name="store">リソースのストア.</param>
        /// <returns>UniTask</returns>
        private async UniTask LoadMasterDataAsync(IResourceStore store) {
            // マスターの読み込み.
            string[] masterAddresses = new string[2] {
                 AssetAddress.AssetAddressEnum.texts.ToString(),
                 AssetAddress.AssetAddressEnum.platform_items.ToString()
            };
            await store.RetainGlobalWithAutoLoad(masterAddresses);
        }

        /// <summary>
        /// UIシステム群のセットアップ.
        /// </summary>
        /// <param name="ui">UI管理クラス.</param>
        /// <param name="userDatastore">ユーザーデータのストア.</param>
        /// <param name="resourceStore">リソースデータのストア.</param>
        /// <param name="sePlayer">SEのプレイヤー.</param>
        /// <returns>UniTask.</returns>
        private async UniTask SetupUIAsync(IUI ui, IUserDataStore userDatastore, IResourceStore resourceStore, IObjectPoolHub poolHub, IAudioPlayer sePlayer, IPrev prev) {
            // UI全般制御クラスの依存性注入.
            var userdata = userDatastore.Data;
            var lang = userdata.Value != null ? userdata.Value.CurrentLanguage : Language.Ja;
            var textMaster = resourceStore.GetObj<Entity_text>(AssetAddress.AssetAddressEnum.texts.ToString());
            var uiTransfer = new UITransfer(userdata, resourceStore, lang, textMaster, sePlayer);
            ui.Inject(uiTransfer);
            // UI系のAddressablesをロードする.
            // UI.Injectの後に呼ぶこと.
            await ui.InitAsync(resourceStore, poolHub, prev);
        }

        /// <summary>
        /// 課金システムのセットアップ.
        /// </summary>
        /// <param name="iap">課金システム.</param>
        /// <param name="resourceStore">リソースデータのストア.</param>
        /// <returns>UniTask.</returns>
        private async UniTask SetupIPAAsync(IIAP iap, IResourceStore resourceStore) {
            var itemMaster = resourceStore.GetObj<Entity_platform_item>(AssetAddress.AssetAddressEnum.platform_items.ToString());
            var productList = itemMaster.GetItemsByCurrentPlatform().ConvertToStoreItem().ToArray();
            await iap.InitAsync(productList);
        }
    }
}
