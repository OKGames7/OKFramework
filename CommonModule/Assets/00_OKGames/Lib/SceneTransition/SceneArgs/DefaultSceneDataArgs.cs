namespace OKGamesLib {
    // ---------------------------------------------------------
    // シーン間で持ち越しするデータの標準の
    // ---------------------------------------------------------
    public class DefaultSceneDataArgs : SceneDataArgs {

        // 前のシーン.
        private readonly GameScene prevGameScene;
        // 追加するシーン一覧.
        private readonly GameScene[] additiveScenes;

        /// <summary>
        /// 追加シーン一覧の外部取得用.
        /// </summary>
        public GameScene[] AdditiveScenes {
            get { return additiveScenes; }
        }

        /// <summary>
        /// 前シーンの外部取得用.
        /// </summary>
        public override GameScene PreviousGameScene {
            get { return prevGameScene; }
        }

        /// <summary>
        /// 前シーン一覧の外部取得用.
        /// 未実装: 拡張したい場合に実装する.
        /// </summary>
        public override GameScene[] PreviousAdditiveScenes {
            get { return null; }
        }

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public DefaultSceneDataArgs(GameScene prev, GameScene[] additives) {
            prevGameScene = prev;
            additiveScenes = additives;
        }
    }
}
