using OKGamesLib;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

namespace OKGamesFramework {

    /// <summary>
    /// OKGamesの初期化処理を担当するクラス.
    /// </summary>
    public class OKGamesInitializer {

        public IGlobalContext Init() {

            // 実機だとRuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoadで呼ばれると
            // UniTaks側のプレイヤープール設定がデフォルトタイミングで間に合わない(そのフレームでawaitなどが使えない)。
            // そのためUniTaks側のawaitが使えるように明示的にこのタイミングでUniTask側のPlayerLoopを設定している.
            SetupPlayLoop();

            // ゲーム内で唯一存在するEventSystemとすること.
            CreateEventSystem();

            var globalContex = CreateGlobalContext();
            globalContex.Init();

            return globalContex;
        }

        /// <summary>
        /// PlayerLoop設定の初期化を行う.
        /// </summary>
        private void SetupPlayLoop() {
            var loop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopHelper.Initialize(ref loop, InjectPlayerLoopTimings.Minimum);
        }

        /// <summary>
        /// EventSystemの生成.
        /// </summary>
        private void CreateEventSystem() {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            GameObject.DontDestroyOnLoad(eventSystem);
        }

        /// <summary>
        /// GlobalContexを生成して初期化し返却する.
        /// </summary>
        /// <returns>IGlobalContex.</returns>
        private IGlobalContext CreateGlobalContext() {
            var context = new GlobalContext();
            return context;
        }

        /// <summary>
        /// 非同期で行う必要のある初期化処理
        /// </summary>
        public async UniTask InitAsync(IGlobalContext globalContex) {
            // IAP初期化前にUnityService初期化が必要.
            await SetupUnityServiceAsync();

            // 1フレ待ってUnityのAwakeが呼ばれるまで待つ.
            // globalContexの非同期初期化(例:IAPやAd)はAwakeのタイミングからしか処理できないものがあるためここで1フレ待機を入れている.
            await UniTask.Yield();

            // GlobalContexの非同期初期化.
            await globalContex.InitAsync();
        }

        public void InitDebugTool() {
#if DEVELOPMENT
            CreateDebugConsoleUI();
#endif
#if DEVELOPMENT || DEBUG_SIMPLEPROFILE_UI
            CreateDebugFPSUI();
#endif
        }

        /// <summary>
        /// UnityServiceのセットアップ.
        /// </summary>
        private async UniTask SetupUnityServiceAsync() {
            // UnityServiceの初期化(IAPで使用する)
            var options = new InitializationOptions().SetEnvironmentName("production");
            await UnityServices.InitializeAsync(options).ToObservable().ToUniTask();
        }

#if DEVELOPMENT
        /// <summary>
        /// Debug用のログを表示するUIの生成.
        /// </summary>
        private void CreateDebugConsoleUI() {
            // 実機でもログを確認するアセットの生成.
            var debugConsole = Resources.Load("Debug/IngameDebugConsole") as GameObject;
            GameObject.Instantiate(debugConsole);
            // コマンド登録.
            var debugCommandRegister = new OKGamesTest.DebugLogCommandRegisgter();
            debugCommandRegister.Register();
        }
#endif

#if DEVELOPMENT || DEBUG_SIMPLEPROFILE_UI
        /// <summary>
        /// Debug用のFPSとメモリ量の値を表示するUIの生成.
        /// </summary>
        private void CreateDebugFPSUI() {
            // 実機でもログを確認するアセットの生成.
            var simplePlofiler = Resources.Load("Debug/ProfileCanvas") as GameObject;
            var plofilerObj = GameObject.Instantiate(simplePlofiler);
            GameObject.DontDestroyOnLoad(plofilerObj);
        }
#endif

    }
}
