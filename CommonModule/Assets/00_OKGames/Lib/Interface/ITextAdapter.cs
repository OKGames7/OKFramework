using Cysharp.Threading.Tasks;

namespace OKGamesLib {

    /// <summary>
    /// テキストを制御するために必要なクラスの注入や設定を仲介するアダプターのインターフェース.
    /// </summary>
    public interface ITextAdapter {

        /// <summary>
        /// 依存性を注入する.
        /// </summary>
        /// <param name="transfar">依存性注入するデータが入っているtransfer.</param>
        /// <param name="loader">フォント関連のローダー.</param>
        void Inject(IUITransfer transfer, IFontLoader loader);

        /// <summary>
        /// セットアップ処理.
        /// </summary>
        /// <param name="wrapper">登録するテキスト.</param>
        /// <returns>UniTask</returns>
        UniTask Setup(TextWrapper wrapper);

        /// <summary>
        /// watcherのリストから削除する.
        /// </summary>
        /// <param name="wrapper">登録削除するテキスト.</param>
        void Remove(TextWrapper wrapper);

        /// <summary>
        /// 全テキストのテキスト表示を更新する.
        /// </summary>
        /// <param name="language">設定言語.</param>
        void UpdateTexts(Language language);
    }
}
