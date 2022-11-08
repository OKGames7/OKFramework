namespace OKGamesLib {

    // ---------------------------------------------------------
    // シーンを跨いでデータを受け渡す時に使用する.
    // ---------------------------------------------------------
    public abstract class SceneDataArgs {

        // 前のシーン.
        public abstract GameScene PreviousGameScene { get; }

        // 前のシーンで追加ロードしていたシーン一覧.
        public abstract GameScene[] PreviousAdditiveScenes { get; }

    }
}
