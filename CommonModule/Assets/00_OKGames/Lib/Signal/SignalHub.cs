namespace OKGamesLib {

    /// <summary>
    /// <see cref="SignalRegistry"/>と何かのクラス/そのクラスの処理を紐づけるためのハブ.
    /// </summary>
    public class SignalHub : ISignalHub {

        public SignalRegistry GlobalScopeSignalRegistry { get; private set; }
        public SignalRegistry SceneScopeSignalRegistry { get; private set; }

        /// <summary>
        /// コントラクタ。
        /// </summary>
        /// <param name="sceneDirector"><シーンスコープの<see cref="Tweener"/>を紐づける<see cref="SceneDirector"/></param>
        public SignalHub(OKGamesFramework.ISceneDirector sceneDirector) {
            GlobalScopeSignalRegistry = new SignalRegistry();
            SceneScopeSignalRegistry = new SignalRegistry();

            sceneDirector.SceneLoading += OnSceneLoading;
        }

        /// <summary>
        /// <see cref="SceneDirector.OnCeneLoading"/>に追加するアクション.
        /// </summary>
        private void OnSceneLoading() {
            SceneScopeSignalRegistry.Clear();
        }

    }
}
