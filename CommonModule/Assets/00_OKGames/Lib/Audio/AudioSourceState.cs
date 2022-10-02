using System;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// <see cref="AudioSource"/>と現在のボリューム値を保持し、フェードイン/アウトを制御.
    /// </summary>
    public class AudioSourceState {

        /// <summary>
        /// 本コンポーネントが紐づいているGameObject.
        /// </summary>
        public GameObject GameObject;

        /// <summary>
        /// 本コンポーネントで扱うAudioSource.
        /// </summary>
        public AudioSource Source;

        /// <summary>
        /// 元の音量.
        /// </summary>
        public float originalVolume;

        /// <summary>
        /// フェード中か.
        /// </summary>
        public bool isFading { get; private set; } = false;


        /// <summary>
        /// サウンド再生が停止された時に行うAction.
        /// </summary>
        public Action onStop = null;

        /// <summary>
        /// フェード時の開始音量.
        /// </summary>
        private float _volumeTo;

        /// <summary>
        /// フェード時の終了音量.
        /// </summary>
        private float _volumeFrom;

        /// <summary>
        /// フェードにかける時間.
        /// </summary>
        private float _fadeTime;

        /// <summary>
        /// フェード処理の進捗.
        /// </summary>
        private float _fadeProgress;

        /// <summary>
        /// 音量設定.
        /// </summary>
        /// <param name="volume">音量.</param>
        public void SetVolumeAtOnce(float volume) {
            Source.volume = volume;
            isFading = false;
        }

        /// <summary>
        /// フェードの設定.
        /// </summary>
        /// <param name="volumeFrom">フェード開始時の音量.</param>
        /// <param name="volumeTo">フェード終了時の音量.</param>
        /// <param name="fadeTime">フェードにかける時間.</param>
        public void SetFade(float volumeFrom, float volumeTo, float fadeTime) {
            _volumeFrom = volumeFrom;
            _volumeTo = volumeTo;
            _fadeTime = fadeTime;

            Source.volume = volumeFrom;
            _fadeProgress = 0f;
            isFading = true;
        }

        /// <summary>
        /// ボリュームを状態に合わせて_volumeToとSource.volumeへ設定する.
        /// </summary>
        /// <param name="volume"></param>
        public void SetTargetVolume(float volume) {
            if (_volumeFrom < _volumeTo) {
                _volumeTo = volume;
            }

            if (!isFading) {
                Source.volume = volume;
            }
        }

        /// <summary>
        /// サウンドを停止し、ストップ時のアクションを実行する.
        /// </summary>
        public void StopSource() {
            if (!Source.isPlaying) {
                return;
            }

            Source.Stop();
            onStop?.Invoke();
            onStop = null;
        }

        /// <summary>
        /// 外部から行う毎フレームの処理.
        /// </summary>
        /// <param name="dt">deltaTime.</param>
        public void Update(float dt) {
            UpdateStopCallback();

            if (!isFading) {
                return;
            }

            if (_fadeTime <= 0f) {
                return;
            }

            _fadeProgress = dt;
            float volumeRate = CalcVolumeRate(_fadeProgress, _fadeTime, _volumeFrom, _volumeTo);
            Source.volume = (_volumeTo - _volumeFrom) * volumeRate * _volumeFrom;

            if (_fadeProgress >= _fadeTime) {
                isFading = false;
            }
        }

        /// <summary>
        /// アップデート中に停止された際のコールバック.
        /// </summary>
        private void UpdateStopCallback() {
            if (onStop == null) {
                return;
            }

            if (!Source.isPlaying) {
                onStop?.Invoke();
                onStop = null;
            }
        }

        /// <summary>
        /// フェード進行値からボリューム値を算出して返す.
        /// </summary>
        /// <param name="progress">フェードの進行値.</param>
        /// <param name="time">フェードにかける時間.</param>
        /// <param name="from">開始値.</param>
        /// <param name="to">終了値.</param>
        /// <returns>現在地点での値.</returns>
        private float CalcVolumeRate(float progress, float time, float from, float to) {

            var rate = Mathf.Clamp01(_fadeProgress / _fadeTime);

            if (from < to) {
                return rate * rate;
            }

            return 1.0f - (1f - rate) * (1f - rate);
        }
    }
}
