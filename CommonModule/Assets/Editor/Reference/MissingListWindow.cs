using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class AssetParameterData {
    public UnityEngine.Object obj { get; set; }            //!< アセットのObject自体
    public string path { get; set; }                    //!< アセットのパス
    public SerializedProperty property { get; set; }    //!< プロパティ
}

public class MissingListWindow : EditorWindow {
    private static string[] extensions = { ".prefab", ".mat", ".controller", ".cs", ".shader", ".mask", ".asset" };

    private static List<AssetParameterData> missingList = new List<AssetParameterData>();
    private Vector2 scrollPos;

    /// <summary>
    /// Missingがあるアセットを検索してそのリストを表示する
    /// </summary>
    [MenuItem("o.k.games/MissingList")]
    private static void ShowMissingList() {
        // Missingがあるアセットを検索
        Search();

        // ウィンドウを表示
        var window = GetWindow<MissingListWindow>();
        window.minSize = new Vector2(900, 300);
    }

    /// <summary>
    /// Missingがあるアセットを検索
    /// </summary>
    private static void Search() {
        // 全てのアセットのファイルパスを取得
        string[] allPaths = AssetDatabase.GetAllAssetPaths();
        int length = allPaths.Length;

        for (int i = 0; i < length; i++) {
            // プログレスバーを表示
            EditorUtility.DisplayProgressBar("Search Missing", string.Format("{0}/{1}", i + 1, length), (float)i / length);

            // Missing状態のプロパティを検索
            if (extensions.Contains(Path.GetExtension(allPaths[i]))) {
                SearchMissing(allPaths[i]);
            }
        }

        // プログレスバーを消す
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 指定アセットにMissingのプロパティがあれば、それをmissingListに追加する
    /// </summary>
    /// <param name="path">Path.</param>
    private static void SearchMissing(string path) {
        // 指定パスのアセットを全て取得
        IEnumerable<UnityEngine.Object> assets = AssetDatabase.LoadAllAssetsAtPath(path);

        // 各アセットについて、Missingのプロパティがあるかチェック
        foreach (UnityEngine.Object obj in assets) {
            if (obj == null) {
                continue;
            }
            if (obj.name == "Deprecated EditorExtensionImpl") {
                continue;
            }

            // SerializedObjectを通してアセットのプロパティを取得する
            SerializedObject sobj = new SerializedObject(obj);
            SerializedProperty property = sobj.GetIterator();

            while (property.Next(true)) {
                // プロパティの種類がオブジェクト（アセット）への参照で、
                // その参照がnullなのにもかかわらず、参照先インスタンスIDが0でないものはMissing状態！
                if (property.propertyType == SerializedPropertyType.ObjectReference &&
                    property.objectReferenceValue == null &&
                    property.objectReferenceInstanceIDValue != 0) {

                    // Missing状態のプロパティリストに追加する
                    missingList.Add(new AssetParameterData() {
                        obj = obj,
                        path = path,
                        property = property
                    });
                }
            }
        }
    }

    /// <summary>
    /// Missingのリストを表示
    /// </summary>
    private void OnGUI() {
        // 列見出し
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Asset", GUILayout.Width(200));
        EditorGUILayout.LabelField("Property", GUILayout.Width(200));
        EditorGUILayout.LabelField("Path");
        EditorGUILayout.EndHorizontal();

        // リスト表示
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (AssetParameterData data in missingList) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(data.obj, data.obj.GetType(), true, GUILayout.Width(200));
            EditorGUILayout.TextField(data.property.name, GUILayout.Width(200));
            EditorGUILayout.TextField(data.path);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }
}
