using OKGamesLib;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesFramework {

    /// <summary>
    /// Bootシーンのアダプタ.
    /// </summary>
    public class BootSceneAdapter : BaseSceneAdapter {

        private ISceneDirector _sceneDirector;

        /// <summary>
        /// <see cref="ISceneAdapter.InitAfterLoadScene"/>.
        /// </summary>
        public override async UniTask InitAfterLoadScene(CancellationTokenSource token) {
            await base.InitAfterLoadScene(token);

            _sceneDirector = OKGames.Context.SceneDirector;
        }

        /// <summary>
        /// <see cref="ISceneAdapter.OnStartupScene"/>.
        /// </summary>
        public override void OnStartupScene() {
#if COMMON_MODULE_DEBUG
            ISceneContext context = new OKGamesTest.TestCommonModuleSceneContext();
            _sceneDirector.GoToNextScene(context).Forget();
#else
            // このフラグとOKGames.IsInitのフラグがtrueになったことをトリガーに
            // PJ側で初期シーン遷移処理を行うこと.
            OKGames.IsPJRoutine = true;
#endif
        }
    }
}
