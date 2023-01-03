namespace OKGamesLib {

    /// <summary>
    /// ポップ通知を管理するクラスのインターフェース.
    /// </summary>
    public interface IPopNotify {

        /// <summary>
        /// 初期化.
        /// </summary>
        void Init();

        /// <summary>
        /// 真ん中のコンテナを使って通知する.
        /// </summary>
        /// <param name="text">通知で表示するテキスト.</param>
        void ShowCenter(string text);

    }
}
