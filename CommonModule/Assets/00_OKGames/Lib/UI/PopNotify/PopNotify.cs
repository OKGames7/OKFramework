using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// ポップ通知の管理クラス.
    /// </summary>
    public class PopNotify : MonoBehaviour, IPopNotify {

        // ポップ通知のrootのGameObject.
        // 使用しない時は描画負荷節約のために非表示にしておく.
        [SerializeField] private GameObject _self;

        // 画面真ん中で通知するためのコンテナ.
        [SerializeField] private PopNotifyContainer _centerContainer;

        // 現在通知中のコンテナの数.
        private int _processCount = 0;

        /// <summary>
        /// <see cref="IPopNotify.Init"/>.
        /// </summary>
        public void Init() {
            _centerContainer.Init();
            SetActive(false);
        }

        /// <summary>
        /// <see cref="IPopNotify.ShowCenter"/>.
        /// </summary>
        public void ShowCenter(string text) {
            // 通知カウントを増やす.
            ++_processCount;
            // 自身を表示する.
            SetActive(true);
            // ポップ通知の表示.
            _centerContainer.Reset();
            _centerContainer.CompleteCallback = NotifyCompleteCallback;
            _centerContainer.Show(text).Forget();
        }

        /// <summary>
        /// 通知終了後のコールバック.
        /// </summary>
        private void NotifyCompleteCallback() {
            --_processCount;
            if (_processCount <= 0) {
                SetActive(false);
                _processCount = 0;
            }
        }

        /// <summary>
        /// Viewのrootを表示/非表示する.
        /// </summary>
        /// <param name="enable">true=表示/false=非表示.</param>
        private void SetActive(bool enable) {
            _self.SetActive(enable);
        }
    }
}
