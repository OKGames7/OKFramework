namespace OKGamesLib {

    /// <summary>
    /// BGMサウンドの再生/停止を管理する.
    /// </summary>
    public class BgmPlayer : AudioPlayer {

        protected override string AudioSourceObjectName => "BGM Source";
        protected override bool DefaultLoop => true;
        protected override bool DefaultMix => false;
        protected override bool DefaultReplay => false;
    }
}
