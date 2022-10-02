using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// UnityEngine.Debugには標準だとDrawLineとDrawRayしかないので、四角形や楕円の描画を拡張している.
    /// </summary>
    public class DebugDrawExtension {

        /// <summary>
        /// Vector3の指定した位置の真ん中を基準に指定サイズのRectを描画する.
        /// </summary>
        /// <param name="pos">描画の中心位置.</param>
        /// <param name="size">描画するRectのサイズ.</param>
        /// <param name="color">描画するRectの色.</param>
        /// <param name="duration">ラインの描画が完了する時間</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRect(Vector3 pos, float size, Color color, float duration = 0f) {
            float x1 = pos.x - size / 2;
            float x2 = pos.x + size / 2;
            float y1 = pos.y - size / 2;
            float y2 = pos.y - size / 2;
            Debug.DrawLine(new Vector2(x1, y1), new Vector2(x2, y1), color, duration);
            Debug.DrawLine(new Vector2(x2, y1), new Vector2(x2, y2), color, duration);
            Debug.DrawLine(new Vector2(x2, y2), new Vector2(x1, y2), color, duration);
            Debug.DrawLine(new Vector2(x1, y2), new Vector2(x1, y1), color, duration);
        }

        /// <summary>
        /// 四角形の頂点を渡してRectを描画する.
        /// </summary>
        /// <param name="cornerPoints">描画する頂点の情報.</param>
        /// <param name="color">描画するRectの色.</param>
        /// <param name="duration">ラインの描画が完了する時間</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRect(Vector3[] cornerPoints, Color color, float duration = 0f) {
            Debug.DrawLine(cornerPoints[0], cornerPoints[1], color, duration);
            Debug.DrawLine(cornerPoints[1], cornerPoints[2], color, duration);
            Debug.DrawLine(cornerPoints[2], cornerPoints[3], color, duration);
            Debug.DrawLine(cornerPoints[3], cornerPoints[0], color, duration);
        }

        /// <summary>
        /// 四角形（AABB)の描画。左上と右下の２頂点を渡してRectを描画する.
        /// </summary>
        /// <param name="leftTop">左上頂点の座標.</param>
        /// <param name="rightBottom">右下頂点の座標.</param>
        /// <param name="color">描画する色.</param>
        /// <param name="duration">ラインの描画が完了する時間</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRect(Vector3 leftTop, Vector3 rightBottom, Color color, float duration = 0f) {
            float x1 = leftTop.x;
            float x2 = rightBottom.x;
            float y1 = leftTop.y;
            float y2 = rightBottom.y;
            Debug.DrawLine(new Vector2(x1, y1), new Vector2(x2, y1), color, duration);
            Debug.DrawLine(new Vector2(x2, y1), new Vector2(x2, y2), color, duration);
            Debug.DrawLine(new Vector2(x2, y2), new Vector2(x1, y2), color, duration);
            Debug.DrawLine(new Vector2(x1, y2), new Vector2(x1, y1), color, duration);
        }

        /// <summary>
        ///　12角形の立体を描画する.
        /// </summary>
        /// <param name="pos">描画する図形の中心点.</param>
        /// <param name="size">描画する図形のサイズ.</param>
        /// <param name="color">描画する図形の色.</param>
        /// <param name="duration">図形を描画するのにかける時間.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawDiamond3D(Vector3 pos, float size, Color color, float duration = 0f) {
            float d = size / 2;
            Vector3[] posList = new Vector3[4];

            posList[0] = pos; posList[0].x -= d;
            posList[1] = pos; posList[1].y -= d;
            posList[2] = pos; posList[2].x += d;
            posList[3] = pos; posList[3].y += d;
            DrawRect(posList, color, duration);

            posList[0] = pos; posList[0].y -= d;
            posList[1] = pos; posList[1].z -= d;
            posList[2] = pos; posList[2].y += d;
            posList[3] = pos; posList[3].z += d;
            DrawRect(posList, color, duration);

            posList[0] = pos; posList[0].z -= d;
            posList[1] = pos; posList[1].x -= d;
            posList[2] = pos; posList[2].z += d;
            posList[3] = pos; posList[3].x += d;
            DrawRect(posList, color, duration);
        }

        /// <summary>
        /// 立方体を描画する.
        /// </summary>
        /// <param name="pos">描画する図形の中心点.</param>
        /// <param name="size">描画する図形のサイズ.</param>
        /// <param name="color">描画する図形の色.</param>
        /// <param name="duration">図形を描画するのにかける時間.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCube(Vector3 pos, Vector3 size, Color color, float duration = 0f) {
            Vector3 s = size / 2;
            Vector3[] topRect = new Vector3[4];
            Vector3[] bottomRect = new Vector3[4];

            topRect[0] = GetPos(pos, -s.x, s.y, -s.z);
            topRect[1] = GetPos(pos, s.x, s.y, -s.z);
            topRect[2] = GetPos(pos, s.x, s.y, s.z);
            topRect[3] = GetPos(pos, -s.x, s.y, s.z);
            DrawRect(topRect, color, duration);

            bottomRect[0] = GetPos(pos, -s.x, -s.y, -s.z);
            bottomRect[1] = GetPos(pos, s.x, -s.y, -s.z);
            bottomRect[2] = GetPos(pos, s.x, -s.y, s.z);
            bottomRect[3] = GetPos(pos, -s.x, -s.y, s.z);
            DrawRect(bottomRect, color, duration);

            Debug.DrawLine(topRect[0], bottomRect[0], color, duration);
            Debug.DrawLine(topRect[1], bottomRect[1], color, duration);
            Debug.DrawLine(topRect[2], bottomRect[2], color, duration);
            Debug.DrawLine(topRect[3], bottomRect[3], color, duration);
        }

        /// <summary>
        /// ベースのポジションから指定した座標分を加算したVector3を取得する.
        /// </summary>
        /// <param name="basePos">基準となる座標.</param>
        /// <param name="dx">加算するx値.</param>
        /// <param name="dy">加算するy値.</param>
        /// <param name="dz">加算するz値.</param>
        /// <returns></returns>
        private static Vector3 GetPos(Vector3 basePos, float dx, float dy, float dz) {
            return new Vector3(
                basePos.x + dx,
                basePos.y + dy,
                basePos.z + dz
            );
        }

        /// <summary>
        /// 立体の円を描画する.
        /// </summary>
        /// <param name="pos">図形の中心座標.</param>
        /// <param name="radius">円の半径.</param>
        /// <param name="color">図形の色.</param>
        /// <param name="duration">図形を描画するのにかける時間.</param>
        /// <param name="segments">孤の分割数.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCircle(
            Vector3 pos, float radius, Color color, float duration = 0f, int segments = 16) {
            DrawEllipse(pos, Vector3.forward, Vector3.up, radius, radius, segments, color, duration);
        }

        /// <summary>
        /// 楕円を描画する.
        /// </summary>
        /// <param name="pos">図形の中心座標.</param>
        /// <param name="forward">前の向き.</param>
        /// <param name="up">上の向き.</param>
        /// <param name="radiusx">円の半径.</param>
        /// <param name="radiusY">円の半径.</param>
        /// <param name="segments">孤の分割数.</param>
        /// <param name="color">図形の色</param>
        /// <param name="duration">図形を描画するのにかける時間.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawEllipse(
            Vector3 pos, Vector3 forward, Vector3 up,
            float radiusX, float radiusY, int segments,
            Color color, float duration = 0f
        ) {
            float angle;
            Quaternion rotation = Quaternion.LookRotation(forward, up);
            Vector3 prev = Vector3.zero;
            Vector3 current = Vector3.zero;

            for (int i = 0; i < segments + 1; ++i) {
                angle = 360f / segments * i;
                current.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
                current.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

                if (i > 0) {
                    Debug.DrawLine(
                        rotation * prev + pos,
                        rotation * current + pos,
                        color, duration
                    );
                }
                prev = current;
            }
        }
    }
}
