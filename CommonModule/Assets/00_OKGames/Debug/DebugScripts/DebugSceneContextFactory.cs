#if DEVELOPMENT
using OKGamesFramework;
using OKGamesLib;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace OKGamesTest {

    /// <summary>
    /// 開発用.
    /// シーン直起動時にもうまく動作するようにSceneContextを都合の良いように書き換えて操作するためのクラス.
    /// </summary>
    public class DebugSceneContextFactory {

        /// <summary>
        /// シーン定義とSceneContextの紐付けをするマッピング.
        /// シーンが増えた際にはここに定義を増やす.
        /// </summary>
        private readonly Dictionary<string, ISceneContext> _defineSceneMap = new Dictionary<string, ISceneContext>() {
            {SceneName.SceneNameEnum.Boot.ToString(), new BootSceneContext()},
            {SceneName.SceneNameEnum.TestCommonModule.ToString(), new TestCommonModuleSceneContext()},
            {SceneName.SceneNameEnum.TestIAP.ToString(), new TestIAPContext()},
            {SceneName.SceneNameEnum.TestScroll.ToString(), new TestScrollContext()},
        };

        /// <summary>
        /// SceneContextを作成する.
        /// </summary>
        /// <returns>ISceneContext</returns>
        public ISceneContext CreateGetSceneContext() {
            string sceneName = SceneManager.GetActiveScene().name;
            return CreateGetSceneContext(sceneName);
        }

        /// <summary>
        /// 指定したシーン名からSceneContextを作成して取得する.
        /// </summary>
        /// <param name="sceneName">シーン名.</param>
        /// <returns>ISceneContext</returns>
        public ISceneContext CreateGetSceneContext(string sceneName) {
            foreach (var scene in _defineSceneMap) {
                // 文字列を厳密比較すると使いづらいので大文字小文字は一致していなくても良いものとする.
                if (scene.Key.Equals(sceneName, StringComparison.OrdinalIgnoreCase)) {
                    Log.Notice($"{scene.Value.GetSceneName()}のシーンのContextを生成しました.");
                    return scene.Value;
                }
            }

            Log.Warning("【DebugSceneContextFactoy】BuildSettingsに定義されていないシーンのためTestBootシーンを起動します.");
            return _defineSceneMap[SceneName.SceneNameEnum.Boot.ToString()];
        }
    }
}
#endif
