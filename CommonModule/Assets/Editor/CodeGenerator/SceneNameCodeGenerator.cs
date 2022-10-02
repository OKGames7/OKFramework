using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace OKGamesLib {

    /// <summary>
    /// シーン名の定数を生成するGenerator.
    /// </summary>
    public class SceneNameCodeGenerator : CodeGenerator {
        public override string OutputFileName { get; set; } = "SceneName.gen.cs";
        public override string ClassName { get; set; } = "SceneName";

        protected override void WriteInner(StringBuilder builder) {
            var sceneNameSet = new HashSet<string>();
            foreach (var scene in EditorBuildSettings.scenes) {
                string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                sceneNameSet.Add(sceneName);
            }
            AppendSymbols(builder, sceneNameSet);
        }
    }
}
