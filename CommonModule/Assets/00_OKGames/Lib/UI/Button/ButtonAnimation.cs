using OKGamesFramework;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

namespace OKGamesLib {

    /// <summary>
    /// ボタンをアニメーションさせるためのクラス.
    /// </summary>
    public class ButtonAnimation : MonoBehaviour {

        /// <summary>
        /// アニメーション基のボタン.
        /// </summary>
        private Button _button;

        /// <summary>
        /// アニメーション元のボタンのRectTransform.
        /// </summary>
        private RectTransform _rect;

        /// <summary>
        /// 押下時のスケール縮小比率.
        /// </summary>
        private readonly float _ratioAnimationScale = 0.8f;

        /// <summary>
        /// 押下時のアニメーション完了時間.
        /// </summary>
        private readonly float _pressAnimationDuration = 0.1f;

        public void Init(Button button) {
            _button = button;
            _rect = button.GetComponent<RectTransform>();

            // ボタン押下時のアニメーション処理を購読させる.
            // interactableがoffの時でも押したら反応するので反応させないようにフィルターさせる
            _button.OnPointerDownAsObservable()
                .Where(_ => button.interactable)
                .Subscribe(_ => PlayPressAnimation())
                .AddTo(_button);

            _button.OnPointerUpAsObservable()
                .Where(_ => button.interactable)
                .Subscribe(_ => PlayResetAnimation())
                .AddTo(_button);
            _button.OnPointerExitAsObservable()
                .Where(_ => button.interactable)
                .Subscribe(_ => PlayResetAnimation())
                .AddTo(_button);

        }

        /// <summary>
        /// ボタン押下時の縮小アニメーション
        /// </summary>
        private void PlayPressAnimation() {
            float currentRatio = _rect.localScale.x / 1.0f;
            OKGames.Tween(_button.gameObject)
                .FromTo(currentRatio, _ratioAnimationScale, _pressAnimationDuration)
                .OnUpdate(ratio => {
                    _rect.SetLocalScale(ratio);
                });
        }

        /// <summary>
        /// ボタンを押した時と離した時にサイズを戻すアニメーション
        /// </summary>
        private void PlayResetAnimation() {
            float currentRatio = _rect.localScale.x / 1.0f;
            OKGames.Tween(_button.gameObject)
                .FromTo(currentRatio, 1.0f, _pressAnimationDuration)
                .OnUpdate(ratio => {
                    _rect.SetLocalScale(ratio);
                });
        }
    }
}
