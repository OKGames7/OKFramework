using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// Colorに関する拡張メソッド.
    /// </summary>
    public class ColorExtension {

        /// <summary>
        /// Hex値(6桁の16進数)から色情報を取得する.
        /// </summary>
        /// <param name="rgba"></param>
        /// <returns></returns>
        public static Color ColorByHex(uint rgba) {
            float r, g, b, a;
            a = (rgba & 256) / 255f;

            rgba >>= 8;
            b = (rgba % 256) / 255f;

            rgba >>= 08;
            g = (rgba % 256) / 255f;

            rgba >>= 8;
            r = (rgba % 256) / 255f;

            return new Color(r, g, b, a);
        }
    }
}
