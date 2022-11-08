using OKGamesLib;
using System.Threading;
using UniRx;
using Cysharp.Threading.Tasks;

namespace OKGamesFramework {

    /// <summary>
    /// 通常シーンのContextの規定クラス.
    /// </summary>
    public abstract class BaseSceneContext : ISceneContext {

        public bool IsReady { get; set; } = false;

        public CancellationTokenSource CancelTokenSource => _cancelTokenSource;
        private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        protected ISceneAdapter adapter = null;

        public virtual void Update() {
        }
        public abstract string GetSceneName();

        public virtual void RetainResource() {
        }

        public virtual UniTask InitBeforeLoadScene() {
            return UniTask.CompletedTask;
        }

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

        public virtual void OnStartupScene() {
            if (adapter != null) {
                adapter.OnStartupScene();
            }
        }

        public virtual async UniTask Finalize() {
            if (adapter != null) {
                await adapter.Finalize();
            }
        }

        public virtual UniTask CustomFadeIn() {
            return UniTask.CompletedTask;
        }

        public virtual UniTask CustomFadeOut() {
            return UniTask.CompletedTask;
        }
    }
}
