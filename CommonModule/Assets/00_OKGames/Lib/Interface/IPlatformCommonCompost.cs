using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using Cysharp.Threading.Tasks;

namespace OKGamesLib {

    /// <summary>
    /// 各ストアでの課金に関わる共通処理のインターフェース.
    /// </summary>
    public interface IPlatformCommonCompost {

        /// <summary>
        /// 初期化処理成功時のコールバック.
        /// </summary>
        /// <param name="name">ストアで販売されている商品.</param>
        /// <param name="error">レシート検証処理.</param>
        void OnInitialized(Product[] products, Func<string, PurchaseState> checkReceipt);

        /// <summary>
        /// 初期化処理失敗時のコールバック.
        /// </summary>
        /// <param name="name">クラス名.</param>
        /// <param name="error">エラー内容.</param>
        void OnInitializeFailed(string name, InitializationFailureReason error);

        /// <summary>
        /// 購入開始処理.
        /// </summary>
        /// <param name="items">ストアに登録されているアイテム情報.</param>
        /// <param name="lister">購入状態のコールバックを返すLister先.</param>
        /// <param name="builder">購入</param>
        void InitializePurchasing(StoreItem[] items, IStoreListener lister, ConfigurationBuilder builder);


        /// <summary>
        /// 購入の実処理.
        /// この処理の開始時点ではまだ購入は済んでいない。
        /// 引数のデータを基にレシート検証を行い問題なければPurchaseProcessingResult.Completeを返すことで課金処理完了となる.
        /// </summary>
        /// <param name="name">クラス名.</param>
        /// <param name="args">ストアから返される課金アイテムに関する引数.</param>
        /// <param name="checkReceipt">レシート情報をチェックするファンクション.</param>
        /// <returns>コンフィグレーションを読み取るためのビルダー.</returns>
        PurchaseProcessingResult ProcessPurchase(string name, PurchaseEventArgs args, Func<string, PurchaseState> checkReceipt);

        /// <summary>
        /// 購入処理失敗時のコールバック.
        /// </summary>
        /// <param name="name">クラス名.</param>
        /// <param name="product">購入失敗した商品.</param>
        /// <param name="failureReason">失敗理由.</param>
        void OnPurchaseFailed(string name, Product product, PurchaseFailureReason failureReason);
    }
}
