using OKGamesFramework;
using System.Collections.Generic;
using System.Text;

namespace OKGamesLib {

    /// <summary>
    /// AddressablesのAddress値の定数を生成するGenerator.
    /// </summary>
    public class AssetAddressCodeGenerator : CodeGenerator {

        public virtual string AssetDirPath { get; set; } = "Assets/AddressableAssetsData/AssetGroups";

        public override string OutputFileName { get; set; } = "AssetAddress.gen.cs";
        public override string ClassName { get; set; } = "AssetAddress";

        protected override void WriteInner(StringBuilder builder) {

            var addressSet = new HashSet<string>();
            var assetGroups = EditorAddressablesUtility.LoadAssetGroups(AssetDirPath);
            Log.DumpList(assetGroups);
            foreach (var group in assetGroups) {
                if (group.ReadOnly) {
                    continue;
                }

                foreach (var entry in group.entries) {
                    bool isNew = addressSet.Add(entry.address);
                    if (!isNew) {
                        Log.Warning($"Duplicated address found : {entry.address}");
                    }
                }
            }
            AppendSymbols(builder, addressSet);
        }
    }
}
