using Cysharp.Threading.Tasks;

namespace OKGamesLib {

    /// <summary>
    /// ボタンを制御するために必要なクラスの注入や設定を仲介するアダプターのインターフェース.
    /// </summary>
    public interface IButtonAdapter {

        /// <summary>
        /// 依存性を注入する.
        /// </summary>
        /// <param name="transfar">依存性注入するデータが入っているtransfer.</param>
        void Inject(IUITransfer transfer);

        /// <summary>
        /// セットアップ処理.
        /// </summary>
        /// <param name="wrapper">登録するボタン.</param>
        /// <returns>UniTask</returns>
        UniTask Setup(ButtonWrapper wrapper);

        /// <summary>
        /// Listから削除する.
        /// </summary>
        /// <param name="button">登録削除するボタン.</param>
        void Remove(ButtonWrapper button);
    }
}
