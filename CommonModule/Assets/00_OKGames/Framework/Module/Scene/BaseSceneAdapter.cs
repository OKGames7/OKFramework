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

        public virtual UniTask InitAfterLoadScene(CancellationTokenSource token) {
            _cancelTokenSource = token;
            return UniTask.CompletedTask;
        }

        public virtual void OnStartupScene() {
        }

        public virtual UniTask Finalize() {
            return UniTask.CompletedTask;
        }
    }
}
