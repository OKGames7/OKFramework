using UnityEngine;
using UnityEngine.UI;

namespace OKGamesLib {

    // ---------------------------------------------------------
    // Canvasタイプ設定によってCanvasComponentの各種設定を動的に変更する.
    // ---------------------------------------------------------
    [ExecuteInEditMode]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [AddComponentMenu("o.k.games/UI/CanvasSetter")]

    public class CanvasSetter : MonoBehaviour {

        // ゲームの標準解像度.
        private static Vector2 _resotution => AppConst.Resolution;

        // この値によってCanvas系コンポーネントの各種設定を振り分ける.
        [SerializeField]
        private LayerConst.CanvasLayer _type = LayerConst.CanvasLayer.UI;

        // depthの定数.
        // 標準はmiddle.
        [SerializeField]
        private SortingOrderType _sortingOrder = SortingOrderType.Middle;
        private enum SortingOrderType {
            Under = 100,
            Middle = 200,
            Over = 300,
        }

        // 同じtypeのCanvas同士で描画順の調整が必要であればこの値で描画順調整する.
        [SerializeField] private int _offsetOrderInLayer = 0;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasScaler _canvasScaler;

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
        /// 描画用のカメラをセットする.
        /// </summary>
        public void SetCamera(Camera camera) {
            _canvas.worldCamera = camera;
        }

        /// <summary>
        /// [SerializeFiled]のtypeによってカメラコンポーネントの各種設定を行う.
        /// </summary>
        [ContextMenu("Setup")]
        public void Setup() {
            if (_canvas == null) {
                _canvas = GetComponent<Canvas>();
            }

            if (_canvasScaler == null) {
                _canvasScaler = GetComponent<CanvasScaler>();
            }

            SetupCanvasMode();

            // カリングマスクの設定.
            SetupOrderInLayer(_sortingOrder, _offsetOrderInLayer);

            // タイプ毎の固有設定.
            SetupOtherSettings(_type);
        }

        /// <summary>
        /// CanvasのモードとScalse周りの設定..
        /// </summary>
        private void SetupCanvasMode() {
            // 描画はScreenSpaceCameraモードで制御する.
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            // UIを動かすと高負荷なので基本OFFにしておく.
            _canvas.pixelPerfect = false;

            // CanvasScalerの設定.
            // 実機の解像度の倍率が変わったときにUIの見た目の倍率も変わらないようにする.
            _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canvasScaler.referencePixelsPerUnit = 100;
            _canvasScaler.referenceResolution = AppConst.Resolution;

            if (_type == LayerConst.CanvasLayer.Front) {
                // 一番前面(画面遷移のマスク画像等の場合は画面いっぱいの範囲を必ず覆えるようにShrinkにする.
                _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
            } else {
                float deviceAspectRatio = ((float)Screen.height) / Screen.width;
                float targetAspectRatio = AppConst.Resolution.y / AppConst.Resolution.x;
                if (deviceAspectRatio <= targetAspectRatio) {
                    // 実機端末の解像度で、縦比率が基準解像度の縦比率より小さければ)、縦画面に合わせてUIにオートスケーリングがかかるようにする.
                    _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    _canvasScaler.matchWidthOrHeight = 1.0f;
                } else {
                    // 縦の比率が基準よりも高い場合は、matchWidthOrHeightを1.0(縦合わせ)だと、横画面で見切れるUIが出てくるので見切れないようにExpandで設定する.
                    _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                }
            }
        }

        /// <summary>
        /// OrderInLayerの設定.
        /// </summary>
        private void SetupOrderInLayer(SortingOrderType orderType, int offset = 0) {
            _canvas.sortingOrder = (int)orderType + offset;
        }

        /// <summary>
        /// typeによってCanvasコンポーネント内のそのほかの設定を行う.
        /// </summary>
        private void SetupOtherSettings(LayerConst.CanvasLayer type) {

            _canvas.sortingLayerName = type.ToString();

            // 必要に応じて記述.
            switch (type) {
                case LayerConst.CanvasLayer.Background:
                    break;
                case LayerConst.CanvasLayer.UI:
                    break;
                case LayerConst.CanvasLayer.Front:
                    break;

            }

        }
    }
}
