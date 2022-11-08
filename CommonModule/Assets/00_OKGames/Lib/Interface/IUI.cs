using Cysharp.Threading.Tasks;
using UniRx;

namespace OKGamesLib {

    /// <summary>
    /// UI全般に関するアクセスポイントのインターフェース.
    /// </summary>
    public interface IUI {
        /// <summary>
        /// フォントアセットを取得するためのローダー.
        /// </summary>
        IFontLoader FontLoader { get; }

        /// <summary>
        /// シーン全体のボタンを制御するためのアダプター.
        /// </summary>
        IButtonAdapter ButtonAdapter { get; }

        /// <summary>
        /// シーン全体のテキストを制御するためのアダプター.
        /// </summary>
        ITextAdapter TextAdapter { get; }

        /// <summary>
        /// 初期化処理.
        /// 主にメンバ変数の初期化処理がされることを想定している.
        /// </summary>
        void Init();

        /// <summary>
        /// 依存関係の注入.
        /// </summary>
        /// <param name="transfar">依存性注入するデータが入っているtransfer.</param>
        void Inject(IUITransfer transfar);

        /// <summary>
        /// 全テキストのテキスト表示を更新する.
        /// </summary>
        /// <param name="lang"></param>
        void UpdateTexts(Language lang);
    }
}
