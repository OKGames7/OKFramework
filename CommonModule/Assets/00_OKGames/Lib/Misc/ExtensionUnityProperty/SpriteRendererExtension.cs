using UnityEngine;

namespace OKGamesLib {
    /// <summary>
    /// SpriteRendererコンポーネントの拡張クラス.
    /// </summary>
    public static class SpriteRendererExtension {

        /// <summary>
        /// SpriteRendererへアルファ値を設定する.
        /// </summary>
        /// <param name="renderer">アルファ設定させたいSpriteRenderer.</param>
        /// <param name="alpha">設定するアルファ値.</param>
        public static void SetAlpha(this SpriteRenderer renderer, float alpha) {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }
}
