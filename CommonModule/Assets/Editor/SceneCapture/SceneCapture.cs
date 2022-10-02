using OKGamesFramework;
using System;
using UnityEngine;
using UnityEditor;

namespace OKGamesLib {

    /// <summary>
    /// GameViewの画面をSSにとって保存する.
    /// </summary>
    public class SceneCapture {

        /// <summary>
        ///  GameViewの画面全体を画像に保存する.
        /// </summary>
        [MenuItem("o.k.games/99.UnityEditor/CaptureScreen/Capture x 1")]
        private static void CaptureScreenX1() {
            CaptureScreen(1);
        }

        /// <summary>
        ///  GameViewの画面全体を画像に保存する.
        /// </summary>
        [MenuItem("o.k.games/99.UnityEditor/CaptureScreen/Capture x 2")]
        private static void CaptureScreenX2() {
            CaptureScreen(2);
        }

        /// <summary>
        ///  GameViewの画面全体を画像に保存する.
        /// </summary>
        [MenuItem("o.k.games/99.UnityEditor/CaptureScreen/Capture x 3")]
        private static void CaptureScreenX3() {
            CaptureScreen(3);
        }

        /// <summary>
        ///  GameViewの画面全体を画像に保存する.
        /// </summary>
        [MenuItem("o.k.games/99.UnityEditor/CaptureScreen/Capture x 4")]
        private static void CaptureScreenX4() {
            CaptureScreen(4);
        }

        /// <summary>
        ///  GameViewの画面全体をスクリーンショットして画像に保存する.
        /// </summary>
        /// <param name="superSize">1だと現在のGameViewの解像度で保存される. 2だとその2倍, 3だと3倍の解像度..という具合./param>
        private static void CaptureScreen(int superSize) {
            if (Camera.main == null || !Camera.main.enabled) {
                Log.Error("カメラが enabled = false の場合にキャプチャを撮ると Unity がクラッシュするようなので処理をキャンセルしました");
                return;
            }

            DateTime now = DateTime.Now;
            string timeStamp = $"{now.Year}-{now.Month.ToString("D2")}{now.Day.ToString("D2")}-"
                             + $"{now.Hour.ToString("D2")}{now.Minute.ToString("D2")}{now.Second.ToString("D2")}";
            string filePath = $"~/Desktop/{Application.productName}-{timeStamp}.png";
            UnityEngine.ScreenCapture.CaptureScreenshot(filePath, superSize);
        }
    }
}
