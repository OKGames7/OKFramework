using OKGamesFramework;
using UnityEditor;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// PJ内にテクスチャアセットを追加した際に
    /// 自動でiOS, Adnroid向けにテクスチャ設定を書き換える.
    /// </summary>
    public class TexturePreprocess : AssetPostprocessor {

        /// <summary>
        /// TextureのAssetがProjectViewへ追加された時に走る処理.
        /// </summary>
        private void OnPreprocessTexture() {
            var textureImporter = (TextureImporter)assetImporter;
            if (!textureImporter.importSettingsMissing) {
                // 初回Importの検知を↑でおこなっている.
                // 初回だとimportSettingsMissingはtrueになる.
                return;
            }

            Log.Notice($"New texture is detected : {assetPath}");
            if (textureImporter == null) {
                Log.Error($"TextureImporter not found : {assetPath}");
                return;
            }

            TextureReimporter.SetImportSettingsForIos(textureImporter, assetPath);
            TextureReimporter.SetImportSettingsForAndroid(textureImporter, assetPath);
        }
    }
}
