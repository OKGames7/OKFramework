namespace OKGamesLib {
    /// <summary>
    /// 課金失敗理由.
    /// </summary>
    public enum IAPBuyFailureReason {
        None,              // エラーなし
        NotInitialization, // 初期化がされてない.
        Initializing,      // 初期化中.
        UnknownItem,       // 販売されていないアイテム.
        NotReceiveMessage, // 課金メッセージを受け取れない
        NetworkUnavailbale,// (初期化は済んでいるが)通信負荷状態.
        NotSupported,      // 非サポート(リストアの場合).
        Unknown,           // 不明なエラー.
    }
}
