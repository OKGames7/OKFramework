using OKGamesLib;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

namespace OKGamesFramework {

    /// <summary>
    /// フレームワークに関するグローバルな情報を制御する.
    /// </summary>
    public class GlobalContext : IGlobalContext {

        private GameObject _contextGameObj;
        public GameObject ContextGameObject => _contextGameObj;

        private ISceneDirector _sceneDirector;
        public ISceneDirector SceneDirector => _sceneDirector;

        private IUI _ui;
        public IUI UI => _ui;

        private ITimeKeeper _timeKeeper;
        public ITimeKeeper TimeKeeper => _timeKeeper;

        private IResourceStore _resourceStore;
        public IResourceStore ResourceStore => _resourceStore;

        private IUserDataStore _userDataStore;
        public IUserDataStore UserDataStore => _userDataStore;

        public GameObject AudioSourceGameObj => _audioSourceGameObj;
        private GameObject _audioSourceGameObj;
        private IAudioPlayer _bgmPlayer;
        public IAudioPlayer BgmPlayer => _bgmPlayer;

        private IAudioPlayer _sePlayer;
        public IAudioPlayer SePlayer => _sePlayer;

        private ISignalHub _signalHub;
        public ISignalHub SignalHub => _signalHub;

        private ITweenerHub _tweenerHub;
        public ITweenerHub TweenerHub => _tweenerHub;

        private IObjectPoolHub _objectPoolHub;
        public IObjectPoolHub ObjectPoolHub => _objectPoolHub;

        private IPrev _prev;
        public IPrev Prev => _prev;

        private IInputBlocker _inputBlocker;
        public IInputBlocker InputBlocker => _inputBlocker;

        private IIAP _iap;
        public IIAP IAP => _iap;

        private IAdmob _ads;
        public IAdmob Ads => _ads;



        public void Init() {
            Log.Notice("[GlobalContext] - Init Start");

            var initializer = new GlobalContexInitializer();
            var res = initializer.Init();

            // 生成したシステムをメンバ変数へ格納.
            _contextGameObj = res.ContextGameObject;
            _sceneDirector = res.SceneDirector;
            _ui = res.UI;
            _timeKeeper = res.TimeKeeper;
            _resourceStore = res.ResourceStore;
            _userDataStore = res.UserDataStore;
            _audioSourceGameObj = res.AudioSourceGameObj;
            _bgmPlayer = res.BgmPlayer;
            _sePlayer = res.SePlayer;
            _signalHub = res.SignalHub;
            _tweenerHub = res.TweenHub;
            _objectPoolHub = res.ObjectPoolHub;
            _prev = res.Prev;
            _inputBlocker = res.InputBlocker;
            _iap = res.IAP;
            _ads = res.Ads;

            // 自身を破棄されないオブジェクトとして登録する.
            GameObject.DontDestroyOnLoad(res.ContextGameObject);
            // オーディオオブジェクトの階層を子に設定(破棄されないようになる).
            res.AudioSourceGameObj.SetParent(res.ContextGameObject);

            // ストレージデータの読み込みを行う.
            res.UserDataStore.Load();

            // UIシステム系の初期化.
            res.UI.Init(res.ContextGameObject.transform);

            // シーン管理クラスのUpdate処理にイベント追加.
            res.SceneDirector.SceneUpdate += OnSceneUpdate;

            Log.Notice("[GlobalContext] - Init End");
        }

        public async UniTask InitAsync(IBootConfig bootConfig = null) {
            Log.Notice("[GlobalContext] - InitAsync Start");

            var initializer = new GlobalContexInitializer();
            await initializer.InitAsync(this, bootConfig);

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

