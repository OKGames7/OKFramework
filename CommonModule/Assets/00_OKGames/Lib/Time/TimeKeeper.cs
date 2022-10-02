using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// プレイ中全体の時間系統の処理を統括する.
    /// </summary>
    public class TimeKeeper : ITimeKeeper {

        // 実際に前フレから時間が経ち過ぎていても20フレ以上(0.05秒)は飛ばないようにする.
        public float maxDeltaTime = 1 / 20f;

        public float dt {
            // Time.maximumDeltaTimeは使わず自前で調整できるようにラップする.
            get { return Mathf.Min(Time.deltaTime, maxDeltaTime); }
        }

        public float t => _t;
        private float _t = 0f;

        /// <summary>
        /// コントラクタ.
        /// </summary>
        /// <param name="sceneDirector">ゲーム内で唯一のsceneDirector.</param>
        public TimeKeeper(OKGamesFramework.ISceneDirector sceneDirector) {
            sceneDirector.SceneLoading += OnSceneLoading;
            sceneDirector.SceneUpdate += OnSceneUpdate;
        }

        /// <summary>
        /// <see cref="ISceneDirector.SceneLoading"/>でさせたい処理.
        /// </summary>
        private void OnSceneLoading() {
            _t = 0f;
        }

        /// <summary>
        /// <see cref="ISceneDirector.SceneUpdate">でさせたい処理.
        /// </summary>
        private void OnSceneUpdate() {
            _t += dt;
        }

        public async UniTask Wait(float seconds) {
            await WaitCoroutine(seconds);
        }

        public async UniTask Wait(float seconds, Action action) {
            await WaitCoroutine(seconds);
            action?.Invoke();
        }

        /// <summary>
        /// 指定秒待つ.
        /// </summary>
        /// <param name="seconds">待つ秒数.</param>
        /// <returns>IEnumerator</returns>
        private IEnumerator WaitCoroutine(float seconds) {
            float elapsedSec = 0;
            while (elapsedSec < seconds) {
                elapsedSec += dt;
                yield return null;
            }
        }

        public async void Delay(float seconds, Action action) {
            await WaitCoroutine(seconds);
            action?.Invoke();
        }

        public async UniTask WaitFrame(int frame) {
            await UniTask.DelayFrame(frame);
        }

        public async void DelayFrame(int frame, Action action = null) {
            await UniTask.DelayFrame(frame);
            action?.Invoke();
        }
    }
}
