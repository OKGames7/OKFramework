using OKGamesLib;
using System.Threading;
using UniRx;
using Cysharp.Threading.Tasks;

namespace OKGamesFramework {

    /// <summary>
    /// 通常シーンのContextの規定クラス.
    /// </summary>
    public abstract class BaseSceneContext : ISceneContext {

        /// <summary>
        /// <see cref="ISceneContext.IsReady"/>.
        /// </summary>
        public bool IsReady { get; set; } = false;

        /// <summary>
        /// <see cref="ISceneContext.CancelTokenSource"/>.
        /// </summary>
        public CancellationTokenSource CancelTokenSource => _cancelTokenSource;
        private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        protected ISceneAdapter adapter = null;

        /// <summary>
        /// <see cref="ISceneContext.Update"/>.
        /// </summary>
        public virtual void Update() {
        }

        /// <summary>
        /// <see cref="ISceneContext.GetSceneName"/>.
        /// </summary>
        public abstract string GetSceneName();

        /// <summary>
        /// <see cref="ISceneContext.RetainResource"/>.
        /// </summary>
        public virtual void RetainResource() {
        }

        /// <summary>
        /// <see cref="ISceneContext.InitBeforeLoadScene"/>.
        /// </summary>
        public virtual UniTask InitBeforeLoadScene() {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// <see cref="ISceneContext.InitAfterLoadScene"/>.
        /// </summary>
        public virtual async UniTask InitAfterLoadScene() {
            /// シーンのPresenterをシーン内から検索して変数に格納する.
            adapter = ObjectUtility.GetComponentInSceneRootObjects<ISceneAdapter>();
            if (adapter == null) {
                Log.Warning("[BaseSceneContext] presenterの取得に失敗しました.");
                return;
            }
            Log.Notice("[BaseSceneContext] presenterの初期化開始");
            await adapter.InitAfterLoadScene(_cancelTokenSource);
        }

        /// <summary>
        /// <see cref="ISceneContext.OnStartupScene"/>.
        /// </summary>
        public virtual void OnStartupScene() {
            if (adapter != null) {
                adapter.OnStartupScene();
            }
        }

        /// <summary>
        /// <see cref="ISceneContext.Finalize"/>.
        /// </summary>
        public virtual async UniTask Finalize() {
            if (adapter != null) {
                await adapter.Finalize();
            }
        }

        /// <summary>
        /// <see cref="ISceneContext.CustomFadeIn"/>.
        /// </summary>
        public virtual UniTask CustomFadeIn() {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// <see cref="ISceneContext.CustomFadeOut"/>.
        /// </summary>
        public virtual UniTask CustomFadeOut() {
            return UniTask.CompletedTask;
        }
    }
}
