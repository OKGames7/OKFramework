using UnityEngine;
using UnityEngine.UI;

namespace OKGamesLib {
    /// <summary>
    /// Graphicコンポーネントの拡張クラス.
    /// </summary>
    public static class GraphicExtension {

        /// <summary>
        /// Graphicへアルファ値を設定する.
        /// </summary>
        /// <typeparam name="T">Graphic型を継承しているクラス(コンポーネント).</typeparam>
        /// <param name="graphic">アルファ設定させたいgraphic.</param>
        /// <param name="alpha">設定するアルファ値.</param>
        public static void SetAlpha<T>(this T graphic, float alpha) where T : Graphic {
            Color color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }
}
