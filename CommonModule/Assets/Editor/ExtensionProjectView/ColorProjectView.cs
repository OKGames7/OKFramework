using System.IO;
using UnityEditor;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// ProjectViewで作業しやすくするためのエディタ拡張.
    /// </summary>
    public static class ColorProjectView {

        /// <summary>
        /// メニューに出す時の表示.
        /// </summary>
        private const string _menuPath = "o.k.games/99.UnityEditor/ColoredProjectView";

        /// <summary>
        /// 色付けするフォルダのキーワード.
        /// 各色は<see cref="GetColorForDarkSkin"/>関数内コメントを参照のこと.
        /// </summary>
        private static readonly string[] _keywords = {
            "scene", "material", "editor", "resource", "prefab", "shader", "script", "texture", "mesh"
        };

        /// <summary>
        /// メニューを押した時の処理.
        /// </summary>
        [MenuItem(_menuPath)]
        private static void ToggleEnable() {
            Menu.SetChecked(_menuPath, !Menu.GetChecked(_menuPath));
        }

        /// <summary>
        /// UnityEditorを起動した直後に呼ばれる処理.
        /// ProjectViewの各データをOnGUIで拡張する.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void SetEvent() {
            EditorApplication.projectWindowItemOnGUI += OnGUI;
        }

        /// <summary>
        /// GUI表示.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="selectionRect"></param>
        private static void OnGUI(string guid, Rect selectionRect) {
            if (!Menu.GetChecked(_menuPath)) {
                return;
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var pathLevel = CountWord(assetPath, "/");

            var originalColor = GUI.color;
            GUI.color = GetColor(pathLevel, assetPath);
            GUI.Box(selectionRect, string.Empty);
            GUI.color = originalColor;
        }

        /// <summary>
        ///　アセットのパスから階層の深さを取得する.
        /// </summary>
        /// <param name="source">アセットパス.</param>
        /// <param name="word">'/'を想定.</param>
        /// <returns>そのアセットの階層数.</returns>
        private static int CountWord(string source, string word) {
            return source.Length - source.Replace(word, "").Length;
        }

        /// <summary>
        /// pathLevelとassetPathから色を取得する.
        /// </summary>
        /// <param name="pathLevel">該当アセットのパスの階層の深さ.</param>
        /// <param name="assetPath">該当アセットのパス<./param>
        /// <returns>Color</returns>
        private static Color GetColor(int pathLevel, string assetPath) {
            var fileName = Path.GetFileName(assetPath);
            string[] folderNames = assetPath.Split('/');

            int id, depth;
            (id, depth) = GetColorIdAndDepth(pathLevel, assetPath);

            Color color = (EditorGUIUtility.isProSkin)
                ? GetColorForDarkSkin(id)
                : GetColorForLightSkin(id);

            float alphaFactor = 1.0f - (depth * 0.25f);
            alphaFactor = Mathf.Clamp(alphaFactor, 0, 1f);
            // 階層が深くなればなるほど薄くする.
            color.a *= alphaFactor;
            return color;
        }

        /// <summary>
        /// Colorを決めるためのidとdepth情報(カラーのアルファにかける係数)を取得する.
        /// </summary>
        /// <param name="pathLevel">該当アセットのパスの階層の深さ.</param>
        /// <param name="assetPath">該当アセットのパス<./param>
        /// <returns>id, depth</returns>
        private static (int id, int depth) GetColorIdAndDepth(int pathLevel, string assetPath) {
            if (pathLevel == 1) {
                return (0, 0);
            }

            int depthBase = 0;
            string[] foloderNames = assetPath.Split('/');
            foreach (string folderName in foloderNames) {
                var lowerName = folderName.ToLower();
                for (int i = 0; i < _keywords.Length; ++i) {
                    if (lowerName.StartsWith(_keywords[i])) {
                        return ((i % _keywords.Length) + 1, pathLevel - depthBase);
                    }
                }
                ++depthBase;
            }
            return (-1, 0);
        }

        /// <summary>
        /// プロモード用のスキンに合わせたカラー情報を取得.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static Color GetColorForDarkSkin(int id) {
            switch (id % 10) {
                case 0: return new Color(8.4f, 8.4f, 0.0f, 0.45f); // 黄色, Assets直下の色.
                case 1: return new Color(9.6f, 3.5f, 0.0f, 0.50f); // オレンジ, scene
                case 2: return new Color(0.0f, 9.6f, 0.0f, 0.40f); // 緑 , material
                case 3: return new Color(1.8f, 4.9f, 7.4f, 0.40f); // 濃い青 , editor
                case 4: return new Color(9.6f, 0.5f, 0.5f, 0.50f); // 赤, resource
                case 5: return new Color(8.9f, 1.4f, 4.4f, 0.35f); // 赤紫, prefab
                case 6: return new Color(8.4f, 0.0f, 8.4f, 0.50f); // 紫, shader
                case 7: return new Color(0.0f, 4.8f, 9.6f, 0.40f); //　青, script
                case 8: return new Color(4.0f, 8.9f, 1.8f, 0.40f); // 深緑, texture
                case 9: return new Color(9.6f, 3.0f, 3.0f, 0.50f); // 茶色, mesh
            }
            return new Color(0, 0, 0, 0); // どこにも属さないものは黒.
        }

        /// <summary>
        /// Personalモード用のスキンに合わせたカラー情報を取得.
        /// 色は適当なので使うなら調整すること.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static Color GetColorForLightSkin(int id) {
            switch (id % 10) {
                case 2: return new Color(1.4f, 1.4f, 0.0f, 0.15f);
                case 1: return new Color(1.4f, 1.4f, 0.0f, 0.11f);
                case 0: return new Color(1.6f, 0.0f, 0.0f, 0.11f);
                case 3: return new Color(0.0f, 1.6f, 0.0f, 0.11f);
                case 4: return new Color(1.6f, 0.0f, 0.0f, 0.15f);
                case 5: return new Color(0.0f, 1.6f, 0.0f, 0.15f);
                case 6: return new Color(0.8f, 0.0f, 1.4f, 0.15f);
                case 7: return new Color(1.6f, 0.5f, 0.0f, 0.15f);
                case 8: return new Color(0.0f, 0.8f, 1.6f, 0.15f);
                case 9: return new Color(1.6f, 0.4f, 0.4f, 0.15f);
            }
            return new Color(0, 0, 0, 0);
        }
    }
}
