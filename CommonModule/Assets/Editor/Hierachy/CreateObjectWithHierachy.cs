using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace OKGamesLib {

    /// <summary>
    /// カスタムで作成したPrefabをHierarchy上に生成する.
    /// </summary>
    public class CreateObjectWithHierachy {

        // アセットパス変数.
        private static readonly string _commonFormat = "Assets/00_OKGames/Lib/Prefabs/{0}";
        private static readonly string _textObjectPath = string.Format(_commonFormat, "CustomeText.prefab");
        private static readonly string _buttonObjectPath = string.Format(_commonFormat, "CustomeButton.prefab");


        /// <summary>
        /// CustomeTextオブジェクトの生成.
        /// priorityは高くして項目の一番下にでるようにしている.
        /// </summary>
        [MenuItem("GameObject/OKGames/Text", priority = 999)]
        private static void CreateText() {
            GenerateAsset(_textObjectPath);
        }

        /// <summary>
        /// CustomeButtonオブジェクトの生成.
        /// priorityはくして項目の一番下にでるようにしている.
        /// </summary>
        [MenuItem("GameObject/OKGames/Button", priority = 999)]
        private static void CreateButton() {
            GenerateAsset(_buttonObjectPath);
        }

        /// <summary>
        /// アセットを生成する.
        /// </summary>
        /// <param name="menuCommand">コマンドメニュー.</param>
        /// <param name="asettPath">生成するアセットのパス.</param>
        private static void GenerateAsset(string asettPath) {
            // アセットの取得.
            var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(asettPath);
            // アセットの生成.
            var generatedObject = PrefabUtility.InstantiatePrefab(gameObject) as GameObject;

            // Createボタンを押すときに選択していたオブジェクトを親オブジェクトとしてその子に生成したオブジェクトを移動させる.
            var parent = Selection.activeGameObject;
            GameObjectUtility.SetParentAndAlign(generatedObject, parent);

            // 今回のオブジェクト生成をなかったことにできるようにUndoできるようにする.
            string name = Path.GetFileNameWithoutExtension(asettPath);
            Undo.RegisterCreatedObjectUndo(generatedObject, name);

            // 生成したオブジェクトを選択している状態にする.
            Selection.activeObject = generatedObject;
        }
    }
}
