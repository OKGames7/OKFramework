namespace OKGamesLib {

    /// <summary>
    /// <see cref="Tweener"/>と何かのクラス/そのクラスの処理を紐づけるためのハブ.
    /// </summary>
    public class TweenerHub : ITweenerHub {

        public Tweener SceneScopeTweener { get; private set; }

        private ITimeKeeper _timeKeeper;

        /// <summary>
        /// コントラクタ.
        /// </summary>
        /// <param name="sceneDirector"><シーンスコープの<see cref="Tweener"/>を紐づける<see cref="SceneDirector"/></param>
        /// <param name="timeKeeper">手動Updateに用いるdtの値を所持している<see cref="TimeKeeper"/></param>
        public TweenerHub(OKGamesFramework.ISceneDirector sceneDirector, ITimeKeeper timeKeeper) {
            _timeKeeper = timeKeeper;

            SceneScopeTweener = new Tweener();

            sceneDirector.SceneLoading += OnSceneLoading;
            sceneDirector.SceneUpdate += OnSceneUpdate;
        }

        /// <summary>
        /// <see cref="SceneDirector.OnCeneLoading"/>に追加するアクション.
        /// </summary>
        private void OnSceneLoading() {
            SceneScopeTweener.ClearAll();
        }

        /// <summary>
        /// <see cref="SceneDirector.SceneUpdate"/>に追加するアクション.
        /// </summary>
        private void OnSceneUpdate() {
            SceneScopeTweener.Update(_timeKeeper.dt);
        }
    }
}
