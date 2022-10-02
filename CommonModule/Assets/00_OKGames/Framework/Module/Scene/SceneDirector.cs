using OKGamesLib;
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OKGamesFramework {

    /// <summary>
    /// SceneContextの処理を指示/制御する.
    /// </summary>
    public class SceneDirector : MonoBehaviour, ISceneDirector {

        public ISceneContext CurrentSceneContext { get; private set; }

        public bool IsInTransition { get; private set; } = false;

        public event Action SceneLoading;
        public event Action SceneLoaded;
        public event Action SceneUpdate;

        private bool _useGlobalAudioListener = false;
        private IResourceStore _resourceStore = null;

        private Fade _screenFader = null;


        public void Init(IBootConfig bootConfig, IResourceStore resourceStore, GameObject fadeScreenRootObject) {
            _useGlobalAudioListener = bootConfig.useGlobalAudioListener;
            _resourceStore = resourceStore;

            _screenFader = fadeScreenRootObject.GetComponentInChildren<Fade>();
        }

        /// <summary>
        /// UnityEngine.Update処理.
        /// </summary>
        private void Update() {
            if (CurrentSceneContext != null && !CurrentSceneContext.IsReady) {
                return;
            }

            CurrentSceneContext?.Update();
            SceneUpdate?.Invoke();
        }

        public async UniTask GoToNextScene(ISceneContext nextSceneContext, float fadeOutTime = 0.3f, float fadeInTime = 0.3f) {
            Log.Notice($"[SceneDirector] Load scene with scene context : <b>{nextSceneContext}</b>");
            await LoadSceneWithFade(nextSceneContext, nextSceneContext.SceneName(), false, fadeOutTime, fadeInTime);
        }

        public async UniTask GoToNextScene(string nextSceneName, float fadeOutTime = 0.3f, float fadeInTime = 0.3f) {
            Log.Notice($"[SceneDirector]* No scene context is given.");
            await LoadSceneWithFade(null, nextSceneName, false, fadeOutTime, fadeInTime);
        }

        public async UniTask GoToNextSceneWithCustomTransition(ISceneContext nextSceneContext) {
            Log.Notice($"[SceneDirector] Load scene with scene context : <b>{nextSceneContext}</b>");
            await LoadSceneWithFade(nextSceneContext, nextSceneContext.SceneName(), true);
        }

        public async UniTask GoToNextSceneWithCustomTransition(string nextSceneName) {
            Log.Notice($"[SceneDirector]* No scene context is given.");
            await LoadSceneWithFade(null, nextSceneName, true);
        }

        /// <summary>
        /// 指定したシーンへ遷移する.
        /// </summary>
        /// <param name="nextSceneContext">遷移先シーンのContext.</param>
        /// <param name="nextSceneName">遷移先シーン名.</param>
        /// <param name="useCutomTransiton">カスタムの遷移を使うか.</param>
        /// <param name="fadeOutTime">フェードアウトにかける時間.</param>
        /// <param name="fadeInTime">フェードインにかける時間.</param>
        /// <returns></returns>
        private async UniTask LoadSceneWithFade(
            ISceneContext nextSceneContext,
            string nextSceneName,
            bool useCutomTransiton,
            float fadeOutTime = 0.3f,
            float fadeInTime = 0.3f
        ) {
            if (IsInTransition) {
                Log.Warning($"[SceneDirector] Now in transition - {nextSceneName} is dismissed.");
                return;
            }
            IsInTransition = true;

            // フェード処理
            if (useCutomTransiton && CurrentSceneContext != null) {
                await CurrentSceneContext.CustomFadeOut();
            } else {
                await _screenFader.FadeOut(fadeOutTime);
            }

            SceneLoading?.Invoke();

            DestroyAllObjectsInScene();

            if (CurrentSceneContext != null) {
                await CurrentSceneContext.Finalize();
                CurrentSceneContext.CancelTokenSource.Cancel();
            }

            SetIsSceneReady(false);
            CurrentSceneContext = nextSceneContext;

            // 次のシーンに必要なリソースをロード.不要なものはアンロードする.
            await LoadAndUnloadResources(nextSceneContext);

            Log.Notice("[SceneDirector] - Init <b>Before</b> Load Scene");
            if (nextSceneContext != null) {
                await nextSceneContext.InitBeforeLoadScene();
            }

            // シーン遷移させる.
            await SceneManager.LoadSceneAsync(nextSceneName);

            // シーン遷移直後は音楽はOFFに.
            DisableLocalAudioListener();

            Log.Notice("[SceneDirector] - Init <b>After</b> Load Scene");
            if (CurrentSceneContext != null) {
                await CurrentSceneContext.InitAfterLoadScene();
            }

            SetIsSceneReady(true);

            SceneLoaded?.Invoke();

            // シーンの1フレーム目は重くなりやすいので、1フレ待ってからフェードインする.
            await UniTask.DelayFrame(1);
            if (useCutomTransiton && CurrentSceneContext != null) {
                await CurrentSceneContext.CustomFadeIn();
            } else {
                await _screenFader.FadeOut(fadeOutTime);
            }

            IsInTransition = false;
            CurrentSceneContext?.OnStartupScene();
        }

        /// <summary>
        /// シーン内の全てのGameObjectを破棄する.
        /// </summary>
        private void DestroyAllObjectsInScene() {
            List<GameObject> rootObjects = new List<GameObject>();
            Scene scene = SceneManager.GetActiveScene();
            scene.GetRootGameObjects(rootObjects);

            foreach (var obj in rootObjects) {
                Destroy(obj);
            }
        }

        /// <summary>
        /// <see cref="CurrentSceneContext">の<see cref="ISceneContext.IsReady"/>のフラグをセットする.
        /// </summary>
        /// <param name="isReady"></param>
        private void SetIsSceneReady(bool isReady) {
            if (CurrentSceneContext != null) {
                CurrentSceneContext.IsReady = isReady;
            }
        }

        /// <summary>
        /// <see cref="nextSceneContext"/>に必要なリソースをロード.不要なものはアンロードする.
        /// </summary>
        /// <param name="nextSceneContext">遷移先の<see cref="ISceneContext"/></param>
        /// <returns></returns>
        private async UniTask LoadAndUnloadResources(ISceneContext nextSceneContext) {
            _resourceStore.ReleaseAllSceneScoped();
            nextSceneContext.RetainResource();
            _resourceStore.Unload();
            await _resourceStore.Load();
        }

        /// <summary>
        /// AudioListerをOffにする.
        /// </summary>
        private void DisableLocalAudioListener() {
            if (!_useGlobalAudioListener) {
                return;
            }

            if (Camera.main == null) {
                return;
            }

            var audioListener = Camera.main.GetComponent<AudioListener>();
            if (audioListener != null) {
                audioListener.enabled = false;
            }
        }


    }


}
