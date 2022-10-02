namespace OKGamesLib {

    /// オブジェクトをプール処理する.
    /// <see cref="ObjectPoolRegistry"/>>と何かのクラス/そのクラスの処理を紐づけるためのハブ.
    /// </summary>
    public class ObjectPoolHub : IObjectPoolHub {

        public ObjectPoolRegistry SceneScopeObjectPoolRegistry { get; private set; }

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="sceneDirector"><シーンスコープの<see cref="Tweener"/>を紐づける<see cref="SceneDirector"/></param>
        public ObjectPoolHub(OKGamesFramework.ISceneDirector sceneDirector) {
            SceneScopeObjectPoolRegistry = new ObjectPoolRegistry();

            sceneDirector.SceneLoading += OnSceneLoading;
        }

        /// <summary>
        /// <see cref="SceneDirector.SceneUpdate"/>に追加するアクション.
        /// </summary>
        private void OnSceneLoading() {
            SceneScopeObjectPoolRegistry.Clear();
        }
    }
}
