using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// GameObjectに関する拡張メソッド.
    /// </summary>
    public static class GameObjectExtention {

        /// <summary>
        /// 指定したオブジェクトを親として自身の階層をセットする.
        /// </summary>
        /// <param name="obj">自身のオブジェクト.</param>
        /// <param name="parent">親オブジェクト.</param>
        public static void SetParent(this GameObject obj, Transform parent) {
            obj.transform.SetParent(parent);
        }

        /// <summary>
        /// 指定したオブジェクトを親として自身の階層をセットする.
        /// </summary>
        /// <param name="obj">自身のオブジェクト.</param>
        /// <param name="parent">親オブジェクト.</param>
        public static void SetParent(this GameObject obj, GameObject parent) {
            obj.transform.SetParent(parent.transform);
        }

        /// <summary>
        /// 指定したオブジェクトを親として自身の階層をセットする.
        /// セットする階層は親オブジェクトの中でも一番上(UI描画なら一番手前に表示される),
        /// </summary>
        /// <param name="obj">自身のオブジェクト.</param>
        /// <param name="parent">親オブジェクト.</param>
        public static void SetParentAsFirstSibling(this GameObject obj, GameObject parent) {
            obj.transform.SetParent(parent.transform, false);
            obj.transform.SetAsFirstSibling();
        }

        /// <summary>
        /// 指定したオブジェクトを親として自身の階層をセットする.
        /// セットする階層は親オブジェクトの中でも一番上(UI描画なら一番手前に表示される),
        /// </summary>
        /// <param name="obj">自身のオブジェクト.</param>
        /// <param name="parent">親オブジェクト.</param>
        public static void SetParentAsFirstSibling(this GameObject obj, Transform parent) {
            obj.transform.SetParent(parent, false);
            obj.transform.SetAsFirstSibling();
        }

        /// <summary>
        /// 指定したオブジェクトを親として自身の階層をセットする.
        /// セットする階層は親オブジェクトの中でも一番上(UI描画なら一番奥に表示される),
        /// </summary>
        /// <param name="obj">自身のオブジェクト.</param>
        /// <param name="parent">親オブジェクト.</param>
        public static void SetParentAsLastSibling(this GameObject obj, GameObject parent) {
            obj.transform.SetParent(parent.transform, false);
            obj.transform.SetAsLastSibling();
        }

        /// <summary>
        /// 指定したオブジェクトを親として自身の階層をセットする.
        /// セットする階層は親オブジェクトの中でも一番上(UI描画なら一番奥に表示される),
        /// </summary>
        /// <param name="obj">自身のオブジェクト.</param>
        /// <param name="parent">親オブジェクト.</param>
        public static void SetParentAsLastSibling(this GameObject obj, Transform parent) {
            obj.transform.SetParent(parent, false);
            obj.transform.SetAsLastSibling();
        }

        /// <summary>
        /// GameObjectを生成し親階層にセットする.
        /// </summary>
        /// <param name="obj">生成するオブジェクト.</param>
        /// <param name="parent">親階層.</param>
        public static GameObject Instantiate(this GameObject obj, Transform parent) {
            return GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity, parent);
        }

        /// <summary>
        /// GameObjectを生成し親階層にセットする.
        /// </summary>
        /// <param name="obj">生成するオブジェクト.</param>
        /// <param name="parent">親階層.</param>
        public static GameObject Instantiate(this GameObject obj, GameObject parent) {
            return GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity, parent.transform);
        }
    }
}
