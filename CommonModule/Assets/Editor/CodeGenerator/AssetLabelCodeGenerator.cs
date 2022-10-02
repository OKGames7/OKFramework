using System.Collections.Generic;
using System.Text;

namespace OKGamesLib {

    /// <summary>
    /// AddressablesのLabel値の定数を生成するGenerator.
    /// </summary>
    public class AssetLabelCodeGenerator : CodeGenerator {

        public virtual string AssetDirPath { get; set; } = "Assets/AddressableAssetsData/AssetGroups";

        public override string OutputFileName { get; set; } = "AssetLabel.gen.cs";

        public override string ClassName { get; set; } = "AssetLabel";

        protected override void WriteInner(StringBuilder builder) {
            var labelSet = new HashSet<string>();
            var assetGroups = EditorAddressablesUtility.LoadAssetGroups(AssetDirPath);
            foreach (var group in assetGroups) {
                if (group.ReadOnly) {
                    // ビルトインのリソース類は無視する.
                    continue;
                }

                foreach (var entry in group.entries) {

                    foreach (string label in entry.labels) {
                        labelSet.Add(label);
                    }
                }
            }

            AppendSymbols(builder, labelSet);
        }
    }
}
