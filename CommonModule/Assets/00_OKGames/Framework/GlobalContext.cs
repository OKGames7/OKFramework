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

            SceneDirector = _contextGameObj.AddComponent<SceneDirector>();
            SceneDirector.SceneUpdate += OnSceneUpdate;

            UI = new UI();
            UI.Init();

            TimeKeeper = new TimeKeeper(SceneDirector);

            BgmPlayer = new BgmPlayer();
            SePlayer = new SePlayer();


            SignalHub = new SignalHub(SceneDirector);

            TweenerHub = new TweenerHub(SceneDirector, TimeKeeper);

            ObjectPoolHub = new ObjectPoolHub(SceneDirector);

            Log.Notice("[GlobalContext] - Init End");
        }

        public async UniTask InitAsync(IBootConfig bootConfig = null) {
            Log.Notice("[GlobalContext] - InitAsync Start");

            if (bootConfig == null) {
                bootConfig = new DefaultBootConfig();
            }

            // フェード管理オブジェクトを生成する.
            string str = AssetAddress.AssetAddressEnum.Fader.ToString();
            string[] addresses = new string[1] { str };
            await ResourceStore.RetainGlobalWithAutoLoad(addresses);
            var fadeScreenObject = ResourceStore.GetGameObj(str);
            fadeScreenObject = GameObject.Instantiate(fadeScreenObject);
            GameObject.DontDestroyOnLoad(fadeScreenObject);

            // シーンディレクターの初期化.
            SceneDirector.Init(bootConfig, ResourceStore, fadeScreenObject);

            // サウンド管理系の初期化.
            BgmPlayer.Init(_audioSourceGameObj, bootConfig.numBgmSourcePool, SceneDirector, ResourceStore);
            SePlayer.Init(_audioSourceGameObj, bootConfig.numSeSourcePool, SceneDirector, ResourceStore);

            if (bootConfig.useGlobalAudioListener) {
                // 立体サウンド用のListerをAdd.
                _contextGameObj.AddComponent<AudioListener>();
            }

            // boot設定に記述している内容で設定初期化する.
            bootConfig.OnGameBoot();

            Log.Notice("[GlobalContext] - InitAsync End");
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
