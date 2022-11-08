using UnityEngine.Purchasing;
using Cysharp.Threading.Tasks;

namespace OKGamesLib {

    /// <summary>
    /// 各プラットフォームの課金を行う処理のインターフェース.
    /// </summary>
    public interface IPlatformStoreIAP {

        /// <summary>
        /// IAP初期化処理.
        /// </summary>
        /// <param name="items">ストアに登録されているアイテム情報.</param>
        /// <param name="lister">購入状態のコールバックを返すLister先.</param>
        /// <param name="builder">購入</param>
        void InitializePurchasing(StoreItem[] items, IStoreListener lister, ConfigurationBuilder builder);

        /// <summary>
        /// <see cref="IIAP.OnInitialized"/>
        /// 初期化処理成功時のコールバック.
        /// </summary>
        /// <param name="controller">課金システムを持つcontroller.</param>
        /// <param name="provier">リストア処理など課金の拡張システムを持つprovider.</param>
        void OnInitialized(IStoreController controller, IExtensionProvider provier);

        /// <summary>
        /// <see cref="IIAP.OnInitializeFailed"/>
        /// 初期化処理失敗時のコールバック.
        /// </summary>
        /// <param name="error">エラー内容.</param>
        void OnInitializeFailed(InitializationFailureReason error);

        /// <summary>
        /// <see cref="IIAP.ProcessPurchase"/>
        /// 購入の実処理.
        /// この処理の開始時点ではまだ購入は済んでいない。
        /// 引数のデータを基にレシート検証を行い問題なければPurchaseProcessingResult.Completeを返すことで課金処理完了となる.
        /// </summary>
        /// <param name="args">ストアから返される課金アイテムに関する引数.</param>
        /// <returns>コンフィグレーションを読み取るためのビルダー.</returns>
        PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args);

        /// <summary>
        /// <see cref="IIAP.OnPurchaseFailed"/>
        /// 購入処理失敗時のコールバック.
        /// </summary>
        /// <param name="product">購入失敗した商品.</param>
        /// <param name="failureReason">失敗理由.</param>
        void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason);

        /// <summary>
        /// レシート検証.
        /// </summary>
        /// <param name="receipt">検証するレシート.</param>
        /// <returns>購入状態.</returns>
        PurchaseState CheckReceipt(string receipt);
    }
}
