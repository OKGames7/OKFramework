using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// オーディオ再生を制御する.
    /// </summary>
    /// <remarks>
    ///
    /// 【Play関数の使い方】
    /// 　- loop : trueならループ再生する。BGMはデフォルトでtrue, SEはデフォルトでfalse.
    /// 　- mix　: trueなら多重再生する. BGMはデフォルトでfalse, SEはデフォルトでtrue.
    /// 　- autoVolume : trueなら同じ音を同時再生したときのボリュームの高まりを抑える.
    /// 　　※ mixがfalseの場合はtrueをしていてもfalseとして扱われる.
    /// 　- replay : trueならすでに指定の音が鳴っていた場合でも再生を行う.
    /// 　　- BGMはデフォルトでfalse, SEはデフォルトでtrue.
    ///
    /// 【ボリュームについて】
    ///　 - ボリュームは個別のvolume指定にsystemMasterVolumeとUserMasterVolumeを
    ///　　乗じて[0, 1]にClampしたものが適用される. MasterVolumeを1未満にしていた場合
    ///　　volumeに1以上の値指定すると他の音よりも相対的に音量が大きくなる.
    ///
    /// 【フェードイン/フェードアウト】
    /// 　- FadeInのawaitはフェードイン完了時に返す.
    /// 　　すでに再生中の音でreplayにfalseを指定した場合は再生がキャンセルされるのでawaitは即時に返る.
    ///　 - FadeOutも同様にフェードアウト完了時にawaitを返す.
    ///　 　すでに音が停止中の場合はキャンセルされ、awaitが即時に返る.
    ///　 ※ FadeOut中のFadeoutは考慮されていない.
    /// </remarks>
    public class AudioPlayer : IAudioPlayer {

        private AudioSourcePool _sourcePool = new AudioSourcePool();
        private IResourceStore _resourceStore;

        /// <summary>
        /// 生成するPool用の<see cref="AudioSource"/>を付与する<see cref="GameObjet"/>の名前.
        /// </summary>
        protected virtual string AudioSourceObjectName => "AudioSource";

        /// <summary>
        /// デフォルトでループするかどうか.
        /// </summary>
        protected virtual bool DefaultLoop => false;

        /// <summary>
        /// デフォルトで多重再生できるようにするか.
        /// </summary>
        protected virtual bool DefaultMix => false;

        /// <summary>
        /// デフォルトで再生するサウンドが既に再生中の場合,サウンド冒頭から再生し直すかどうか.
        /// </summary>
        protected virtual bool DefaultReplay => false;

        private float _systemMasterVolume = 1.0f;
        public float SystemMasterVolume {
            get { return _systemMasterVolume; }
            set {
                _systemMasterVolume = Mathf.Clamp01(value);
                UpdatePlayingVolume();
            }
        }

        private float _userMasterVolume = 1.0f;
        public float UserMasterVolume {
            get { return _userMasterVolume; }
            set {
                _userMasterVolume = Mathf.Clamp01(value);
                UpdatePlayingVolume();
            }
        }

        private float _duckingVolume = 1.0f;
        public float duckingVolume {
            get { return _duckingVolume; }
            set {
                _duckingVolume = Mathf.Clamp01(value);
                UpdatePlayingVolume();
            }
        }

        private Tweener _tween = new Tweener();


        public void Init(
            GameObject gameObject, int numSourcePool,
            OKGamesFramework.ISceneDirector sceneDIrector, IResourceStore resourceStore
        ) {
            _sourcePool.Init(gameObject, AudioSourceObjectName, numSourcePool);
            _resourceStore = resourceStore;

            sceneDIrector.SceneUpdate += OnSceneUpdate;
            Log.Notice("AudioPlayer Init確認.");
        }

        public void Play(
            AudioClip audioClip, bool? loop = null, bool? mix = null, bool? replay = null,
            bool autoVolume = true, float volume = 1.0f, float pitch = 1.0f, float pan = 0f,
            float spatial = 0f, Vector3? position = null
        ) {
            PlayGeneral(
                audioClip, loop, mix, replay,
                autoVolume, 0f, volume, 0f, pitch, pan, spatial, position
            );
        }

        public void Play(
          string audioPath, bool? loop = null, bool? mix = null, bool? replay = null,
          bool autoVolume = true, float volume = 1.0f, float pitch = 1.0f, float pan = 0f,
          float spatial = 0f, Vector3? position = null
        ) {
            var audioClip = _resourceStore.GetAudio(audioPath);
            Play(audioClip, loop, mix, replay, autoVolume, volume, pitch, pan, spatial, position);
        }

        public async UniTask PlayOndemand(
            string audioPath, bool? loop = null, bool? mix = null, bool? replay = null,
            bool autoVolume = true, float volume = 1.0f, float pitch = 1.0f, float pan = 0f,
            float spatial = 0f, Vector3? position = null
        ) {
            var audioClip = await _resourceStore.GetAudioOndemand(audioPath);
            var state = PlayGeneral(
                audioClip, loop, mix, replay,
                autoVolume, 0f, volume, 0f, pitch, pan, spatial, position
            );
            if (state == null) {
                _resourceStore.ReleaseGlobalWithAutoUnLoad(audioPath);
                return;
            }

            state.onStop = () => {
                _resourceStore.ReleaseGlobalWithAutoUnLoad(audioPath);
            };
        }

        public async UniTask FadeIn(
            AudioClip audioClip, float fadeTime, float volumeFrom = 0f, float volumeTo = 1f,
            bool? loop = null, bool? mix = null, bool? replay = null,
            bool autoVolume = true, float pitch = 1.0f, float pan = 0f,
            float spatial = 0f, Vector3? position = null
        ) {
            var state = PlayGeneral(
                audioClip, loop, mix, replay,
                autoVolume, volumeFrom, volumeTo, fadeTime, pitch, pan, spatial, position
            );
            if (state == null) { return; }

            await UniTask.Delay(TimeSpan.FromSeconds(fadeTime));
        }

        public async UniTask FadeIn(
            string audioPath, float fadeTime, float volumeFrom = 0f, float volumeTo = 1f,
            bool? loop = null, bool? mix = null, bool? replay = null,
            bool autoVolume = true, float pitch = 1.0f, float pan = 0f,
            float spatial = 0f, Vector3? position = null
        ) {
            var audioClip = _resourceStore.GetAudio(audioPath);
            await FadeIn(
                audioClip, fadeTime, volumeFrom, volumeTo,
                loop, mix, replay, autoVolume, pitch, pan, spatial, position
            );
        }

        public async UniTask FadeOut(
            float fadeTime, AudioClip audioClip = null, float volumeFrom = 1f, float volumeTo = 0f
        ) {
            float finalVolumeFrom = GetVolume(volumeFrom);
            float finalVolumeTo = GetVolume(volumeTo);

            var states = (audioClip != null)
                ? _sourcePool.GetByPlayingAudioClipList(audioClip)
                : _sourcePool.GetPlayingList();


            if (states.Count == 0) { return; }

            foreach (var state in states) {
                state.SetFade(finalVolumeFrom, finalVolumeTo, fadeTime);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(fadeTime));
            foreach (var state in states) {
                state.StopSource();
            }
        }

        public async UniTask FadeOut(
           float fadeTime, string audioPath, float volumeFrom = 1f, float volumeTo = 0f
       ) {
            var audioClip = _resourceStore.GetAudio(audioPath);
            await FadeOut(fadeTime, audioClip, volumeFrom, volumeTo);
        }

        public void Stop(AudioClip audioClip = null) {
            if (audioClip == null) {
                _sourcePool.StopAll();
            } else {
                _sourcePool.Stop(audioClip);
            }
        }

        public void Stop(string audioPath) {
            var audioClip = _resourceStore.GetAudio(audioPath);
            Stop(audioClip);
        }

        public async UniTask CrossFade(
          AudioClip audioClip, float fadeOutTime, float fadeInDelay, float fadeInTime,
          float volumeFrom = 0f, float volumeTo = 1f, bool? loop = null,
          bool autoVolume = true, float pitch = 1.0f, float pan = 0f,
          float spatial = 0f, Vector3? position = null
      ) {
            var playingStates = _sourcePool.GetPlayingList();
            foreach (var state in playingStates) {
                FadeOut(fadeOutTime, state.Source.clip, state.Source.volume, 0f).Forget();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(fadeInDelay));
            await FadeIn(
                audioClip, fadeInTime, volumeFrom, volumeTo,
                loop, mix: true, replay: true, autoVolume, pitch, pan, spatial, position
            );
        }

        public async UniTask CrossFade(
            string audioPath, float fadeOutTime, float fadeInDelay, float fadeInTime,
            float volumeFrom = 0f, float volumeTo = 1f, bool? loop = null,
            bool autoVolume = true, float pitch = 1.0f, float pan = 0f,
            float spatial = 0f, Vector3? position = null
        ) {
            var audioClip = _resourceStore.GetAudio(audioPath);
            await CrossFade(
                audioClip, fadeOutTime, fadeInDelay, fadeInTime, volumeFrom, volumeTo,
                loop, autoVolume, pitch, pan, spatial, position
            );
        }

        public async UniTask Ducking(
            float _duckTime, float volumeScale = 0.25f,
            float _fadeOutTime = 0.2f, float _fadeInTime = 0.4f
        ) {
            float fadeOutTime = Mathf.Max(_fadeOutTime, 0.01f);
            float fadeInTime = Mathf.Max(_fadeInTime, 0.01f);
            float duckTime = Mathf.Max(_duckTime - fadeOutTime - fadeInTime, 0f);
            _tween.NewTween().FromTo(1f, volumeScale, fadeOutTime, Ease.OutQuad).OnUpdate(t => {
                duckingVolume = t;
            });
            _tween.NewTween().Delay(fadeOutTime + duckTime)
                .FromTo(volumeScale, 1f, fadeInTime, Ease.Linear).OnUpdate(t => {
                    duckingVolume = t;
                }
            );
            await UniTask.Delay(TimeSpan.FromSeconds(duckTime + fadeOutTime + fadeInTime));
        }

        /// <summary>
        /// マスターボリュームをフェードで変更する。
        /// 現状リセット機能は無いので元に戻す際は手動で元の音量に FadeVolume() すること
        /// </summary>
        public async UniTask FadeVolume(
            float _duration, float volumeFrom, float volumeTo,
            EasingFunc ease = null
        ) {
            float duration = Mathf.Max(_duration, 0.01f);
            if (ease == null) {
                ease = (volumeFrom > volumeTo) ? (EasingFunc)Ease.OutQuad
                                               : (EasingFunc)Ease.Linear;
            }

            _tween.NewTween().FromTo(volumeFrom, volumeTo, duration, ease).OnUpdate(t => {
                duckingVolume = t;
            });
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
        }

        public void Pause(AudioClip audioClip = null) {
            if (audioClip == null) {
                _sourcePool.PauseAll();
            } else {
                _sourcePool.Pause(audioClip);
            }
        }

        public void Pause(string audioPath) {
            var audioClip = _resourceStore.GetAudio(audioPath);
            Pause(audioClip);
        }

        public void Resume(AudioClip audioClip = null) {
            if (audioClip == null) {
                _sourcePool.ResumeAll();
            } else {
                _sourcePool.Resume(audioClip);
            }
        }

        public void Resume(string audioPath) {
            var audioClip = _resourceStore.GetAudio(audioPath);
            Resume(audioClip);
        }

        private void OnSceneUpdate() {
            _sourcePool.Update(Time.unscaledDeltaTime);
            _tween.Update(Time.unscaledDeltaTime);
        }

        /// <summary>
        /// フェードインに対応した汎用再生処理。
        /// 再生時は再生した音の AudioSourceState を, 再生がキャンセルされた場合は null を返す
        /// </summary>
        private AudioSourceState PlayGeneral(
            AudioClip audioClip, bool? loop, bool? _mix, bool? _replay,
            bool _autoVolume, float volumeFrom, float volumeTo, float fadeTime,
            float pitch, float pan, float spatial, Vector3? _position
        ) {
            if (audioClip == null) { return null; }

            bool replay = _replay ?? DefaultReplay;
            if (!replay) {
                if (_sourcePool.IsPlaying(audioClip)) { return null; }
            }

            bool mix = _mix ?? DefaultMix;
            if (!mix) { _sourcePool.StopAll(); }

            bool autoVolume = !mix ? false : _autoVolume;

            var audioSourceState = _sourcePool.GetNext();
            audioSourceState.originalVolume = volumeTo;
            float finalVolumeFrom = GetVolume(volumeFrom, autoVolume, audioClip);
            float finalVolumeTo = GetVolume(volumeTo, autoVolume, audioClip);
            if (fadeTime <= 0f) {
                audioSourceState.SetVolumeAtOnce(finalVolumeTo);
            } else {
                audioSourceState.SetFade(finalVolumeFrom, finalVolumeTo, fadeTime);
            }

            var source = audioSourceState.Source;
            source.clip = audioClip;
            source.loop = loop ?? DefaultLoop;
            source.pitch = pitch;
            source.panStereo = pan;
            source.spatialBlend = spatial;

            Vector3 position = _position ?? Vector3.zero;
            var transform = audioSourceState.GameObject.transform;
            transform.position = position;

            source.Play();
            return audioSourceState;
        }

        private float GetVolume(float volume, bool autoVolume = false, AudioClip audioClip = null) {
            // 同じ音が多重再生された時のボリュームの高まりを抑える
            if (autoVolume) {
                var states = _sourcePool.GetByPlayingAudioClipList(audioClip);
                foreach (var state in states) {
                    if (state.Source.time <= 1 / 60f) { volume *= 0f; } else if (state.Source.time <= 1 / 30f) { volume *= 0.8f; } else if (state.Source.time <= 1 / 20f) { volume *= 0.9f; }
                }
            }

            return Mathf.Clamp01(volume * SystemMasterVolume * UserMasterVolume * duckingVolume);
        }

        /// <summary>
        /// マスターボリュームが変更された際に再生中の音のボリュームを更新する
        /// </summary>
        private void UpdatePlayingVolume() {
            var states = _sourcePool.GetPlayingList();
            foreach (var state in states) {
                float volume = GetVolume(state.originalVolume);
                state.SetTargetVolume(volume);
            }
        }
    }
}
