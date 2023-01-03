using OKGamesFramework;
using OKGamesLib;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PJ {

    /// <summary>
    /// タイトルシーンのアダプタ.
    /// </summary>
    public class TitleSceneAdapter : BaseSceneAdapter {

        private ISceneDirector _sceneDirector;

        /// <summary>
        /// <see cref="ISceneAdapter.InitAfterLoadScene"/>.
        /// </summary>
        public override async UniTask InitAfterLoadScene(CancellationTokenSource token) {
            await base.InitAfterLoadScene(token);

            _sceneDirector = PJContext.SceneDirector;
        }

        /// <summary>
        /// <see cref="ISceneAdapter.OnStartupScene"/>.
        /// </summary>
        public override void OnStartupScene() {
            ISceneContext context = new OKGamesTest.TestCommonModuleSceneContext();
            _sceneDirector.GoToNextScene(context).Forget();
        }
    }
}
