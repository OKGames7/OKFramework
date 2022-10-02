using System.Collections.Generic;
using System.Text;
using UnityEditorInternal;

namespace OKGamesLib {

    /// <summary>
    /// Tagの定数を生成するGenerator.
    /// </summary>
    public class TagCodeGenerator : CodeGenerator {
        public override string OutputFileName { get; set; } = "Tag.gen.cs";
        public override string ClassName { get; set; } = "Tag";

        protected override void WriteInner(StringBuilder builder) {
            var tags = InternalEditorUtility.tags;
            var labelSet = new HashSet<string>(tags);
            AppendSymbols(builder, labelSet);
        }
    }
}
