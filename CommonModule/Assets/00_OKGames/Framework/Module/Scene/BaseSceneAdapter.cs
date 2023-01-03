using OKGamesLib;
using System.Threading;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesFramework {

    /// <summary>
    /// 通常シーンのアダプタの規定クラス.
    /// </summary>
    public class BaseSceneAdapter : MonoBehaviour, ISceneAdapter {
        public CancellationTokenSource CancelTokenSource => _cancelTokenSource;
        private CancellationTokenSource _cancelTokenSource;

        /// <summary>
        /// <see cref="ISceneAdapter.InitAfterLoadScene"/>.
        /// </summary>
        public virtual UniTask InitAfterLoadScene(CancellationTokenSource token) {
            _cancelTokenSource = token;
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// <see cref="ISceneAdapter.OnStartupScene"/>.
        /// </summary>
        public virtual void OnStartupScene() {
        }

        /// <summary>
        /// <see cref="ISceneAdapter.Finalize"/>.
        /// </summary>
        public virtual UniTask Finalize() {
            return UniTask.CompletedTask;
        }
    }
}
