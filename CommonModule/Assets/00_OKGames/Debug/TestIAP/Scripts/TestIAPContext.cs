using OKGamesFramework;
using OKGamesLib;

namespace OKGamesTest {

    /// <summary>
    /// Test用のIAPシーンのContext.
    /// </summary>
    public class TestIAPContext : BaseSceneContext {

        public override string GetSceneName() {
            return SceneName.SceneNameEnum.TestIAP.ToString();
        }
    }
}
