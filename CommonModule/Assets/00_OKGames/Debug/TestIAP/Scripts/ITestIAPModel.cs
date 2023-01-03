using OKGamesLib;
using UniRx;
using UnityEngine.Purchasing;

namespace OKGamesTest {

    /// <summary>
    /// IAPのテスト用のシーンのモデルのインターフェース.
    /// </summary>
    public interface ITestIAPModel {

        /// <summary>
        /// ストア商品のリアクティブコレクション.
        /// </summary>
        public IReadOnlyReactiveCollection<ProductWrapper> Prodcts { get; }

        /// <summary>
        /// 課金処理中かどうか.
        /// </summary>
        public bool IsProcessingPurchase { get; }

        /// <summary>
        /// 購入した商品.
        /// </summary>
        public ProductWrapper BoughtProduct { get; }


        /// <summary>
        /// 初期化.
        /// </summary>
        void Init();

        /// <summary>
        /// 依存性の注入.
        /// </summary>
        /// <param name="iap">iapシステム.</param>
        void Inject(IIAP iap);

        /// <summary>
        /// セットアップ.
        /// </summary>
        void Setup();

        /// <summary>
        /// 課金商品の購入.
        /// </summary>
        /// <param name="productID">購入する商品のID.</param>
        void BuyProduct(string productID);

        /// <summary>
        /// ストア商品のリアクティブコレクションの指定した商品を更新する.
        /// </summary>
        /// <param name="product">更新する商品.</param>
        void Update(ProductWrapper product);

        /// <summary>
        /// リストアが必要な商品があればリストア処理する.
        /// 端末からアプリを削除して再インストールした際や、端末変更した際にiOSでは手動でリストアしてもらう必要がある.
        /// (何かすでに購入したアイテムを再購入(無料)しても過去購入文が反映されるが別UIからも復元を可能にする用).
        /// </summary>
        public void RestoreIfNeeded();

        /// <summary>
        /// 破棄時の処理.
        /// </summary>
        void Dispose();

    }
}
