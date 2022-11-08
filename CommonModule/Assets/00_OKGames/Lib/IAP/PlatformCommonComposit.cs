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
    /// 各ストアでの課金に関わる共通処理.
    /// </summary>
    public class PlatformCommonComposit : IPlatformCommonCompost {

        /// <summary>
        /// ストア商品のIDのリスト.
        /// </summary>
        private List<string> _productIDList;

        public void OnInitialized(Product[] products, Func<string, PurchaseState> checkReceipt) {
#if DEVELOPMENT
            // デバッグがしやすいので、初期化時に溜まっているレシートをログに出している.
            // 要素0のみで全購入アイテムのレシート内容を含んでいる(購入した分Productは溜まっているが全ての要素で所持ているレシート情報は同じ内容だった).
            if (products[0].hasReceipt) {
                checkReceipt(products[0].receipt);
            }
#endif
        }

        /// <summary>
        /// <see cref="IPlatformCommonCompost.OnInitializeFailed"/>
        /// </summary>
        public void OnInitializeFailed(string name, InitializationFailureReason error) {
            Log.Warning($"【{name}】 OnInitializeFailed InitializationFailureReason:" + error);
        }

        /// <summary>
        /// <see cref="IPlatformCommonCompost.InitializePurchasing"/>
        /// </summary>
        public void InitializePurchasing(StoreItem[] items, IStoreListener lister, ConfigurationBuilder builder) {
            _productIDList = new List<string>();
            for (int i = 0; i < items.Length; ++i) {
                var storeName = items[i].StoreName;
                var productID = items[i].ProductID;
                var productType = items[i].ProductType;
                builder.AddProduct(productID, productType, new IDs {
                    { productID, storeName },
                });

                _productIDList.Add(productID);
            }

            UnityPurchasing.Initialize(lister, builder);
        }

        /// <summary>
        /// <see cref="IPlatformCommonCompost.ProcessPurchase"/>
        /// </summary>
        public PurchaseProcessingResult ProcessPurchase(string name, PurchaseEventArgs args, Func<string, PurchaseState> checkReceipt) {
            if (_productIDList.Contains(args.purchasedProduct.definition.id)) {
                Log.Notice(string.Format("【{0}】 ProcessPurchase: PASS. Product: '{1}'", name, args.purchasedProduct.definition.id));
                checkReceipt(args.purchasedProduct.receipt);
            } else {
                Log.Warning(string.Format("【{0}】 ProcessPurchase: FAIL.Unrecognized product: '{1}'", name, args.purchasedProduct.definition.id));
            }

            return PurchaseProcessingResult.Complete;
        }

        /// <summary>
        /// <see cref="IPlatformCommonCompost.OnPurchaseFailed"/>
        /// </summary>
        public void OnPurchaseFailed(string name, Product product, PurchaseFailureReason failureReason) {
            Log.Warning(string.Format("【{0}】 OnPurchaseFailed: FAIL. Product: '{1}', PurchaseFailureReason: {2}", name, product.definition.storeSpecificId, failureReason));
        }
    }
}
