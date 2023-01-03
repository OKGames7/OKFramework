using OKGamesFramework;
using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;


namespace OKGamesLib {

    /// <summary>
    /// ポップ通知を中身を制御する.
    /// </summary>
    public class PopNotifyContainer : MonoBehaviour {

        // 自身のRectTransform.
        [SerializeField] private RectTransform _rect;

        // 非表示に透過させる用のCanvasGroup.
        [SerializeField] private CanvasGroup _canvasGroup;

        // ポップ通知時に表示するテキスト.
        [SerializeField] private TextWrapper _textWrapper;

        // 表示と非表示の間の時間間隔(ms).
        private readonly int _interval = 500;

        // 非表示時のアニメーション用Tween.
        private ITween _closeTeeen;
        // アニメーション開始から完了までの秒数(s).
        private readonly float _animationDuration = 0.5f;
        // 非表示完了時のコールバック.
        public Action CompleteCallback { get; set; }

        /// <summary>
        /// 初期化.
        /// </summary>
        public void Init() {
            // 見えないようにしておく.
            _rect.SetLocalScale(Vector3.zero);
            _textWrapper.SetText("");
        }

        /// <summary>
        /// 二度目以降に呼ばれた時用.
        /// </summary>
        public void Reset() {
            if (_closeTeeen != null) {
                _closeTeeen.Complete();
            } else {
                CompleteCallback?.Invoke();
                CompleteCallback = null;
            }
        }

        /// <summary>
        /// 表示する.
        /// </summary>
        /// <param name="text"></param>
        public async UniTask Show(string text) {
            // テキストの設定.
            SetText(text);

            // 通知の表示.
            _rect.SetLocalScale(Vector3.one);
            _canvasGroup.alpha = 1.0f;

            // ユーザーが通知を確認できる程度に表示し続けている.
            await UniTask.Delay(_interval);

            // ユーザータップによる強制非表示が実行されていなければシステムから非表示処理を呼ぶ.
            Hide();
        }

        /// <summary>
        /// 非表示にする.
        /// </summary>
        private void Hide() {
            // 徐々に透明にしていく.
            _closeTeeen = OKGames.Tween(_rect.gameObject)
                .FromTo(1.0f, 0.0f, _animationDuration)
                .SetAlpha(_canvasGroup)
                .OnComplete(_ => {
                    _rect.SetLocalScale(Vector3.zero);

                    SetText("");

                    CompleteCallback?.Invoke();
                    CompleteCallback = null;

                    _closeTeeen = null;
                });
        }

        /// <summary>
        /// テキストを表示する.
        /// </summary>
        /// <param name="text"></param>
        private void SetText(string text) {
            _textWrapper.SetText(text);
        }

        /// <summary>
        /// 破棄時の処理.
        /// </summary>
        private void OnDestroy() {
            _rect = null;
            _textWrapper = null;

            _closeTeeen = null;

            CompleteCallback = null;
        }
    }
}
