
namespace OKGamesLib {

    /// <summary>
    ///
    /// </summary>
    public interface IUI {
        IFontLoader FontLoader { get; }

        IButtonWatcher ButtonWatcher { get; }


        /// <summary>
        /// 初期化処理.
        /// 主にメンバ変数の初期化処理がされることを想定している.
        /// </summary>
        void Init();
    }
}
