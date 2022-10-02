using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// Transformに関する拡張メソッド.
    /// </summary>
    public static class TransformExtension {

        /// <summary>
        /// 各メソッド内で使用するVector3.
        /// </summary>
        private static Vector3 _vec;

        /// <summary>
        /// ワールド座標で位置を設定する.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="x">x座標.</param>
        /// <param name="y">y座標.</param>
        /// <param name="z">z座標.</param>
        public static void SetPosition(this Transform transform, float? x, float? y, float? z) {
            _vec.Set(
                x ?? transform.position.x,
                y ?? transform.position.y,
                z ?? transform.position.z
            );
            transform.position = _vec;
        }

        /// <summary>
        /// ワールド座標で現在位置からAddする.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="x">加えるx値.</param>
        /// <param name="y">加えるy値.</param>
        /// <param name="z">加えるz値.</param>
        public static void AddPosition(this Transform transform, float x, float y, float z) {
            _vec.Set(
                transform.position.x + x,
                transform.position.y + y,
                transform.position.z + z
            );
            transform.position = _vec;
        }

        /// <summary>
        /// ワールド座標で現在位置からAddする.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="vec">加えるvector3の値.</param>
        public static void AddPosition(this Transform transform, Vector3 vec) {
            transform.AddPosition(vec.x, vec.y, vec.z);
        }

        /// <summary>
        /// ローカル座標で位置を設定する.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="x">x座標.</param>
        /// <param name="y">y座標.</param>
        /// <param name="z">z座標.</param>
        public static void SetLocalPosition(this Transform transform, float? x, float? y, float? z) {
            _vec.Set(
                x ?? transform.localPosition.x,
                y ?? transform.localPosition.y,
                z ?? transform.localPosition.z
            );
            transform.localPosition = _vec;
        }

        /// <summary>
        /// ローカル座標で現在位置からAddする.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="x">加えるx値.</param>
        /// <param name="y">加えるy値.</param>
        /// <param name="z">加えるz値.</param>
        public static void AddLocalPosition(this Transform transform, float x, float y, float z) {
            _vec.Set(
                transform.localPosition.x + x,
                transform.localPosition.y + y,
                transform.localPosition.z + z
            );
            transform.localPosition = _vec;
        }

        /// <summary>
        /// ローカル座標で現在位置からAddする.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="vec">加えるvector3の値.</param>
        public static void AddLocalPosition(this Transform transform, Vector3 vec) {
            transform.AddLocalPosition(vec.x, vec.y, vec.z);
        }

        /// <summary>
        /// ローカルスケールを設定する.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="x">xのサイズ.</param>
        /// <param name="y">yのサイズ.</param>
        /// <param name="z">zのサイズ.</param>
        public static void SetLocalScale(this Transform transform, float? x, float? y, float? z) {
            _vec.Set(
                x ?? transform.localScale.x,
                y ?? transform.localScale.y,
                z ?? transform.localScale.z
            );
            transform.localScale = _vec;
        }

        /// <summary>
        /// ローカルスケールを現在のサイズからAddする.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="x">加えるx値.</param>
        /// <param name="y">加えるy値.</param>
        /// <param name="z">加えるz値.</param>
        public static void AddLocalScale(this Transform transform, float x, float y, float z) {
            _vec.Set(
                transform.localScale.x + x,
                transform.localScale.y + y,
                transform.localScale.z + z
            );
            transform.localScale = _vec;
        }

        /// <summary>
        /// ローカルスケールを現在のサイズからAddする.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="vec">加えるVector3の値.</param>
        public static void AddLocalScale(this Transform transform, Vector3 vec) {
            transform.AddLocalScale(vec.x, vec.y, vec.z);
        }

        /// <summary>
        /// ワールドの角度を設定する.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="x">xの回転</param>
        /// <param name="y">yの回転.</param>
        /// <param name="z">zの回転.</param>
        public static void SetEulterAngles(this Transform transform, float? x, float? y, float? z) {
            _vec.Set(
                x ?? transform.eulerAngles.x,
                y ?? transform.eulerAngles.y,
                z ?? transform.eulerAngles.z
            );
            transform.eulerAngles = _vec;
        }

        /// <summary>
        /// ワールドの角度を現在の角度からAddする.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="x">加算させるxの回転値.</param>
        /// <param name="y">加算させるyの回転値.</param>
        /// <param name="z">加算させるzの回転値.</param>
        public static void AddEulerAngles(this Transform transform, float x, float y, float z) {
            _vec.Set(
                transform.eulerAngles.x + x,
                transform.eulerAngles.y + y,
                transform.eulerAngles.z + z
            );
            transform.eulerAngles = _vec;
        }

        /// <summary>
        /// ワールドの角度を現在の角度からAddする.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="vec">加算させる回転値.</param>
        public static void AddEulerAngles(this Transform transform, Vector3 vec) {
            transform.AddEulerAngles(vec.x, vec.y, vec.z);
        }

        /// <summary>
        /// ローカルの角度を設定する.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="x">xの回転</param>
        /// <param name="y">yの回転.</param>
        /// <param name="z">zの回転.</param>
        public static void SetLocalEulterAngles(this Transform transform, float? x, float? y, float? z) {
            _vec.Set(
                x ?? transform.localEulerAngles.x,
                y ?? transform.localEulerAngles.y,
                z ?? transform.localEulerAngles.z
            );
            transform.localEulerAngles = _vec;
        }

        /// <summary>
        /// ローカルの角度を現在の角度からAddする.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="x">加算させるxの回転値.</param>
        /// <param name="y">加算させるyの回転値.</param>
        /// <param name="z">加算させるzの回転値.</param>
        public static void AddLocalEulerAngles(this Transform transform, float x, float y, float z) {
            _vec.Set(
                transform.localEulerAngles.x + x,
                transform.localEulerAngles.y + y,
                transform.localEulerAngles.z + z
            );
            transform.localEulerAngles = _vec;
        }

        /// <summary>
        /// ローカルの角度を現在の角度からAddする.
        /// </summary>
        /// <param name="transform">設定するTransform.</param>
        /// <param name="vec">加算させる回転値.</param>
        public static void AddLocalEulerAngles(this Transform transform, Vector3 vec) {
            transform.AddLocalEulerAngles(vec.x, vec.y, vec.z);
        }
    }
}
