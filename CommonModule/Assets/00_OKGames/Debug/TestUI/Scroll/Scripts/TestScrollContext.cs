using OKGamesFramework;
using OKGamesLib;

namespace OKGamesTest {

    /// <summary>
    /// Test用のスクロールシーンのContext.
    /// </summary>
    public class TestScrollContext : BaseSceneContext {

        public override string GetSceneName() {
            return SceneName.SceneNameEnum.TestScroll.ToString();
        }
    }
}
