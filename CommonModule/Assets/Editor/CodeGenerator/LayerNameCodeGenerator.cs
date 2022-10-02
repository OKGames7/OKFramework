using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEditorInternal;

namespace OKGamesLib {

    /// <summary>
    /// Layer名の定数を生成するGenerator.
    /// </summary>
    public class LayerNameCodeGenerator : CodeGenerator {
        public override string OutputFileName { get; set; } = "LayerName.gen.cs";
        public override string ClassName { get; set; } = "LayerName";

        protected override void WriteInner(StringBuilder builder) {
            var layers = InternalEditorUtility.layers;
            var labelSet = new HashSet<string>(layers);
            // var labelSet = new HashSet<string>(layers);
            AppendSymbols(builder, labelSet);
        }
    }
}
