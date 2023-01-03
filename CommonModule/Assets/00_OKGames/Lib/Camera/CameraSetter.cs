using System;
using System.Reflection;
using UnityEngine;

namespace OKGamesLib {

    // ---------------------------------------------------------
    // カメラタイプ設定によってCameraComponentの各種設定を動的に変更する.
    // ---------------------------------------------------------
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("o.k.games/UI/CameraSetter")]
    public class CameraSetter : MonoBehaviour {

        // この値によってカメラの各種設定を振り分ける.
        [SerializeField]
        private LayerName.LayerNameEnum _layer = LayerName.LayerNameEnum.Default;

        // depthの定数.
        // 標準はmiddle.
        [SerializeField]
        private DepthType _depth = DepthType.Middle;
        private enum DepthType {
            Under = 10,
            Middle = 20,
            Over = 30,
        }

        // 同じtypeのカメラ同士で描画順の調整が必要であればこの値で描画順調整する.
        [SerializeField] private int _offsetDepth = 0;

        [SerializeField] private Camera _camera;


        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Awake() {
            Setup();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Update() {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                // Editorで編集中だけに処理させる
                Setup();
            }
#endif
        }

        /// <summary>
        /// [SerializeFiled]のtypeによってカメラコンポーネントの各種設定を行う.
        /// </summary>
        [ContextMenu("Setup")]
        public void Setup() {
            if (_camera == null) {
                _camera = GetComponent<Camera>();
            }

            // カリングマスクの設定.
            SetupCullingMask(_layer);
            // depthの設定.
            SetupDepth(_layer, _depth, _offsetDepth);
            // タイプ毎の固有設定.
            SetupOtherSettings(_layer);
        }

        /// <summary>
        /// typeによってCullingの設定を行う.
        /// </summary>
        private void SetupCullingMask(LayerName.LayerNameEnum layer) {
            /// typeがDefault以外は用途に応じたレイヤーのみを描画する設定.
            int cullingMask;
            switch (layer) {
                default:
                    var t = typeof(LayerMask);
                    cullingMask = (int)LayerExtension.GetLayerMask(layer);
                    break;
                case LayerName.LayerNameEnum.Default:
                    cullingMask = -1;
                    break;
                case LayerName.LayerNameEnum.TransitionBoard:
                    cullingMask = (int)LayerMasks.LayerMasksEnum.TransparentFX | (int)LayerMasks.LayerMasksEnum.TransitionBoard;
                    break;
            }
            // Cameraコンポーネントへ設定.
            _camera.cullingMask = cullingMask;
        }

        /// <summary>
        /// typeによってdepthの設定を行う.
        /// </summary>
        private void SetupDepth(LayerName.LayerNameEnum layer, DepthType type, int offset = 0) {
            int baseDepth;
            switch (layer) {
                case LayerName.LayerNameEnum.UI:
                case LayerName.LayerNameEnum.Character:
                    baseDepth = 100;
                    break;
                case LayerName.LayerNameEnum.Dialog:
                    baseDepth = 200;
                    break;
                case LayerName.LayerNameEnum.PopText:
                    baseDepth = 300;
                    break;
                case LayerName.LayerNameEnum.TransitionBoard:
                    baseDepth = 400;
                    break;
                case LayerName.LayerNameEnum.NowLoading:
                    baseDepth = 500;
                    break;
                case LayerName.LayerNameEnum.SystemDialog:
                    baseDepth = 600;
                    break;
                case LayerName.LayerNameEnum.TapEffect:
                    baseDepth = 700;
                    break;
                case LayerName.LayerNameEnum.Debug:
                    baseDepth = 900;
                    break;
                default:
                    baseDepth = 0;
                    break;
            }
            // typeに設定した値と、その中で差分をつけるDepthTypeと、さらにDepthTypeも同じ場合はoffsetで描画順設定する.
            _camera.depth = baseDepth + (int)type + offset;
        }

        /// <summary>
        /// typeによってカメラコンポーネント内のそのほかの設定を行う.
        /// </summary>
        private void SetupOtherSettings(LayerName.LayerNameEnum layer) {
            // 必要になれば記入していく.
            switch (layer) {
                case LayerName.LayerNameEnum.Default:

                    break;
                case LayerName.LayerNameEnum.TransparentFX:

                    break;
                case LayerName.LayerNameEnum.Ignore_Raycast:

                    break;
                case LayerName.LayerNameEnum.Water:
                    break;
                case LayerName.LayerNameEnum.UI:
                case LayerName.LayerNameEnum.Dialog:
                case LayerName.LayerNameEnum.PopText:
                case LayerName.LayerNameEnum.NowLoading:
                case LayerName.LayerNameEnum.Debug:
                case LayerName.LayerNameEnum.SystemDialog:
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    transform.localScale = Vector3.one;
                    _camera.clearFlags = CameraClearFlags.Depth;
                    _camera.orthographic = true;
                    break;
                case LayerName.LayerNameEnum.Character:
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    transform.localScale = Vector3.one;
                    _camera.orthographic = false;
                    break;
                case LayerName.LayerNameEnum.TapEffect:
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    transform.localScale = Vector3.one;
                    _camera.orthographic = false;
                    _camera.clearFlags = CameraClearFlags.Depth;
                    _camera.fieldOfView = 60.0f;
                    break;
            }
        }
    }
}
