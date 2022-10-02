using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

namespace OKGamesLib {


    /// <summary>
    /// AddressablesのEditorに関するUtility.
    /// </summary>
    public class EditorAddressablesUtility {

        /// <summary>
        ///  AddressablesのGroupのリストを返す.
        /// </summary>
        /// <param name="dirPath">操作するパス.</param>
        /// <returns></returns>
        public static List<AddressableAssetGroup> LoadAssetGroups(string dirPath) {
            var assetGroups = new List<AddressableAssetGroup>();
            string[] filePathList = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories);

            foreach (string filePath in filePathList) {
                var assetGroup = AssetDatabase.LoadAssetAtPath<AddressableAssetGroup>(filePath);
                if (assetGroup != null) { assetGroups.Add(assetGroup); }
            }
            return assetGroups;
        }
    }
}
