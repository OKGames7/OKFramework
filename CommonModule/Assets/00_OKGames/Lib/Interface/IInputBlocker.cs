namespace OKGamesLib {

    /// <summary>
    /// ユーザーの入力ブロック状態を管理をするクラスのインターフェース.
    /// </summary>
    public interface IInputBlocker {

        /// <summary>
        /// 処理中プロセスのカウンターをインクリメントする.
        /// </summary>
        void AddBusyProcess();

        /// <summary>
        /// 処理中プロセスのカウンターをディクリメンとする.
        /// </summary>
        void ReduceBusyProcess();

        /// <summary>
        /// ブロック中かどうか.
        /// </summary>
        /// <returns>ブロック中ならばtrue.</returns>
        bool IsBlocking();

    }
}
