using System.Collections.Generic;
using System.Text;
using UnityEditorInternal;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// LayerMaskの定数を生成するGenerator.
    /// </summary>
    public class LayerMaskCodeGenerator : CodeGenerator {
        public override string OutputFileName { get; set; } = "LayerMasks.gen.cs";
        public override string ClassName { get; set; } = "LayerMasks";

        protected override void WriteInner(StringBuilder builder) {
            var layers = InternalEditorUtility.layers;
            var layerToMask = new Dictionary<string, int>();

            foreach (string layer in layers) {
                int layerMask = 1 << UnityEngine.LayerMask.NameToLayer(layer);
                layerToMask.Add(layer, layerMask);
            }

            AppendSymbols(builder, layerToMask);
        }
    }
}
