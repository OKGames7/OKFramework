namespace PJ {
    /// <summary>
    /// 起動タイプ.
    /// </summary>
    public enum BootType {
        // 用意がまだ(OKGames側の).
        NotReady,
        // 共通モジュールをテストするためのデバッグプレイ用.
        CommonModuleDebug,
        // OKGamesのモジュールを用いてPJをプレイするため用.
        PJRoutine
    }
}
