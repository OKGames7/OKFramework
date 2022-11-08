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
    /// Android(Google Play)ストアで課金処理を行うクラス.
    /// </summary>
    public class AndroidIAP : IPlatformStoreIAP {


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
#if DEVELOPMENT
            foreach (var product in controller.products.all) {
                if (product.hasReceipt) {
                    CheckReceipt(product.receipt);
                }
            }
#endif
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
            PurchaseState resultState;

            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                AppleTangle.Data(), Application.identifier);

            // アプリが強制終了しても処理続行するためにtryを使っている.
            try {
                var result = validator.Validate(receipt);
                Log.Notice("【AndroidIAP】 レシート情報は正しいです。Contents:");

                GooglePurchaseState lastGoogleResult = GooglePurchaseState.Cancelled;
                DateTime lastData = DateTime.Today;
                bool setdata = false;
                foreach (IPurchaseReceipt productReceipt in result) {
                    Log.Notice($"productID: {productReceipt.productID}");
                    Log.Notice($"purchaseDate: {productReceipt.purchaseDate}");
                    Log.Notice($"transactionID: {productReceipt.transactionID}");
                    GooglePlayReceipt google = (GooglePlayReceipt)productReceipt;
                    if (google != null) {
                        Log.Notice($"purchaseState: {google.purchaseState}");
                        if (!setdata) {
                            setdata = true;
                            lastData = productReceipt.purchaseDate;
                            lastGoogleResult = google.purchaseState;
                        } else {
                            if (lastData < productReceipt.purchaseDate) {
                                lastData = productReceipt.purchaseDate;
                                lastGoogleResult = google.purchaseState;
                            }
                        }
                    }
                }

                if (lastGoogleResult == GooglePurchaseState.Purchased) {
                    resultState = PurchaseState.Purchased;
                } else if ((int)lastGoogleResult == 4) {
                    // コンビニ支払いの場合.
                    resultState = PurchaseState.Pending;
                } else {
                    resultState = PurchaseState.NotPurchased;
                }
            }
            catch (IAPSecurityException e) {
                Log.Warning("【AndroidIAP】正しくないレシートです: " + e);
                resultState = PurchaseState.NotPurchased;
            }

            return resultState;
        }
    }
}
