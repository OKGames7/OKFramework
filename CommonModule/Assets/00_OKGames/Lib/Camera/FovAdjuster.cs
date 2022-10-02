using OKGamesFramework;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// 実機のアスペクト比率が異なっていてもうまくカメラに収まるように、カメラのFOVを自動調整してくれる機能.
    /// </summary>
    public class FovAdjuster : MonoBehaviour {

        /// <summary>
        /// 高さの閾値.
        /// </summary>
        [SerializeField] private float aspectThresholdV = 16.0f;

        /// <summary>
        /// 横の閾値.
        /// </summary>
        [SerializeField] private float aspectThresholdH = 9.0f;

        /// <summary>
        /// 対象の横幅.
        /// </summary>
        [SerializeField] private float subjectWidth = 20.0f;

        /// <summary>
        /// 対象の座標.
        /// </summary>
        [SerializeField] private Vector3 subjctPos = new Vector3(0, 0, 0);

        private Camera _camera;
        private Camera _mainCamera;
        private float _lastAspect = 0;

        private void Start() {
            _camera = GetComponent<Camera>();
            _mainCamera = Camera.main;
        }

        private void Update() {
            AdjustCameraFOV();
        }

        /// <summary>
        /// CameraのFOV値を調整する.
        /// </summary>
        private void AdjustCameraFOV() {
            if (_lastAspect == _mainCamera.aspect) {
                return;
            }

            _lastAspect = _mainCamera.aspect;

            _camera.fieldOfView = GetCameraFOVToFit(_camera, _mainCamera, subjectWidth);
        }

        /// <summary>
        /// 指定した閾値に収まるCameraのFOVの値を取得する.
        /// </summary>
        /// <param name="targetCamera"></param>
        /// <param name="mainCamera"></param>
        /// <param name="subjectWidth"></param>
        /// <returns></returns>
        private float GetCameraFOVToFit(Camera targetCamera, Camera mainCamera, float subjectWidth) {
            if (targetCamera == null || mainCamera == null || subjectWidth <= 0.0f) {
                Log.Error("[FovAdjuster] Invalid parameter.");
                // FOVのMaxの値.
                return 179;
            }

            // 対象とするアスペクト比率はカメラのアスペクトか、閾値としたものか小さい方を採択する.
            float aspect = Mathf.Min(mainCamera.aspect, aspectThresholdV / aspectThresholdH);

            // 対象の高さ.
            float frustumHeight = subjectWidth / aspect;

            // 対象とカメラまでの距離.
            float distance = Vector3.Distance(targetCamera.transform.position, subjctPos);

            // 対象が収まるようなFOVの値を取得.
            return 2.0f * Mathf.Atan(frustumHeight * 0.5f / distance) * Mathf.Rad2Deg;
        }
    }
}
