using OKGamesLib;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UniRx;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OKGamesFramework {

    // ---------------------------------------------------------
    // 共通モジュールへのグローバルアクセスポイント.
    // ---------------------------------------------------------
    public class OKGames {

        private static IGlobalContext _context = null;
        public static IGlobalContext Context {
            get { return _context; }
        }

        public static bool HasGlobalContext => (_context != null);

        public static ISceneDirector SceneDirector {
            get { return Context.SceneDirector; }
        }

        // SceneDirectorに指示されるSceneContextの取得.
        public static ISceneContext SceneContext {
            get { return Context.SceneDirector?.CurrentSceneContext; }
        }

        public static bool HasSceneContext => (SceneContext != null);

        public static float dt {
            get { return Context.TimeKeeper.dt; }
        }

        public static float t {
            get { return Context.TimeKeeper.t; }
        }

        public static ITimeKeeper Time {
            get { return Context.TimeKeeper; }
        }

        public static IResourceStore ResourceStore {
            get { return Context.ResourceStore; }
        }

        public static IAudioPlayer BgmPlayer {
            get { return Context.BgmPlayer; }
        }

        public static IAudioPlayer SePlayer {
            get { return Context.SePlayer; }
        }

        public static ITweener Tweener {
            get { return Context.TweenerHub.SceneScopeTweener; }
        }

        public static IAdmob Ads {
            get { return Context.Ads; }
        }

        public static IIAP Iap {
            get { return Context.IAP; }
        }

        public static IUI UI {
            get { return Context.UI; }
        }

        /// <summary>
        /// 初期化済みか.
        /// InitializeAsyncの終了まで待つ必要がある場合はこのフラグをみること.
        /// </summary>
        public static bool IsInit { get; private set; } = false;

        /// <summary>
        /// 制作しているPJ側の開始遷移挙動を取るために使用するフラグ.
        /// </summary>
        public static bool IsPJRoutine { get; set; } = false;

        /// <summary>
        /// ゲームプレイ時に一番初めに呼び出される処理.
        /// ゲーム起動時のメインループの一番頭の処理とする.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() {
            if (IsInit) {
                Log.Warning("【OKGames】: 初期化済み.");
                return;
            }
            Log.Notice("【OKGames】: Init Start.");

            // 初期化(同期)の実処理.
            var initializer = new OKGamesInitializer();
            _context = initializer.Init();

            // 非同期で初期化が必要な部分の初期化処理を行う.
            InitAsync(_context).Forget();
        }

        /// <summary>
        /// ゲームプレイ時に一番初めに呼び出される処理.
        /// ゲーム起動時のメインループの一番頭の処理とする.
        /// </summary>
        private static async UniTask InitAsync(IGlobalContext globalContex) {
            if (IsInit) {
                Log.Warning("【OKGames】: 初期化済み.");
                return;
            }

            // 初期化(非同期)の実処理.
            var initializer = new OKGamesInitializer();
            await initializer.InitAsync(globalContex);

            // 各種システムの初期化が終わってから最初のシーンを表示する.
            IsPJRoutine = true;

            IsInit = true;
            Log.Notice("【OKGames】: InitializeAsync End.");
        }

        /// <summary>
        /// ゲームプレイ時に一番初めに呼び出される処理.
        /// ゲーム起動時のメインループの一番頭の処理とする.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitDebugTool() {
            // 初期化(同期)の実処理.
            var initializer = new OKGamesInitializer();
            initializer.InitDebugTool();
        }

        /// <summary>
        /// Signalのインスタンスを取得する.
        /// シーンを跨いだ際には破棄される.
        ///
        /// 使用例.
        /// public class MySignal : Signal {}
        /// public class MySignalWithParam : Signal<int> {}
        ///
        /// OKGames.Signal<MySignal>().Connect(handler);      // subscribe
        /// OKGames.Signal<MySignal>().ConnectOnce(handler);  // subscribe one-shoft event
        /// OKGames.Signal<MySignal>().Disconnect(handler);   // unsubscribe
        /// OKGames.Signal<MySignal>().Emit();                // publish (connected handlers are invoked)
        /// OKGames.Signal<MySignalWithParam>().Emit(123);    // publish with argument
        ///
        /// </summary>
        /// <typeparam name="T"><see cref="ISignal"/>を継承したカスタムクラス.</typeparam>
        /// <returns>T</returns>
        public static T Signal<T>() where T : ISignal, new() {
            return Context.SignalHub.SceneScopeSignalRegistry.GetOrCreate<T>();
        }

        /// <summary>
        /// <see cref="Signal"/>関数.
        /// ライフのスコープがグローバル(シーンを跨いでも破棄されない)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GlobalSignal<T>() where T : ISignal, new() {
            return Context.SignalHub.GlobalScopeSignalRegistry.GetOrCreate<T>();
        }

        /// <summary>
        /// tween処理をさせるオブジェクトを登録した新規のtweenを取得する.
        ///
        /// OnUpdateでイージング中の値を受け取って好きな処理に使える.
        /// 以下がその処理例.
        /// OKGames.Tween().FromTo(0f, 1.0f, 2.0f).OnUpdate(x => {
        ///     someImage.material.SetFloat("shaderProp", x);
        /// });
        ///
        /// awaitしたい場合はAsync()をつけること.
        /// シーン遷移時には自動でキャンセルされる、キャンセルしたくない場合はAsync(false)すればいい。
        /// 以下がその例.
        /// await OKGames.Tween().FromTo(0f, 1.0f, 2.0f).SetAplpha(graphic).Async(false);
        /// </summary>
        /// <param name="obj">tween処理させるオブジェクト.</param>
        /// <returns>ITween</returns>
        public static ITween Tween(object obj = null) {
            return Context.TweenerHub.SceneScopeTweener.NewTween(obj);
        }

        /// <summary>
        /// Behaviourを継承したクラスを付与しているオブジェクト用のPoolを作成する.
        ///
        /// 使う前にオリジナルのGameObjectで初期化が必要.
        /// 初期プールサイズは指定可能.
        ///
        /// 記述例.
        ///
        /// //プール対象は<see cref=PoolableBehaviour"/>を継承している必要がある.
        /// CreateObjectPool<YourComponent>(originalGameObj, 64);
        ///
        /// // プールからオブジェクトを取得.
        /// ObjectPool<YourComponent>().Get();
        ///
        /// // プールに返却する時はオブジェクト側で以下を呼ぶ.
        /// ReturnToPool();
        ///
        /// </summary>
        /// <typeparam name="T"/>プールするオブジェクトに付与しているクラス.</typeparam>
        /// <param name="original">プール元のオブジェクト.</param>
        /// <param name="reserveNum">プール数.</param>
        /// <returns></returns>
        public static ObjectPool<T> CreateObjectPool<T>(GameObject original, int reserveNum, GameObject parent = null) where T : PoolableBehaviour {
            return Context.ObjectPoolHub.SceneScopeObjectPoolRegistry.CreatePool<T>(original, reserveNum, parent);
        }

        /// <summary>
        /// 指定したオブジェクトプールの取得.
        /// </summary>
        /// <typeparam name="T"/>プールするオブジェクトに付与しているクラス.</typeparam>
        /// <returns>ObjectPool</returns>
        public static ObjectPool<T> ObjectPool<T>() where T : PoolableBehaviour {
            return Context.ObjectPoolHub.SceneScopeObjectPoolRegistry.GetPool<T>();
        }

        /// <summary>
        /// UniTaskをシーン遷移時に自動でキャンセルされるUniTaskに変換する.
        /// 各所でCancelTokenSourceの紐付けをして該当リソースが破棄される時に処理キャンセルをするのは面倒なのでここでラップしてしている.
        /// </summary>
        /// <param name="task">シーン遷移時にキャンセルするよう登録するUniTask.</param>
        /// <returns>UniTask</returns>
        public static UniTask Async(UniTask task) {
            var ct = SceneContext.CancelTokenSource.Token;
            return task.AttachExternalCancellation(ct);
        }

        /// <summary>
        /// 指定秒待機して、アクションを実行する.
        /// </summary>
        /// <param name="seconds">待機する時間.</param>
        /// <param name="action">待機後処理するアクション.</param>
        /// <returns></returns>
        public static UniTask Wait(float seconds, Action action = null) {
            return Async(Time.Wait(seconds, action));
        }
    }
}
