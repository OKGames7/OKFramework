namespace OKGamesLib {

    /// <summary>
    /// SEサウンドの再生/停止を管理する.
    /// </summary>
    public class SePlayer : AudioPlayer {

        protected override string AudioSourceObjectName => "SE Source";
        protected override bool DefaultLoop => false;
        protected override bool DefaultMix => true;
        protected override bool DefaultReplay => true;
    }
}
