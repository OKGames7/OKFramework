#if DEVELOPMENT
using OKGamesFramework;
using Cysharp.Threading.Tasks;

namespace OKGamesTest {

    /// <summary>
    /// DebugLogConsoleのアセットへCommand機能を追加するための開発用スクリプト.
    /// </summary>
    public class DebugLogCommandRegisgter {

        /// <summary>
        /// DebugLogConsoleへ各種Commandを登録する.
        /// </summary>
        public void Register() {
            RegisterGotoScene();
        }

        /// <summary>
        /// シーン遷移機能を登録する.
        /// </summary>
        private void RegisterGotoScene() {
            IngameDebugConsole.DebugLogConsole.AddCommand<string>("scene", "goto scene",
                sceneName => {
                    var sceneContextFactory = new DebugSceneContextFactory();
                    var context = sceneContextFactory.CreateGetSceneContext(sceneName);
                    OKGames.Context.SceneDirector.GoToNextScene(context).Forget();
                });
        }
    }
}
#endif
