using UnityEngine.Purchasing;

namespace OKGamesLib {

    /// <summary>
    /// プラットフォームストア上の商品情報.
    /// </summary>
    public class StoreItem {

        /// <summary>
        /// プラットフォームストア名.
        /// </summary>
        public string StoreName { get; private set; }

        /// <summary>
        /// 非消費型商品のID.
        /// </summary>
        public string ProductID { get; private set; }

        public ProductType ProductType { get; private set; }

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="storeName">課金アイテムを取り扱うプラットフォームストアの名前.</param>
        /// <param name="productID">課金アイテムのID.</param>
        /// <param name="productType">課金アイテムタイプ.</param>
        public StoreItem(string storeName, string productID, string productType) {
            StoreName = storeName;
            ProductID = productID;

            if (productType == "nonconsumable") {
                ProductType = ProductType.NonConsumable;
            } else if (productType == "consumable") {
                ProductType = ProductType.Consumable;
            } else if (productType == "subscription") {
                ProductType = ProductType.Subscription;
            } else {
                Log.Error("【StoreItem】 サポートしていないproductType");
            }
        }
    }
}
