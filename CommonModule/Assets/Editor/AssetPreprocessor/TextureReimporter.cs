using OKGamesFramework;
using UnityEditor;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// 既存のTextureAssetのImportの設定を変更する.
    /// </summary>
    public class TextureReimporter {

        /// <summary>
        /// iOSの圧縮形式.
        /// </summary>
        private const TextureImporterFormat iOSTextureFormat = TextureImporterFormat.ASTC_6x6;

        /// <summary>
        /// Androidの圧縮形式.
        /// </summary>
        private const TextureImporterFormat androidTextureFormat = TextureImporterFormat.ETC2_RGBA8;

        /// <summary>
        /// 走査スコープのフォルダ.
        /// </summary>
        private static readonly string[] _targetFolders = { "Assets" };

        /// <summary>
        /// 進捗バーを表示する際のタイトル.
        /// </summary>
        private const string ProgressBarTitle = "Update Texture Format";

        /// <summary>
        /// iOSのテクスチャ設定.
        /// </summary>
        /// <param name="textureImporter">textureのimporter.</param>
        /// <param name="assetPath">設定するアセットのパス.</param>
        public static void SetImportSettingsForIos(TextureImporter textureImporter, string assetPath) {
            int originalMaxSize = textureImporter.maxTextureSize;
            textureImporter.SetPlatformTextureSettings(new TextureImporterPlatformSettings {
                name = "iPhone",
                overridden = true,
                maxTextureSize = originalMaxSize,
                resizeAlgorithm = TextureResizeAlgorithm.Mitchell,
                format = iOSTextureFormat,
                textureCompression = TextureImporterCompression.Compressed,
                compressionQuality = 50,
            });
            Log.Notice($"{assetPath} [iOS] : Set {iOSTextureFormat.ToString()}");
        }

        /// <summary>
        /// Androidのテクスチャ設定.
        /// </summary>
        /// <param name="textureImporter">textureのimporter.</param>
        /// <param name="assetPath">設定するアセットのパス.</param>
        public static void SetImportSettingsForAndroid(TextureImporter textureImporter, string assetPath) {
            int originalMaxSize = textureImporter.maxTextureSize;
            textureImporter.SetPlatformTextureSettings(new TextureImporterPlatformSettings {
                name = "Android",
                overridden = true,
                maxTextureSize = originalMaxSize,
                resizeAlgorithm = TextureResizeAlgorithm.Mitchell,
                format = androidTextureFormat,
                textureCompression = TextureImporterCompression.Compressed,
                compressionQuality = 50,
            });
            Log.Notice($"{assetPath} [Android] : Set {iOSTextureFormat.ToString()}");
        }

        /// <summary>
        /// 既存のTextureアセットを取得し、Import設定を書き換える.
        /// </summary>
        [MenuItem("o.k.games/99.UnityEditor/Reimport/TextureAsset", false)]
        private static void UpdateTextureCompressionSettings() {
            string[] guids = AssetDatabase.FindAssets("t:texture2D", _targetFolders);

            try {
                EditorUtility.DisplayProgressBar(ProgressBarTitle, "", 0f);
                for (int i = 0; i < guids.Length; ++i) {
                    string guid = guids[i];
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    SetTextureSettings(assetPath, i, guids.Length);
                }

                EditorUtility.DisplayProgressBar(ProgressBarTitle, "Refresh AssetDatabase ...", 1f);
                AssetDatabase.Refresh();
                Log.Notice("Texture format conversion is completed.");
            } finally {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// 指定したアセットのImportの設定を書き換える.
        /// </summary>
        /// <param name="assetPath">指定アセットのパス.</param>
        /// <param name="count">進捗率を出す際の分子.</param>
        /// <param name="totalCount">新緑率を出す際の分母.</param>
        private static void SetTextureSettings(string assetPath, int count, int totalCount) {
            if (assetPath.EndsWith(".ttf")) {
                return;
            }

            if (assetPath.EndsWith(".otf")) {
                return;
            }

            var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (textureImporter == null) {
                Log.Error($"TextureImporter not found : {assetPath}");
                return;
            }

            if (!ShouldReimport(textureImporter)) {
                return;
            }

            float progress = (float)count / totalCount;
            EditorUtility.DisplayProgressBar(ProgressBarTitle, $"{assetPath} ({count} / {totalCount})", progress);

            SetImportSettingsForIos(textureImporter, assetPath);
            SetImportSettingsForAndroid(textureImporter, assetPath);
            AssetDatabase.ImportAsset(assetPath);
        }

        /// <summary>
        /// 任意のImporterの設定状態を見て、書き換えが必要かを判断する.
        /// </summary>
        /// <param name="textureImporter">書き換えが必要か検討するImporter.</param>
        /// <returns>書き換えが必要.</returns>
        private static bool ShouldReimport(TextureImporter textureImporter) {
            var iosSettinngs = textureImporter.GetPlatformTextureSettings("iPhone");
            if (iosSettinngs.format != iOSTextureFormat || iosSettinngs.overridden == false) {
                return true;
            }

            var androidSettings = textureImporter.GetPlatformTextureSettings("Android");
            if (androidSettings.format != androidTextureFormat || androidSettings.overridden == false) {
                return true;
            }

            return false;
        }
    }
}
