
namespace OKGamesLib {

    // ---------------------------------------------------------
    // NowLoadingの依存性逆転用のインターフェース.
    // ---------------------------------------------------------
    public interface INowLoading {
        /// <summary>
        /// 表示する
        /// </summary>
        void Show(float progress);

        /// <summary>
        /// 表示を閉じる.
        /// </summary>
        void Close();
    }
}
