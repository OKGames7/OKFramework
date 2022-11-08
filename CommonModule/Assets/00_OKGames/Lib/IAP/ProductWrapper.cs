using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using Cysharp.Threading.Tasks;

namespace OKGamesLib {

    /// <summary>
    /// IAPのProductのラッパー.
    /// </summary>
    public class ProductWrapper {

        public Product Product => _product;
        private Product _product;

        public PurchaseState PurchaseState => _purchaseState;
        private PurchaseState _purchaseState;

        public ProductWrapper(Product product, PurchaseState purchaseState) {
            _product = product;
            _purchaseState = purchaseState;
        }

        /// <summary>
        /// ファイナライザ.
        /// </summary>
        ~ProductWrapper() {
            _product = null;
        }
    }
}
