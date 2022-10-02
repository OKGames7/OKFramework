using OKGamesLib;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OKGamesLib {

    /// <summary>
    /// Tween系の汎用処理.
    /// </summary>
    public class Tween : ITween {

        /// <summary>
        /// 開始値.
        /// </summary>
        private float _from;

        /// <summary>
        /// 終了値.
        /// </summary>
        private float _to;

        /// <summary>
        /// 開始値から終了値まで変化にかける時間.
        /// </summary>
        private float _duration;

        /// <summary>
        /// 変化曲線関数.
        /// </summary>
        private EasingFunc _easingFunc;

        /// <summary>
        /// Tween処理中のUpdate処理.
        /// </summary>
        private TweenCallback _onUpdate;

        /// <summary>
        /// Tween処理終了後の完了時処理.
        /// </summary>
        private TweenCallback _onComplete;

        /// <summary>
        /// Tween開始してから経った時間.
        /// </summary>
        private float _passedTime = 0f;

        /// <summary>
        /// Tween処理を遅延させる時間.
        /// </summary>
        private float _delayTime = 0;

        /// <summary>
        /// コントラクタ.
        /// </summary>
        /// <param name="from">開始値.</param>
        /// <param name="to">終了値.</param>
        /// <param name="duration">開始値から終了値まで変化に何秒かけるか.</param>
        /// <param name="easingFunc">変化曲線の種類.</param>
        /// <param name="onUpdate">Tween処理中にさせる処理.</param
        /// <param name="onComplete">Tween終了時にさせる処理.</param>
        public Tween(
            float from = 0f, float to = 0f, float duration = 0f,
            EasingFunc easingFunc = null,
            TweenCallback onUpdate = null,
            TweenCallback onComplete = null
        ) {
            _from = from;
            _to = to;
            _duration = duration;
            _easingFunc = easingFunc;
            _onUpdate = onUpdate;
            _onComplete = onComplete;
        }

        public ITween FromTo(
            float from, float to, float duration,
            EasingFunc easingFunc = null) {

            _from = from;
            _to = to;
            _duration = duration;
            _easingFunc = (easingFunc != null) ? easingFunc : Ease.Linear;
            return this;
        }

        public ITween OnUpdate(TweenCallback onUpdate) {
            _onUpdate = onUpdate;
            _passedTime = 0;
            Update(0);
            return this;
        }

        public ITween OnComplete(TweenCallback onComplete) {
            _onComplete = onComplete;
            return this;
        }

        public ITween Delay(float delaySec) {
            _delayTime = delaySec;
            return this;
        }

        public async UniTask Async(bool autoCancelOnSceneChange = true) {
            if (autoCancelOnSceneChange && OKGamesFramework.OKGames.HasSceneContext) {
                await OKGamesFramework.OKGames.Async(
                    UniTask.WaitUntil(() => IsCompleted())
                );
                return;
            }
        }

        public ITween SetAlpha(Graphic graphic) {
            return OnUpdate(x => {
                if (graphic == null) {
                    return;
                }

                var color = graphic.color;
                color.a = x;
                graphic.color = color;
            });
        }

        public ITween SetAlpha(CanvasGroup canvasGroup) {
            return OnUpdate(x => {
                if (canvasGroup == null) {
                    return;
                }
                canvasGroup.alpha = x;
            });
        }

        /// <summary>
        /// 外部から行う初期化.
        /// </summary>
        public void Init() {
            _passedTime = 0;
            Update(0);
        }

        /// <summary>
        /// 外部から行う毎フレーム行う処理.
        /// </summary>
        /// <param name="deltaTime">deltaTime.</param>
        public void Update(float deltaTime) {
            if (IsCompleted()) {
                return;
            }

            if (_easingFunc == null) {
                Log.Warning($"_easingFunc is empty.");
            }

            if (_onUpdate == null) {
                Log.Warning($"_onUpdate is empty.");
            }

            if (_delayTime > 0) {
                _delayTime -= deltaTime;
                return;
            }

            _passedTime += deltaTime;
            if (_passedTime >= _duration) {
                Complete();
                return;
            }

            float t = _easingFunc(_passedTime / _duration);
            float x = _from + (_to - _from) * t;
            _onUpdate(x);
        }

        /// <summary>
        /// Tweenの完了処理.
        /// </summary>
        public void Complete() {
            _passedTime = _duration;
            _onUpdate(_to);
            _onComplete?.Invoke(_to);
        }

        /// <summary>
        /// Tween処理が完了しているか.
        /// </summary>
        /// <returns>完了しているか.</returns>
        public bool IsCompleted() {
            return (_passedTime >= _duration);
        }
    }
}
