using OKGamesLib;
using System;
using Cysharp.Threading.Tasks;

namespace OKGamesTest {

    /// <summary>
    /// IAPのテスト用の課金ノードを生成するために必要なパラメータのTransfer.
    /// </summary>
    public class TestIAPNodeTransfer {
        /// <summary>
        /// ID情報(ProductID).
        /// </summary>
        public string ID;

        /// <summary>
        /// テキストマスターから取る課金商品のタイトル文のキー.
        /// </summary>
        public string TitleKey;

        /// <summary>
        /// テキストマスターから取る課金商品の説明文のキー.
        /// </summary>
        public string BodyKey;

        /// <summary>
        /// 通貨の単位($や¥など).
        /// </summary>
        public string CurrencyCode;

        /// <summary>
        /// 課金商品の価格.
        /// </summary>
        public string Price;

        public PurchaseState PurchaseState;

        /// <summary>
        /// 課金ノードを押した時のコールバック.
        /// </summary>
        public Func<UniTask> ButtonAsync;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public TestIAPNodeTransfer(string productID, string titleKey, string bodyKey,
            string currencyCode, string price, PurchaseState purchaseState, Func<UniTask> buttonAsync) {

            ID = productID;
            TitleKey = titleKey;
            BodyKey = bodyKey;
            CurrencyCode = currencyCode;
            Price = price;
            PurchaseState = purchaseState;
            ButtonAsync = buttonAsync;
        }
    }
}
