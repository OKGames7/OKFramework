using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// <see refs="AudioSourceState"/>のプールを保持.
    /// AudioPlayer から使用する.
    /// </summary>
    public class AudioSourcePool {

        /// <summary>
        /// プールする<see cref="AudioSourceState"/>のリスト.
        /// </summary>
        private List<AudioSourceState> _poolList = new List<AudioSourceState>();

        /// <summary>
        /// プール中のどの要素を使用しているか.
        /// </summary>
        private int _head = 0;

        /// <summary>
        /// <see cref="GetNext"/>関数で取得した<see cref="AudioSourceState"/>.
        /// </summary>
        public AudioSourceState CurrentSourceState { get; private set; }


        /// <summary>
        /// 外部からの初期化.
        /// </summary>
        /// <param name="gameObject">Poolする<see cref="AudioSource"/>を付与するGameObjectの親オブジェクト.</param>
        /// <param name="objectName">プールに紐づける<see cref="GameObjet"/>の名前.</param>
        /// <param name="numPool">Poolさせる数.</param>
        public void Init(GameObject gameObject, string objectName, int numPool) {
            for (int i = 0; i < numPool; ++i) {
                var sourceObj = new GameObject($"{objectName} ({i + 1})");
                sourceObj.transform.SetParent(gameObject.transform);
                var audioSource = sourceObj.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.dopplerLevel = 0;
                var audioState = new AudioSourceState() {
                    GameObject = sourceObj,
                    Source = audioSource,
                };

                _poolList.Add(audioState);
            }
        }

        /// <summary>
        /// 外部からマイフレーム更新する処理.
        /// </summary>
        /// <param name="dt">deltaTime.</param>
        public void Update(float dt) {
            foreach (var state in _poolList) {
                state.Update(dt);
            }
        }

        /// <summary>
        /// 最も最近使用したpoolの要素の次の要素の<see cref="AudioSourceState"/>を返す.
        /// </summary>
        /// <returns>AudioSourceState.</returns>
        public AudioSourceState GetNext() {
            ++_head;
            if (_head >= _poolList.Count) {
                _head = 0;
            }

            CurrentSourceState = _poolList[_head];
            return CurrentSourceState;
        }

        /// <summary>
        /// poolしているListから指定した<see cref="AudioSourceState"/>で再生中のListを返す.
        /// </summary>
        /// <param name="audioClip">取得したい<see cref="AudioClip"/></param>
        /// <returns>List<AudioSourceState/>.</returns>
        public List<AudioSourceState> GetByPlayingAudioClipList(AudioClip audioClip) {
            return _poolList.Where(x => x.Source.isPlaying && x.Source.clip == audioClip).ToList();
        }

        /// <summary>
        /// poolしているlistのうち、再生中の<see cref="AudioSourceState"/>のListを返す.
        /// </summary>
        /// <returns>>List<AudioSourceState/>.</returns>
        public List<AudioSourceState> GetPlayingList() {
            return _poolList.Where(x => x.Source.isPlaying).ToList();
        }

        /// <summary>
        /// 指定した<see cref="AudioClip/">が再生中か.
        /// </summary>
        /// <param name="audioClip"></param>
        /// <returns></returns>
        public bool IsPlaying(AudioClip audioClip) {
            return GetByPlayingAudioClipList(audioClip).Count > 0;
        }

        /// <summary>
        /// 全てのサウンドを停止する.
        /// </summary>
        public void StopAll() {
            foreach (var state in _poolList) {
                state.StopSource();
            }
        }

        /// <summary>
        /// <see cref="CurrentSourceState"/>のサウンドを停止する.
        /// </summary>
        public void StopCurrent() {
            if (CurrentSourceState == null) {
                return;
            }

            CurrentSourceState.StopSource();
        }

        /// <summary>
        /// 指定したサウンドを停止する.
        /// </summary>
        /// <param name="audioClip">停止させたい<see cref="AudioClip"/>.</param>
        public void Stop(AudioClip audioClip) {
            foreach (var state in _poolList) {
                if (state.Source.clip == audioClip) {
                    state.StopSource();
                }
            }
        }

        /// <summary>
        /// 全てのサウンドを一時停止する.
        /// </summary>
        public void PauseAll() {
            foreach (var state in _poolList) {
                state.Source.Pause();
            }
        }

        /// <summary>
        /// <see cref="CurrentSourceState"/>のサウンドを一時停止する.
        /// </summary>
        public void PauseCurrent() {
            if (CurrentSourceState == null) {
                return;
            }
            CurrentSourceState.Source.Pause();
        }

        /// <summary>
        /// 指定したサウンドを一時停止する.
        /// </summary>
        /// <param name="audioClip">一時停止させたい<see cref="AudioClip"/>.</param>
        public void Pause(AudioClip audioClip) {
            foreach (var state in _poolList) {
                if (state.Source.clip == audioClip) {
                    state.Source.Pause();
                }
            }
        }

        /// <summary>
        /// 全てのサウンドを再開する.
        /// </summary>
        public void ResumeAll() {
            foreach (var state in _poolList) {
                state.Source.UnPause();
            }
        }

        /// <summary>
        /// <see cref="CurrentSourceState"/>のサウンドを再開する.
        /// </summary>
        public void ResumeCurrent() {
            if (CurrentSourceState == null) {
                return;
            }

            CurrentSourceState.Source.UnPause();
        }

        /// <summary>
        /// 指定したサウンドを再開する.
        /// </summary>
        /// <param name="audioClip">再開させたい<see cref="AudioClip"/>.</param>
        public void Resume(AudioClip audioClip) {
            foreach (var state in _poolList) {
                if (state.Source.clip == audioClip) {
                    state.Source.UnPause();
                }
            }
        }
    }
}
