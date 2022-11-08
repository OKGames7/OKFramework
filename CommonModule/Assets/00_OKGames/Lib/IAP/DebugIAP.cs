using OKGamesFramework;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using Cysharp.Threading.Tasks;

namespace OKGamesLib {

    /// <summary>
    /// アプリ内課金のアクセスポイント.
    /// 非消費型のみ対応している.
    /// </summary>
    public class DebugIAP : IIAP, IStoreListener {

        /// <summary>
        /// Purchasingシステム.
        /// </summary>
        private IStoreController _storeController = null;

        /// <summary>
        /// Purchsasingの拡張システム.
        /// </summary>
        private IExtensionProvider _storeProvider = null;

        /// <summary>
        /// 各ストア用の課金システムのコンポジット.
        /// </summary>
        private IPlatformStoreIAP _storeIAP = null;

        // 初期化ステータス.
        private IAPInitializeState _initState = IAPInitializeState.NotInitialization;

        /// <summary>
        /// <see cref="IIAP.SuccessEvent"/>.
        /// </summary>
        private IIAP.SuccessEvent _onPurchaseSuccessResult;

        /// <summary>
        /// <see cref="IIAP.FailureEvent"/>.
        /// </summary>
        private IIAP.FailureEvent _onPurcharseFailureResult;

        /// <summary>
        /// <see cref="IIAP.PurchaseDeferredEvent"/>.
        /// </summary>
        private IIAP.PurchaseDeferredEvent _onPurcharseDeferredResult;


        /// <summary>
        /// 初期化がうまくいけば<see cref="_storeController"/>と<see cref="_storeProvider"/>が格納される
        /// それらが格納されているかで初期化済みか判断している.
        /// </summary>
        /// <returns>初期化完了していたらtrue.</returns>
        public bool IsInit() {
            return (_storeController != null) && (_storeProvider != null);
        }

