using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    ///
    /// </summary>
    public class GizmoUtility {

        /// <summary>
        /// 回転した立方体を表示.
        /// </summary>
        /// <param name="center">図の中心座標.</param>
        /// <param name="eulerAngles">回転角度.</param>
        /// <param name="size">図の大きさ.</param>
        public static void DrawRotatedCube(Vector3 center, Vector3 eulerAngles, Vector3 size) {
            Quaternion rotation = Quaternion.Euler(eulerAngles);
            DrawRotatedCube(center, rotation, size);
        }

        /// <summary>
        /// 回転した立方体を表示.
        /// </summary>
        /// <param name="center">図の中心座標.</param>
        /// <param name="Quaternion">回転角度.</param>
        /// <param name="size">図の大きさ.</param>
        public static void DrawRotatedCube(Vector3 center, Quaternion rotation, Vector3 size) {
            var backupMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, size);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = backupMatrix;
        }

        /// <summary>
        /// 回転した立方体をワイヤーを表示.
        /// </summary>
        /// <param name="center">図の中心座標.</param>
        /// <param name="eulerAngles">回転角度.</param>
        /// <param name="size">図の大きさ.</param>
        public static void DrawRotatedWireCube(Vector3 center, Vector3 eulerAngles, Vector3 size) {
            Quaternion ratation = Quaternion.Euler(eulerAngles);
            DrawRotatedWireCube(center, ratation, size);
        }

        /// <summary>
        /// 回転した立方体をワイヤーを表示.
        /// </summary>
        /// <param name="center">図の中心座標.</param>
        /// <param name="rotation">回転角度.</param>
        /// <param name="size">図の大きさ.</param>
        public static void DrawRotatedWireCube(Vector3 center, Quaternion rotation, Vector3 size) {
            var backupMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, size);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = backupMatrix;
        }
    }
}
