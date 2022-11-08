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
    /// リストア処理などiOS固有の課金処理を持つ拡張クラス.
    /// </summary>
    public class IOSIAPExtention {

        private IStoreController _controller;
        private IExtensionProvider _provier;
        private Func<string, PurchaseState> _checkReceipt;

        private Dictionary<string, Product> _pendingProductDict;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="controller"></param>
        /// <param name="func"></param>
        public IOSIAPExtention(IStoreController controller, IExtensionProvider provider, Func<string, PurchaseState> func) {
            _controller = controller;
            _provier = provider;
            _checkReceipt = func;
        }

        /// <summary>
        /// リストアが必要な商品があればリストアを行う.
        /// </summary>
        /// <param name="restoreTransactionsCallback">トランザクション処理時のコールバック.</param>
        public IAPBuyFailureReason RestoreIfNeeded(Action restoreTransactionsCallback) {
            Log.Notice("【IAP】 リストア処理が必要な商品あるか確認します");
            var apple = _provier.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) => {
                Log.Notice(("【IAP】 リストア処理済み: " + result + ". この部分のログ以降でIAP.ProcessPurchase関数の処理が出ていなければリストアされるものはない."));
                restoreTransactionsCallback();
            });

            return IAPBuyFailureReason.None;
        }
    }
}
