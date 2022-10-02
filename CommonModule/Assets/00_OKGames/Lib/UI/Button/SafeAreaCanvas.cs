using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// CanvasにアタッチすることでCanvas上の表示がSafeArea対応される.
    /// </summary>
    public class SafeAreaCanvas : MonoBehaviour {

        /// <summary>
        /// SafeArea対応するPanel.
        /// </summary>
        private RectTransform _panel;

        /// <summary>
        /// 最後に更新したパネル.
        /// </summary>
        private Rect _lastSafeArea = new Rect(0, 0, 0, 0);

        /// <summary>
        /// Awakeによる初期化.
        /// </summary>
        private void Awake() {
            _panel = GetComponent<RectTransform>();
            UpdateSafeArea();
        }

        /// <summary>
        /// 毎クレーム行う処理.
        /// </summary>
        private void Update() {
            UpdateSafeArea();
        }

        /// <summary>
        /// SafeAreaに収まるようにpanelを調整する.
        /// </summary>
        private void UpdateSafeArea() {
            Rect safeArea = Screen.safeArea;
            if (safeArea == _lastSafeArea) {
                return;
            }

            _lastSafeArea = safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            _panel.anchorMin = anchorMin;
            _panel.anchorMax = anchorMax;

        }
    }
}
