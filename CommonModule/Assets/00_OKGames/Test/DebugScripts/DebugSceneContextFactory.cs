#if DEVELOPMENT
using OKGamesFramework;
using OKGamesTest;
using System;
using UnityEngine.SceneManagement;

namespace OKGamesLib {

    /// <summary>
    /// 開発用.
    /// シーン直起動時にもうまく動作するようにSceneContextを都合の良いように書き換えて操作するためのクラス.
    /// </summary>
    public class DebugSceneContextFactory {

        public ISceneContext CreateGetSceneContext() {
            string sceneName = SceneManager.GetActiveScene().name;
            return CreateGetSceneContext(sceneName);
        }

        public ISceneContext
        CreateGetSceneContext(string sceneName) {
            ISceneContext context;
            if (sceneName.Equals(SceneName.SceneNameEnum.TestBoot.ToString(), StringComparison.OrdinalIgnoreCase)) {
                context = new TestBootContext();
            } else if (sceneName.Equals(SceneName.SceneNameEnum.TestIAP.ToString(), StringComparison.InvariantCultureIgnoreCase)) {
                context = new TestIAPContext();
            } else {
                // 登録されていないシーンであれば、TestBootのContextを作る.
                context = new TestBootContext();
            }

            Log.Notice($"{context.GetSceneName()}のシーンのContextを生成しました.");
            return context;
        }
    }
}
#endif
