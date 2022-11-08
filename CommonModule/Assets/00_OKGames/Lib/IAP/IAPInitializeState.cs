namespace OKGamesLib {

    /// <summary>
    /// 課金の初期化ステータス.
    /// </summary>
    public enum IAPInitializeState {
        NotInitialization, // 初期化をまだ行っていない.
        Initializing,      // 初期化中.
        Success,           // 初期化成功.
        Failure,           // 初期化失敗.
    }
}
