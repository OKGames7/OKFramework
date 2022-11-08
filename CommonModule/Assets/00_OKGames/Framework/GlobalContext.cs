using OKGamesLib;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace OKGamesFramework {

    /// <summary>
    /// フレームワークに関するグローバルな情報を制御する.
    /// </summary>
    public class GlobalContext : IGlobalContext {

        public ISceneDirector SceneDirector { get; private set; }

        public IUI UI { get; private set; }

        public ITimeKeeper TimeKeeper { get; private set; }

        public IResourceStore ResourceStore { get; private set; }

        public IUserDataStore UserDataStore { get; private set; }

        public IAudioPlayer BgmPlayer { get; private set; }

        public IAudioPlayer SePlayer { get; private set; }

        public ISignalHub SignalHub { get; private set; }

        public ITweenerHub TweenerHub { get; private set; }

        public IObjectPoolHub ObjectPoolHub { get; private set; }

        public GameObject ContextGameObject => _contextGameObj;
        private GameObject _contextGameObj;
        private GameObject _audioSourceGameObj;

        public void Init() {
            Log.Notice("[GlobalContext] - Init Start");

            // ゲーム全体で使用する各種管理クラスの生成および初期化処理を行う.

            _contextGameObj = new GameObject("OKGamesGlobalContext");
            _audioSourceGameObj = new GameObject("AudioSource");
            GameObject.DontDestroyOnLoad(_contextGameObj);
            GameObject.DontDestroyOnLoad(_audioSourceGameObj);

            ResourceStore = new ResourceStore();

            UserDataStore = new UserDataStore();
            UserDataStore.Load();

            UI = new UI();
            UI.Init();

#if DEVELOPMENT
            SceneDirector = _contextGameObj.AddComponent<DebugSceneDirector>();
#else
            SceneDirector = _contextGameObj.AddComponent<SceneDirector>();
#endif

            SceneDirector.SceneUpdate += OnSceneUpdate;

            TimeKeeper = new TimeKeeper(SceneDirector);

            BgmPlayer = new BgmPlayer();
            SePlayer = new SePlayer();

            SignalHub = new SignalHub(SceneDirector);

            TweenerHub = new TweenerHub(SceneDirector, TimeKeeper);

            ObjectPoolHub = new ObjectPoolHub(SceneDirector);

            // Camera FlagがDepth Onlyだけのカメラだと描画箇所以外の更新がされない.
            // シーン遷移時など画面の描画を更新するためのカメラ.
            CreateInitCamera();

            Log.Notice("[GlobalContext] - Init End");
        }

        public async UniTask InitAsync(IBootConfig bootConfig = null) {
            Log.Notice("[GlobalContext] - InitAsync Start");

            if (bootConfig == null) {
                bootConfig = new DefaultBootConfig();
            }

            // マスターの読み込み.
            string[] masterAddresses = new string[2] {
                 AssetAddress.AssetAddressEnum.texts.ToString(),
                 AssetAddress.AssetAddressEnum.platform_items.ToString()
            };
            var masterLoadAsync = ResourceStore.RetainGlobalWithAutoLoad(masterAddresses);

            // フェード管理オブジェクトを生成する.
            string str = AssetAddress.AssetAddressEnum.Fader.ToString();
            string[] addresses = new string[1] { str };
            var fadeLoadAsync = ResourceStore.RetainGlobalWithAutoLoad(addresses);

            // Addressablesのロード待ち.
            await UniTask.WhenAll(masterLoadAsync, fadeLoadAsync);

            // フェードオブジェクトの生成.
            var fadeScreenObject = ResourceStore.GetGameObj(str);
            fadeScreenObject = GameObject.Instantiate(fadeScreenObject);
            GameObject.DontDestroyOnLoad(fadeScreenObject);

            // サウンド管理系の初期化.
            BgmPlayer.Init(_audioSourceGameObj, bootConfig.numBgmSourcePool, SceneDirector, ResourceStore);
            SePlayer.Init(_audioSourceGameObj, bootConfig.numSeSourcePool, SceneDirector, ResourceStore);

            // UI全般制御クラスの依存性注入.
            var userdata = UserDataStore.Data;
            var lang = userdata.Value != null ? userdata.Value.CurrentLanguage : Language.Ja;
            var textMaster = ResourceStore.GetObj<Entity_text>(AssetAddress.AssetAddressEnum.texts.ToString());
            var uiTransfer = new UITransfer(userdata, ResourceStore, lang, textMaster, SePlayer);
            UI.Inject(uiTransfer);

            // boot設定に記述している内容で設定初期化する.
            bootConfig.OnGameBoot();

            // シーンディレクターの初期化.
            SceneDirector.Init(bootConfig, ResourceStore, fadeScreenObject);

            if (bootConfig.useGlobalAudioListener) {
                // 立体サウンド用のListerをAdd.
                _contextGameObj.AddComponent<AudioListener>();
            }

            Log.Notice("[GlobalContext] - InitAsync End");
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

        /// <summary>
        /// <see cref="ISceneDirector.SceneUpdate">でさせたい処理.
        /// </summary>
        private void OnSceneUpdate() {
            // Global context の GameObject に AudioListener をつけた場合に
            // 3D サウンドも機能させるため、GameObject の位置をカメラと合わせる
            if (Camera.main != null) {
                _contextGameObj.transform.position = Camera.main.transform.position;
            }
        }
    }
}

