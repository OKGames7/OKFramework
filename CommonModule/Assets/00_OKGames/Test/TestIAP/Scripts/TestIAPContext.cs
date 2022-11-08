using OKGamesFramework;
using OKGamesLib;
using System;
using UnityEngine;

namespace OKGamesTest {

    /// <summary>
    /// Test用の起動シーンのContext.
    /// </summary>
    public class TestIAPContext : BaseSceneContext {

        public override string GetSceneName() {
            return SceneName.SceneNameEnum.TestIAP.ToString();
        }
    }
}
