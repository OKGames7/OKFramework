using OKGamesLib;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UniRx;

namespace OKGamesTest {

    /// <summary>
    /// IAPのテスト用のシーンのモデル.
    /// </summary>
    public class TestIAPModel : ITestIAPModel {

        private IIAP _iap;

        /// <summary>
        /// <see cref="ITestIAPModel.Prodcts"/>.
        /// </summary>
        public IReadOnlyReactiveCollection<ProductWrapper> Prodcts => _products;
        private ReactiveCollection<ProductWrapper> _products;

        /// <summary>
        /// <see cref="ITestIAPModel.IsProcessingPurchase"/>.
        /// </summary>
        public bool IsProcessingPurchase => _isProcessingPurchase;
        private bool _isProcessingPurchase = false;

        public ProductWrapper BoughtProduct => _boughtProduct;
        private ProductWrapper _boughtProduct;

        /// <summary>
        /// <see cref="ITestIAPModel.Init"/>.
        /// </summary>
        public void Init() {
            _products = new ReactiveCollection<ProductWrapper>();
            _isProcessingPurchase = false;
            _boughtProduct = null;
        }

        /// <summary>
        /// <see cref="ITestIAPModel.Inject"/>.
        /// </summary>
        public void Inject(IIAP iap) {
            _iap = iap;
        }

        /// <summary>
        /// <see cref="ITestIAPModel.Setup"/>.
        /// </summary>
        public void Setup() {
            _products.Clear();

            var tuple = _iap.GetProductsWithState();
            if (tuple == null) {
                Log.Warning("productsが存在しないです.");
                return;
            }

            var products = tuple.Value.Products;
            var states = tuple.Value.PurchaseStates;

            for (int i = 0; i < products.Length; ++i) {
                var productWrapper = new ProductWrapper(products[i], states[i]);
                _products.Add(productWrapper);
            }
        }

        /// <summary>
        /// <see cref="ITestIAPModel.BuyProduct"/>.
        /// </summary>
        public void BuyProduct(string productID) {
            _isProcessingPurchase = true;
            _boughtProduct = null;
            var failureReason = _iap.BuyProductWithID(productID, PurchaseSuccessCallback, PurchaseFailedCallback, PurchaseDeferredCallback);
            if (failureReason != IAPBuyFailureReason.None) {
                // InitiatePurchase以前の処理で失敗していたら.
                _isProcessingPurchase = false;
            }
        }

        /// <summary>
        /// 商品購入成功時のコールバック.
        /// </summary>
        /// <param name="product">購入した商品.</param>
        /// <param name="purchaseState">購入状態.</param>
        private void PurchaseSuccessCallback(Product product, PurchaseState purchaseState) {
            _boughtProduct = null;
            _boughtProduct = new ProductWrapper(product, purchaseState);
            _isProcessingPurchase = false;
        }

        /// <summary>
        /// 商品遅延購入時のコールバック.
        /// </summary>
        /// <param name="product">購入した商品.</param>
        private void PurchaseDeferredCallback(Product product) {
            _boughtProduct = null;
            _boughtProduct = new ProductWrapper(product, PurchaseState.Pending);
            _isProcessingPurchase = false;
        }

        /// <summary>
        /// 商品購入失敗時のコールバック.
        /// </summary>
        /// <param name="product">購入失敗した商品.</param>
        /// <param name="reason">失敗理由.</param>
        private void PurchaseFailedCallback(Product product, PurchaseFailureReason reason) {
            Log.Warning($"【TestIAPModel】課金購入失敗: 理由 :{reason}");
            _isProcessingPurchase = false;
        }

        /// <summary>
        /// 購読対象の商品情報を更新する.
        /// </summary>
        /// <param name="product"></param>
        public void Update(ProductWrapper product) {
            int index = _products.ToList().FindIndex((x) => (x.Product.definition.id == product.Product.definition.id));
            _products[index] = product;
        }

        /// <summary>
        /// <see cref="ITestIAPModel.RestoreIfNeeded"/>.
        /// </summary>
        public void RestoreIfNeeded() {
            _isProcessingPurchase = true;

            // 商品をリストアした際のコールバック.
            IIAP.SuccessEvent restorePurchaseCallback = (Product product, PurchaseState state) => {
                Log.Notice($"【TestIAPModel】手動リストアした商品: product:  {product.definition.id}, state: {state} ");
                PurchaseSuccessCallback(product, state);
                Update(_boughtProduct);
            };
            // 商品をリストアした際のコールバック.
            IIAP.FailureEvent restoreFailureCallback = (Product product, PurchaseFailureReason state) => {
                Log.Notice($"【TestIAPModel】手動リストア失敗した商品: product:  {product}, failureReason: {state} ");
            };
            // リストア処理は成功したがPending中アイテムがなかった場合の処理.
            Action restoreTransactionsCallback = () => {
                Log.Notice($"【TestIAPModel】手動リストアのTransaction処理.");
                _isProcessingPurchase = false;
            };
            var status = _iap.RestoreIfNeeded(restorePurchaseCallback, restoreFailureCallback, restoreTransactionsCallback);

            if (status != IAPBuyFailureReason.None) {
                // 失敗した場合.
                Log.Notice($"【TestIAPModel】手動リストア失敗しました.: 理由 {status} ");
                _isProcessingPurchase = false;
                return;
            }
        }

        /// <summary>
        /// <see cref="ITestIAPModel.Dispose"/>.
        /// </summary>
        public void Dispose() {
            _iap = null;

            _boughtProduct = null;
            _products = null;
        }
    }
}
