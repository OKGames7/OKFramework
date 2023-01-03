using OKGamesFramework;
using OKGamesLib;

namespace PJ {

    /// <summary>
    /// タイトルシーンのContext.
    /// </summary>
    public class TitleSceneContext : BaseSceneContext {

        /// <summary>
        /// <see cref="ISceneContext.GetSceneName"/>.
        /// </summary>
        public override string GetSceneName() {
            return SceneName.SceneNameEnum.TitleScene.ToString();
        }
    }
}
