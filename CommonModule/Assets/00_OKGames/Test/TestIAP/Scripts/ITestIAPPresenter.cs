using OKGamesLib;

namespace OKGamesTest {

    /// <summary>
    /// IAPのテストシーン用のPresnterのインターフェース.
    /// </summary>
    public interface ITestIAPPresenter {

        /// <summary>
        /// 初期化.
        /// </summary>
        void Init();

        /// <summary>
        /// 依存性の注入.
        /// </summary>
        /// <param name="iap">iapシステム.</param>
        /// <param name="store">Addressableのstore.</param>
        void Inject(IIAP iap, IResourceStore store);

        /// <summary>
        /// 購読処理.
        /// </summary>
        void Bind();

        /// <summary>
        /// セットアップ.
        /// </summary>
        void SetupIAPItems();

        /// <summary>
        /// 破棄時の処理.
        /// </summary>
        void Dispose();
    }
}
