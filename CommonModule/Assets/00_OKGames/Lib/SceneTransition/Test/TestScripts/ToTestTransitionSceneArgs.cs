using OKGamesLib;

// ---------------------------------------------------------
// シーン遷移の挙動のテスト用.
// ---------------------------------------------------------
public class ToTestTransitionSceneArgs : SceneDataArgs {
    // 前とこれから読むシーン.
    private readonly GameScene _previousGameScene;
    private readonly GameScene[] _previousAdditiveGameScenes;
    public override GameScene PreviousGameScene {
        get { return _previousGameScene; }
    }
    public override GameScene[] PreviousAdditiveScenes {
        get { return _previousAdditiveGameScenes; }
    }

    // 固有引数がうまく渡っているかのテスト用.
    public string TestText { get; private set; }

    /// <summary>
    /// コンストラクタ.
    /// </summary>
    public ToTestTransitionSceneArgs(string testText, GameScene previousScene, GameScene[] additiveScenes) {
        TestText = testText;
        _previousGameScene = previousScene;
        _previousAdditiveGameScenes = additiveScenes;
    }
}
