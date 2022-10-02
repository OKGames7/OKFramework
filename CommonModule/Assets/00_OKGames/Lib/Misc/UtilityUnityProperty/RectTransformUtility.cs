using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// RectTransformに関する便利クラス.
    /// </summary>
    public class RectTransformUtility {

        /// <summary>
        /// ピボットの位置を設定する.
        /// </summary>
        /// <param name="rect">変更させたいRectTransform.</param>
        /// <param name="pivot">変更後のpivotの位置.</param>
        public static void SetPivot(ref RectTransform rect, Vector2 pivot) {
            Vector2 size = rect.rect.size;
            Vector2 deltaPivot = rect.pivot - pivot;
            Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rect.pivot = pivot;
            rect.localPosition -= deltaPosition;
        }
    }
}
