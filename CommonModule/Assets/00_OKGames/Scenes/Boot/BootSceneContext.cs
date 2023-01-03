using OKGamesLib;
using System;
using UnityEngine;

namespace OKGamesFramework {

    /// <summary>
    /// 起動シーンのContext.
    /// </summary>
    public class BootSceneContext : BaseSceneContext {

        /// <summary>
        /// <see cref="ISceneContext.GetSceneName"/>.
        /// </summary>
        public override string GetSceneName() {
            return SceneName.SceneNameEnum.Boot.ToString();
        }
    }
}
