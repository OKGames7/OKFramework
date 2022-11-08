using OKGamesLib;
using System;
using UnityEngine;

namespace OKGamesFramework {

    /// <summary>
    /// タイトルシーンのContext.
    /// </summary>
    public class TitleContext : BaseSceneContext {

        public override string GetSceneName() {
            return SceneName.SceneNameEnum.TitleScene.ToString();
        }
    }
}