        /// <summary>
        /// 通信接続があるかどうか.
        /// </summary>
        /// <returns>通信接続があればtrue.</returns>
        private bool HasNetworkConnection() {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        /// <summary>
        /// <see cref="IIAP.InitAsync"/>.
        /// </summary>
        public async UniTask<IAPInitializeState> InitAsync(StoreItem[] items) {
            if (IsInit()) {
                return _initState;
            }

            if (!HasNetworkConnection()) {
                Log.Warning("【IAP】通信接続がありません.");
                return IAPInitializeState.Failure;
            }

            Log.Notice("【DebugIAP】初期化開始.");

            _storeIAP = new EditorIAP();

            // 初期化処理.
            InitializePurchasing(items);

            // 初期化が成功するか失敗するまで待機する.
            await UniTask.WaitUntil(() => IsInit() || (_initState == IAPInitializeState.Failure));

            Log.Notice("【DebugIAP】初期化終了.");

            return _initState;
        }

        /// <summary>
        /// IAP初期化処理.
        /// </summary>
        private void InitializePurchasing(StoreItem[] items) {
            if (IsInit()) {
                Log.Notice("【DebugIAP】 すでに初期化済みです");
                return;
            }

            _initState = IAPInitializeState.Initializing;

            var module = StandardPurchasingModule.Instance();
            module.useFakeStoreAlways = true;
            module.useFakeStoreUIMode = FakeStoreUIMode.DeveloperUser;
            var builder = ConfigurationBuilder.Instance(module);
            if (Application.platform == RuntimePlatform.Android) {
                // Androidのみコンビニ決済方法による課金がある.
                // コンビニ決済を選んだ場合のコールバックを設定する.
                var googleStoreConfigration = builder.Configure<IGooglePlayConfiguration>();
                googleStoreConfigration?.SetDeferredPurchaseListener(OnPurchaseDeferred);
            }

            _storeIAP.InitializePurchasing(items, this, builder);
        }

        /// <summary>
        /// <see cref="IIAP.OnInitialized"/>
        /// </summary>
        public void OnInitialized(IStoreController controller, IExtensionProvider provier) {
            Log.Notice("【DebugIAP】 OnInitialized: PASS");

            _storeController = controller;
            _storeProvider = provier;

            _storeIAP.OnInitialized(_storeController, _storeProvider);
            _initState = IAPInitializeState.Success;
        }

        /// <summary>
        /// <see cref="IIAP.OnInitializeFailed"/>
        /// </summary>
        public void OnInitializeFailed(InitializationFailureReason error) {
            _storeIAP.OnInitializeFailed(error);

            _initState = IAPInitializeState.Failure;
        }

        /// <summary>
        /// <see cref="IIAP.GetProductsPossibleBuy"/>
        /// </summary>
        public Product[] GetProductsPossibleBuy() {
            return IsInit() ?
                // 非消費型は一度購入していたら二度目の購入はさせないため含めない.
                _storeController.products.all
                    .ToList()
                    .Where((x) => (x.definition.type == ProductType.NonConsumable) && x.hasReceipt)
                    .ToArray()
                : null;
        }

        /// <summary>
        /// <see cref="IIAP.GetProducts"/>
        /// </summary>
        public Product[] GetProducts() {
            return IsInit() ? _storeController.products.all : null;
        }

        /// <summary>
        /// <see cref="IIAP.GetProductsWithState"/>
        /// </summary>
        public (Product[] Products, PurchaseState[] PurchaseStates)? GetProductsWithState() {
            if (!IsInit()) {
                return null;
            }

            var products = _storeController.products.all;
            var states = products.ToList()
                .Select((x) => (x.hasReceipt ? _storeIAP.CheckReceipt(x.receipt) : PurchaseState.NotPurchased))
                .ToArray();
            return (products, states);
        }

        /// <summary>
        /// <see cref="IIAP.GetProduct"/>
        /// </summary>
        public Product GetProduct(string productID) {
            return IsInit() ? _storeController.products.WithID(productID) : null;
        }

        /// <summary>
        /// <see cref="IIAP.BuyProductWithID"/>
        /// </summary>
        public IAPBuyFailureReason BuyProductWithID(string productID, IIAP.SuccessEvent successResult, IIAP.FailureEvent failureEvent, IIAP.PurchaseDeferredEvent purchaseDeferredEvenet) {
            // アプリが強制終了しても処理続行するためにtryを使っている.
            try {
                if (!IsInit()) {
                    var reason = IAPBuyFailureReason.Unknown;
                    switch (_initState) {
                        case IAPInitializeState.NotInitialization:
                        case IAPInitializeState.Failure:
                            reason = IAPBuyFailureReason.NotInitialization;
                            break;
                        case IAPInitializeState.Initializing:
                            reason = IAPBuyFailureReason.Initializing;
                            break;
                    }
                    return reason;
                }

                Product product = _storeController.products.WithID(productID);
                if ((product == null) || (!product.availableToPurchase)) {
                    // 購入できないアイテムの場合.
                    return IAPBuyFailureReason.UnknownItem;
                }

                if (!HasNetworkConnection()) {
                    // 通信不可の場合は何もしない.
                    return IAPBuyFailureReason.NetworkUnavailbale;
                }

                _onPurchaseSuccessResult = successResult;
                _onPurcharseFailureResult = failureEvent;
                _onPurcharseDeferredResult = purchaseDeferredEvenet;

                Log.Notice(string.Format("【DebugIAP】 Purchasing product asychronously: '{0}' - '{1}'", product.definition.id, product.definition.storeSpecificId));
                _storeController.InitiatePurchase(product);

                return IAPBuyFailureReason.None;
            }
            catch (Exception e) {
                Log.Warning("【DebugIAP】 BuyProductID: FAIL.Exception during purchase. :" + e);
                _onPurchaseSuccessResult = null;
                _onPurcharseFailureResult = null;
                _onPurcharseDeferredResult = null;
                return IAPBuyFailureReason.Unknown;
            }
        }

        /// <summary>
        /// <see cref="IIAP.ProcessPurchase"/>
        /// <summary>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {

            string productID = args.purchasedProduct.definition.id;
            Product product = args.purchasedProduct;

            if ((_onPurchaseSuccessResult == null) || (_onPurcharseFailureResult == null)) {
                // コールバックが通知できない場合はここで処理を終える.
                return PurchaseProcessingResult.Complete;
            }

            // アプリが強制終了しても処理続行するためにtryを使っている.
            try {
                // 未登録のアイテムの除外はしない(過去に購入した現在は販売していないアイテムが未消費の可能性があるため)

                // ストアからの正しいレシートか確認(不正なレシートなら例外処理が走る.
                var purchaseState = _storeIAP.CheckReceipt(product.receipt);
                // アイテムの購入完了処理.
                _onPurchaseSuccessResult.Invoke(product, purchaseState);
            }
            catch (Exception) {
                // 不明なエラーが発生(成功のコールバックで強制終了している場合はここで通知されるが、ここに入った商品についてはレシートの有無で判断できる.)
                _onPurcharseFailureResult.Invoke(product, PurchaseFailureReason.Unknown);
            }

            _onPurchaseSuccessResult = null;
            _onPurcharseFailureResult = null;
            _onPurcharseDeferredResult = null;
            return PurchaseProcessingResult.Complete;
        }

        /// <summary>
        /// <see cref="IIAP.OnPurchaseFailed"/>
        /// </summary>
        /// <param name="product">購入失敗した商品.</param>
        /// <param name="failureReason">失敗理由.</param>
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
            _storeIAP.OnPurchaseFailed(product, failureReason);

            _onPurcharseFailureResult?.Invoke(product, failureReason);
            _onPurchaseSuccessResult = null;
            _onPurcharseFailureResult = null;
            _onPurcharseDeferredResult = null;
        }

        /// <summary>
        /// 遅延課金をした際のコールバック.
        /// </summary>
        /// <param name="product"></param>
        private void OnPurchaseDeferred(Product product) {
            Log.Notice($"【DebugIAP】 OnPurchaseDeferred id: {product.definition.id}");
            _onPurcharseDeferredResult?.Invoke(product);
            _onPurchaseSuccessResult = null;
            _onPurcharseFailureResult = null;
            _onPurcharseDeferredResult = null;
        }

        /// <summary>
        /// <see cref="IIAP.RestoreIfNeeded"/>
        /// </summary>
        public IAPBuyFailureReason RestoreIfNeeded(IIAP.SuccessEvent successResult, IIAP.FailureEvent failureResult, Action notPendingResult) {
            // Editorでは対象外.
            return IAPBuyFailureReason.NotSupported;
        }
    }
}
