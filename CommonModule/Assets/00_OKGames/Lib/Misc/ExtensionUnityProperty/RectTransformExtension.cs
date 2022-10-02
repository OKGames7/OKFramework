using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// RectTransformに関する拡張メソッド.
    /// </summary>
    public static class RectTransformExtension {

        /// <summary>
        /// 各メソッド内で使用するVector2.
        /// </summary>
        private static Vector2 _vec2;

        private static Vector3 _vec3;


        /// <summary>
        /// Anchorの設定を行う.
        /// </summary>
        /// <param name="rect">設定対象となるRectTransform.</param>
        /// <param name="x">AnchorのX座標.</param>
        /// <param name="y">AnchorのY座標.</param>
        public static void SetAnchoredPosition(this RectTransform rect, float? x, float? y) {
            _vec2.Set(
                x ?? rect.anchoredPosition.x,
                y ?? rect.anchoredPosition.y
            );

            rect.anchoredPosition = _vec2;
        }

        /// <summary>
        /// Anchorの設定を行う.
        /// </summary>
        /// <param name="rect">設定対象となるRectTransform.</param>
        /// <param name="x">AnchorのX座標.</param>
        /// <param name="y">AnchorのY座標.</param>
        public static void AddAnchoredPosition(this RectTransform rect, float x, float y) {
            _vec2.Set(
                rect.anchoredPosition.x + x,
                rect.anchoredPosition.y + y
            );
            rect.anchoredPosition = _vec2;
        }

        /// <summary>
        /// Anchorの設定を行う.
        /// </summary>
        /// <param name="rect">設定対象となるRectTransform.</param>
        /// <param name="vec">anchorの座標.</param>
        public static void AddAnchoredPosition(this RectTransform rect, Vector2 vec) {
            rect.AddAnchoredPosition(vec.x, vec.y);
        }

        /// <summary>
        /// サイズの設定を行う.
        /// </summary>
        /// <param name="rect">設定対象となるRectTransform.</param>
        /// <param name="x">横幅.</param>
        /// <param name="y">高さ.</param>
        public static void SetSizeDelta(this RectTransform rect, float? x, float? y) {
            _vec2.Set(
                x ?? rect.sizeDelta.x,
                y ?? rect.sizeDelta.y
            );
            rect.sizeDelta = _vec2;
        }

        /// <summary>
        /// サイズの設定を行う.
        /// </summary>
        /// <param name="rect">設定対象となるRectTransform.</param>
        /// <param name="x">横幅.</param>
        /// <param name="y">高さ.</param>
        public static void AddSizeDelta(this RectTransform rect, float x, float y) {
            _vec2.Set(
                rect.sizeDelta.x + x,
                rect.sizeDelta.y + y
            );
            rect.sizeDelta = _vec2;
        }

        /// <summary>
        /// サイズの設定を行う.
        /// </summary>
        /// <param name="rect">設定対象となるRectTransform.</param>
        /// <param name="vec">横x縦の値.</param>
        public static void AddSizeDelta(this RectTransform rect, Vector2 vec) {
            rect.AddSizeDelta(vec.x, vec.y);
        }

        /// <summary>
        /// サイズの設定を行う.
        /// </summary>
        /// <param name="rect">設定対象となるRectTransform.</param>
        /// <param name="vec">横x縦の値.</param>
        public static void SetLocalScale(this RectTransform rect, float ratio) {
            _vec2.Set(
                ratio,
                ratio
            );
            rect.SetLocalScale(_vec2);
        }


        /// <summary>
        /// サイズの設定を行う.
        /// </summary>
        /// <param name="rect">設定対象となるRectTransform.</param>
        /// <param name="vec">横x縦の値.</param>
        public static void SetLocalScale(this RectTransform rect, float x, float y) {
            _vec2.Set(
                x,
                y
            );
            rect.SetLocalScale(_vec2);
        }

        /// <summary>
        /// サイズの設定を行う.
        /// </summary>
        /// <param name="rect">設定対象となるRectTransform.</param>
        /// <param name="vec">横x縦の値.</param>
        public static void SetLocalScale(this RectTransform rect, Vector2 vec) {
            _vec3.Set(
                vec.x,
                vec.y,
                1.0f
            );
            rect.localScale = _vec3;
        }
    }
}
