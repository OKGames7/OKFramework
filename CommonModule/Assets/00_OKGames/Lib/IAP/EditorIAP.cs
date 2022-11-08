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
    /// UnityEditorでのデバッグ用の課金処理を行うクラス.
    /// </summary>
    public class EditorIAP : IPlatformStoreIAP {

        /// <summary>
        /// ストア共通処理を持つコンポジット.
        /// </summary>
        private readonly IPlatformCommonCompost _commonCompost = new PlatformCommonComposit();

        /// <summary>
        /// <see cref="IPlatformStoreIAP.InitializePurchasing"/>.
        /// </summary>
        public void InitializePurchasing(StoreItem[] items, IStoreListener lister, ConfigurationBuilder builder) {
            _commonCompost.InitializePurchasing(items, lister, builder);
        }

        /// <summary>
        /// <see cref="IPlatformStoreIAP.OnInitialized"/>
        /// </summary>
        public void OnInitialized(IStoreController controller, IExtensionProvider provider) {
            Log.Notice("【AndroidIAP】 OnInitialized PASS");
        }

        /// <summary>
        /// <see cref="IPlatformStoreIAP.OnInitializeFailed"/>.
        /// </summary>
        public void OnInitializeFailed(InitializationFailureReason error) {
            string name = GetType().ToString();
            _commonCompost.OnInitializeFailed(name, error);
        }

        /// <summary>
        /// <see cref="IPlatformStoreIAP.ProcessPurchase"/>.
        /// </summary>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
            string name = GetType().ToString();
            var result = _commonCompost.ProcessPurchase(name, args, CheckReceipt);
            return result;
        }

        /// <summary>
        /// <see cref="IPlatformStoreIAP.OnPurchaseFailed"/>.
        /// </summary>
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
            string name = GetType().ToString();
            _commonCompost.OnPurchaseFailed(name, product, failureReason);
        }

        /// <summary>
        /// <see cref="IPlatformStoreIAP.CheckReceipt"/>
        /// </summary>
        public PurchaseState CheckReceipt(string receipt) {
            // 常に購入OKで返す.
            return PurchaseState.Purchased;
        }
    }
}
