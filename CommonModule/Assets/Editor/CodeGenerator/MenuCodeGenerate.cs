using UnityEditor;

namespace OKGamesLib {

    /// <summary>
    /// Addressables関連や、LayerやTagなど、Editer内で定義されている情報をもとに、定数を自動生成するためのメニュー.
    /// </summary>
    public class MenuCodeGenerate {

        /// <summary>
        /// 本クラスの各関数で対応している全ての定数自動生成を実行する.
        /// </summary>
        [MenuItem("o.k.games/07.Generate/Generate All Auto Code", false)]
        private static void GenerateAutoCode() {
            GenerateAddressableAutoCode();
            GenerateLayerAutoCode();
            GenerateSceneAutoCode();
            GenerateSceneTagCode();
        }

        /// <summary>
        /// AddressablesのAddress値とラベルの定数を生成する.
        /// </summary>
        [MenuItem("o.k.games/07.Generate/Generate Addressables Auto Code", false)]
        private static void GenerateAddressableAutoCode() {
            var addressableAddressGenerator = new AssetAddressCodeGenerator();
            var addressableLabelGenarator = new AssetLabelCodeGenerator();
            addressableAddressGenerator.Generate();
            addressableLabelGenarator.Generate();
        }

        /// <summary>
        /// Layer値とMask値の定数を生成する.
        /// </summary>
        [MenuItem("o.k.games/07.Generate/Generate Layer Auto Code", false)]
        private static void GenerateLayerAutoCode() {
            var layerNameGenerator = new LayerNameCodeGenerator();
            var layerMaskCodeGenerator = new LayerMaskCodeGenerator();
            layerNameGenerator.Generate();
            layerMaskCodeGenerator.Generate();
        }

        /// <summary>
        /// シーン名の定数を生成する.
        /// </summary>
        [MenuItem("o.k.games/07.Generate/Generate Scene Auto Code", false)]
        private static void GenerateSceneAutoCode() {
            var sceneNameGenerator = new SceneNameCodeGenerator();
            sceneNameGenerator.Generate();
        }

        /// <summary>
        /// Tag名の定義を生成する.
        /// </summary>
        [MenuItem("o.k.games/07.Generate/Generate Tag Auto Code", false)]
        private static void GenerateSceneTagCode() {
            var tagCodeGenerator = new TagCodeGenerator();
            tagCodeGenerator.Generate();
        }
    }
}
