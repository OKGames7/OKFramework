using OKGamesFramework;
using OKGamesLib;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesTest {

    /// <summary>
    /// CommonModuleの機能をテストするシーンのContext.
    /// </summary>
    public class TestCommonModuleSceneContext : BaseSceneContext {

        public override string GetSceneName() {
            return SceneName.SceneNameEnum.TestCommonModule.ToString();
        }
    }
}
