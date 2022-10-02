using OKGamesFramework;
using OKGamesLib;
using UnityEngine;
using UnityEngine.UI;

namespace OKGamesLib {

    // ---------------------------------------------------------
    // 処理待ちの間に表示するNow Loadingの表示管理クラス.
    // ---------------------------------------------------------
    public class NowLoadingComponent : MonoBehaviour, INowLoading {

        [SerializeField] private GameObject _canvas = null;
        [SerializeField] private CanvasSetter _canvasSetter = null;

        [SerializeField] private Slider _slider = null;


        /// <summary>
        /// キャンバスで描画するためのカメラをセットする.
        /// </summary>
        public void SetCanvasCamera(Camera camera) {
            _canvasSetter.SetCamera(camera);
        }

        /// <summary>
        /// NowLoadingのUIの表示.
        /// </summary>
        public void Show(float progress) {
            SetProgressBar(progress);
            SetActive(true);
        }

        /// <summary>
        /// NowLoadingのUIの非表示.
        /// </summary>
        public void Close() {
            SetActive(false);
        }

        /// <summary>
        /// NowLoadingのUIの表示、非表示.
        /// </summary>
        private void SetActive(bool active) {
            _canvas.SetActive(active);
        }

        /// <summary>
        /// 進捗率に合わせてスライダー中身の表示を更新する.
        /// </summary>
        private void SetProgressBar(float progress) {
            Log.Notice(progress);
            _slider.value = progress;
        }
    }
}
