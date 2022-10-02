using OKGamesFramework;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace OKGamesLib {

    // ---------------------------------------------------------
    // Addressableに関するエディタ拡張.
    // ---------------------------------------------------------
    public class AddressableAssetPostprocessor : AssetPostprocessor {

        // AddressableAssetのアセットを配置する用のフォルダまでの階層.
        // (このフォルダ以下のデータに差分が出た際は、それをトリガーに、アセットの登録、再登録、登録解除など処理させる)
        private const string _addressableResources = "Assets/Addressable/Asset";

        /// <summary>
        /// AssetPostprocessorの機能で、Assets内のアセットに差分が出たときに実行される処理.
        /// _addressableResources以下のAssetを対象にAddressableのsettings上の登録、再登録、移動、削除などを自動化する.
        /// </summary>
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssets) {
            // 使用しているAddressableSettingsを取得する.
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            // 追加アセットの自動登録.
            ImportAddressableAssets(settings, importedAssets);
            // 削除アセットの登録自動削除
            DeleteAddressableAssets(settings, deletedAssets);
            // 移動後アセットの登録と移動前アセットの登録削除.
            MoveAddressableAssets(settings, movedAssets, movedFromAssets);

        }

        /// <summary>
        /// 追加されたAddressable用のアセットをAddressable Settingsへ登録する.
        /// グループのフォルダ以下のアセットにはアドレスがデフォルトで登録されないため、フォルダ登録はせず、フォルダ以下のオブジェクトはグループのrootで登録する.
        /// </summary>
        private static void ImportAddressableAssets(AddressableAssetSettings settings, string[] assets) {
            bool isRegisterd = false;

            foreach (string asset in assets) {

                (bool valied, string assetPath, int assetPathIndex) = Validation(asset);
                if (!valied) {
                    continue;
                }

                string groupName = assetPath.Substring(0, assetPathIndex);
                string assetName = assetPath.Remove(0, assetPathIndex + 1);
                AddressableAssetGroup group = settings.FindGroup(groupName);
                if (group == null) {
                    // まだないグループの場合は新規作成.
                    group = CreateAddressableGroup(settings, groupName);
                }

                if (group == null) {
                    //エラーハンドリング.
                    Log.Error($"Addressableのグループ作成に失敗しました: {groupName}");
                    continue;
                }

                // Addressableにエントリーしたアセットにパスの登録を行う.
                AddressableAssetEntry entry = RegisterAssetPath(settings, group, asset);
                // エントリーへラベルを登録する.
                RegisterAssetLabel(entry, Path.GetDirectoryName(assetPath), assetName);

                isRegisterd = true;
            }

            if (isRegisterd) {
                // 一つでも登録したものがあれば内容反映させる.
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// 削除されたAddressable用のアセットをAddressable Settingsから削除する.
        /// フォルダ削除時については、元々AddressableSettings上へフォルダを登録していないので何も対応しなくてOK.
        /// </summary>

        private static void DeleteAddressableAssets(AddressableAssetSettings settings, string[] assets) {
            bool isDeleted = false;

            foreach (string asset in assets) {

                (bool valied, _, _) = Validation(asset);
                if (!valied) {
                    continue;
                }

                string guid = AssetDatabase.AssetPathToGUID(asset);
                if (string.IsNullOrEmpty(guid)) {
                    continue;
                }

                settings.RemoveAssetEntry(guid);
                isDeleted = true;
            }

            if (isDeleted) {
                // 一つでも削除したものがあれば内容反映させる.
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// 移動されたAddressable用のアセットについて、Addressable Settingsへ新規エントリ登録と古い分のエントリーを削除する.
        /// </summary>
        private static void MoveAddressableAssets(AddressableAssetSettings settings, string[] movedAssets, string[] movedFromAssets) {
            // 移動前の登録分を削除.
            RemovePastAddressableAssets(settings, movedAssets, movedFromAssets);
            // 移動後の分を登録.
            ImportAddressableAssets(settings, movedAssets);
        }

        /// <summary>
        /// バリデーション処理.
        /// </summary>
        private static (bool, string, int) Validation(string asset, bool isCheckFolder = true) {
            bool valid = false;
            string assetPath = "";
            int assetPathIndex = -1;

            if (!File.Exists(asset)) {
                // 存在していないアセットの場合.
                return (valid, "", -1);
            }

            int forlderNameIndex = asset.IndexOf(_addressableResources);
            if (forlderNameIndex < 0) {
                // Addressable以下以外のアセットの場合.
                return (valid, "", -1);
            }

            // 変更データがフォルダかどうかを検知して、フォルダだったら処理の対象外とする場合.
            assetPath = asset.Remove(0, forlderNameIndex + _addressableResources.Length + 1);
            assetPathIndex = assetPath.IndexOf("/");
            if (isCheckFolder) {
                if (assetPathIndex < 0) {
                    return (valid, "", -1);
                }
            }

            valid = true;
            return (valid, assetPath, assetPathIndex);
        }

        /// <summary>
        /// 移動されたAddressable用のアセットについて、Addressable Settingsへ新規エントリ登録と古い分のエントリーを削除する.
        /// </summary>
        private static void RemovePastAddressableAssets(AddressableAssetSettings settings, string[] movedAssets, string[] movedFromAssets) {
            bool isRemoved = false;
            for (int i = 0; i < movedFromAssets.Length; ++i) {
                // ここのバリデーションは特殊なので個別に書く.
                if (!File.Exists(movedAssets[i])) {
                    continue;
                }
                if (movedFromAssets[i].IndexOf(_addressableResources) < 0) {
                    // 移動前のアセットがAddressableフォルダ以下にないものだった場合はSkip
                    continue;
                }

                string guid = AssetDatabase.AssetPathToGUID(movedAssets[i]);
                if (string.IsNullOrEmpty(guid)) {
                    // guidが取得できなかった場合は処理しない.
                    continue;
                }

                // AddressableSettingsのエントリーから削除.
                settings.RemoveAssetEntry(guid);
                isRemoved = true;
            }
            if (isRemoved) {
                // 一つでも削除したものがあれば内容反映させる.
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Addressable Settingsへ新規グループを登録する.
        /// </summary>
        private static AddressableAssetGroup CreateAddressableGroup(AddressableAssetSettings settings, string groupName) {
            var groupTemplate = settings.GetGroupTemplateObject(0) as AddressableAssetGroupTemplate;
            AddressableAssetGroup group = settings.CreateGroup(groupName, false, false, true, null, groupTemplate.GetTypes());
            groupTemplate.ApplyToAddressableAssetGroup(group);
            return group;
        }

        /// <summary>
        /// Addressable Settingsのグループへエントリーを新規登録/移動する.
        /// </summary>
        private static AddressableAssetEntry RegisterAssetPath(AddressableAssetSettings settings, AddressableAssetGroup group, string asset) {
            string guid = AssetDatabase.AssetPathToGUID(asset);
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
            return entry;
        }

        /// <summary>
        /// Addressable Settingsのグループ内エントリーへラベルを上書き/新規登録する.
        /// </summary>
        private static void RegisterAssetLabel(AddressableAssetEntry entry, string assetDirectoryName, string assetName) {
            string label = assetDirectoryName.Replace('\\', '/');

            if (string.IsNullOrEmpty(label)) {
                return;
            }

            entry.SetAddress(Path.GetFileNameWithoutExtension(assetName));
            // 第二引数がtrueで追加、falseで削除.
            // 第三引数がtrueでラベルがなかった場合に自動的に作られる.
            entry.SetLabel(label, true, true);
        }
    }
}
