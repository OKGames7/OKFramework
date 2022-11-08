using System;
using UnityEngine.Purchasing;
using Cysharp.Threading.Tasks;

namespace OKGamesLib {

    /// <summary>
    /// アプリ内課金のアクセスポイントを持つクラスのインターフェース.
    /// </summary>
    public interface IIAP {

        /// <summary>
        /// 課金成功時のイベント処理.
        /// </summary>
        /// <param name="product">課金した商品.</param>
        delegate void SuccessEvent(Product product, PurchaseState purchaseState);

        /// <summary>
        /// 課金失敗時にイベント処理.
        /// </summary>
        /// <param name="product">課金失敗した商品.</param>
        /// <param name="reason">失敗理由.</param>
        delegate void FailureEvent(Product product, PurchaseFailureReason reason);

        /// <summary>
        /// 遅延課金(コンビニ決済)時のイベント処理.
        /// </summary>
        /// <param name="product">課金失敗した商品.</param>
        delegate void PurchaseDeferredEvent(Product product);

        /// <summary>
        /// 初期化済みか.
        /// </summary>
        /// <returns>初期化完了していたらtrue.</returns>
        bool IsInit();

        /// <summary>
        /// 初期化処理.
        /// </summary>
        /// <param name="items">ストアに登録されているアイテム情報.</param>
        /// <returns>初期化が終わればbool値を返す. trueなら成功. falseなら失敗.</returns>
        UniTask<IAPInitializeState> InitAsync(StoreItem[] items);

        /// <summary>
        /// 初期化成功時のコールバック.
        /// </summary>
        /// <param name="controller">課金システムを持つcontroller.</param>
        /// <param name="provier">リストア処理など課金の拡張システムを持つprovider.</param>
        void OnInitialized(IStoreController controller, IExtensionProvider provider);

        /// <summary>
        /// 初期化処理失敗時のコールバック.
        /// </summary>
        /// <param name="error">エラー内容.</param>
        void OnInitializeFailed(InitializationFailureReason error);

        /// <summary>
        /// ストアに登録されている商品情報を取得する.
        /// </summary>
        /// <returns>ストアの課金商品情報.</returns>
        Product[] GetProducts();

        /// <summary>
        /// ストアに登録されている商品情報とその商品の購入状態とを取得する.
        /// </summary>
        /// <returns>ストアの課金商品情報.</returns>
        (Product[] Products, PurchaseState[] PurchaseStates)? GetProductsWithState();

        /// <summary>
        /// 購入可能な商品情報を取得する.
        /// </summary>
        /// <returns>ストアの課金商品情報.</returns>
        Product[] GetProductsPossibleBuy();

        /// <summary>
        /// ストアに登録されている商品情報をproductIDで取得する.
        /// </summary>
        /// <param name="productID">購入商品のID.</param>
        /// <returns>ストアの課金商品情報.</returns>
        Product GetProduct(string productID);


        /// <summary>
        /// 商品の購入開始処理.
        /// </summary>
        /// <param name="productID">購入商品のID.</param>
        IAPBuyFailureReason BuyProductWithID(string productID, SuccessEvent successResult, FailureEvent failureEvent, PurchaseDeferredEvent purchaseDeferredEvenet);

        /// <summary>
        /// 購入の実処理.
        /// この処理の開始時点ではまだ購入は済んでいない。
        /// 引数のデータを基にレシート検証を行い問題なければPurchaseProcessingResult.Completeを返すことで課金処理完了となる.
        /// </summary>
        /// <param name="args">ストアから返される課金アイテムに関する引数.</param>
        /// <returns>コンフィグレーションを読み取るためのビルダー.</returns>
        PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args);


        /// <summary>
        /// 購入処理失敗時のコールバック.
        /// </summary>
        /// <param name="product">購入失敗した商品.</param>
        /// <param name="failureReason">失敗理由.</param>
        void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason);

        /// <summary>
        /// リストアする商品があればリストを処理を行う.
        /// </summary>
        /// <param name="successResult">リストア成功した時のイベント.</param>
        /// <param name="failureResult">リストア失敗した時のイベント.</param>
        /// <param name="restoreTransactionsCallback">リストアする商品のトランザクション処理開始時のコールバック.</param>
        /// <returns>失敗理由.</returns>
        IAPBuyFailureReason RestoreIfNeeded(SuccessEvent successResult, FailureEvent failureResult, Action restoreTransactionsCallback);
    }
}
