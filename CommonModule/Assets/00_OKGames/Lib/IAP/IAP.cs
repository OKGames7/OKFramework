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
    public class IAP : IIAP, IStoreListener {

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

        private IOSIAPExtention _iOSExtention = null;

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
        /// リストア処理中か.
        /// </summary>
        private bool _isRestoreProcess = false;


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

            Log.Notice("【IAP】初期化開始.");

            if ((Application.platform == RuntimePlatform.IPhonePlayer) || (Application.platform == RuntimePlatform.OSXPlayer)) {
                _storeIAP = new IOSIAP();
            } else if (Application.platform == RuntimePlatform.Android) {
                _storeIAP = new AndroidIAP();
            } else {
                Log.Warning("【IAP】未対応なプラットフォームです.");
            }
            // 初期化処理.
            InitializePurchasing(items);

            // 初期化が成功するか失敗するまで待機する.
            await UniTask.WaitUntil(() => IsInit() || (_initState == IAPInitializeState.Failure));

            Log.Notice("【IAP】初期化終了.");

            return _initState;
        }

        /// <summary>
        /// IAP初期化処理.
        /// </summary>
        private void InitializePurchasing(StoreItem[] items) {
            if (IsInit()) {
                Log.Notice("【IAP】 すでに初期化済みです");
                return;
            }

            _initState = IAPInitializeState.Initializing;

            var module = StandardPurchasingModule.Instance();
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
            Log.Notice("【IAP】 OnInitialized: PASS");

            _storeController = controller;
            _storeProvider = provier;

            if ((Application.platform == RuntimePlatform.IPhonePlayer) || (Application.platform == RuntimePlatform.OSXPlayer)) {
                // iOSのみリストアを手動でする必要がある.
                // リストアの実処理が記載されている拡張クラスを生成する.
                _iOSExtention = new IOSIAPExtention(_storeController, _storeProvider, _storeIAP.CheckReceipt);
            }

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
                // 非消費型は一度購入していたら二度目の購入はさせないため、それ以外の条件でフィルターする.
                _storeController.products.all
                    .ToList()
                    .Where((x) => !((x.definition.type == ProductType.NonConsumable) && x.hasReceipt))
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


                Log.Notice(string.Format("【IAP】 Purchasing product asychronously: '{0}' - '{1}'", product.definition.id, product.definition.storeSpecificId));
                _storeController.InitiatePurchase(product);

                return IAPBuyFailureReason.None;
            }
            catch (Exception e) {
                Log.Warning("【IAP】 BuyProductID: FAIL.Exception during purchase. :" + e);
                _onPurchaseSuccessResult = null;
                _onPurcharseFailureResult = null;
                _onPurcharseDeferredResult = null;
                return IAPBuyFailureReason.Unknown;
            }
        }

        /// <summary>
        /// <see cref="IIAP.ProcessPurchase"/>
        /// 1. IStoreController.InitiatePurchaseが成功したら呼ばれる.
        /// 2. AndroidではUnityPurchasing.Initializeの処理が成功後に未完了の商品があれば呼ばれる.
        /// 3. iOSではIAppleExtensions.RestoreTransactinosで未完了の商品があれば呼ばれる.
        /// </summary>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {

            Log.Notice("【IAP】ProcessPurchase開始");

            string productID = args.purchasedProduct.definition.id;
            Product product = args.purchasedProduct;

            if ((_onPurchaseSuccessResult == null) || (_onPurcharseFailureResult == null)) {
                // コールバックが通知できない場合はここで処理を終える.
                Log.Notice("【IAP】成功/失敗のコールバックが設定されていない");
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
                Log.Error("【IAP】失敗.");
                _onPurcharseFailureResult.Invoke(product, PurchaseFailureReason.Unknown);
            }

            if (!_isRestoreProcess) {
                // リストア中は個々に本関数が呼ばれるためコールバックは使い回すため破棄しない.
                _onPurchaseSuccessResult = null;
                _onPurcharseFailureResult = null;
                _onPurcharseDeferredResult = null;
            }

            Log.Notice("【IAP】ProcessPurchase終了");
            return PurchaseProcessingResult.Complete;
        }

        /// <summary>
        /// <see cref="IIAP.OnPurchaseFailed"/>
        /// </summary>
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
            Log.Notice($"【IAP】 OnPurchaseDeferred id: {product.definition.id}");
            _onPurcharseDeferredResult?.Invoke(product);
            _onPurchaseSuccessResult = null;
            _onPurcharseFailureResult = null;
            _onPurcharseDeferredResult = null;
        }

        /// <summary>
        /// <see cref="IIAP.RestoreIfNeeded"/>
        /// </summary>
        public IAPBuyFailureReason RestoreIfNeeded(IIAP.SuccessEvent successResult, IIAP.FailureEvent failureResult, Action restoreTransactionEndEvent) {

            if (!(Application.platform == RuntimePlatform.IPhonePlayer) ||
                Application.platform == RuntimePlatform.OSXPlayer) {
                // リストアが必要なのはiOSのみなのでそれ以外は対象外として返す.
                return IAPBuyFailureReason.NotSupported;
            }

            if (!IsInit()) {
                return IAPBuyFailureReason.NotInitialization;
            }

            if (!HasNetworkConnection()) {
                return IAPBuyFailureReason.NetworkUnavailbale;
            }

            _isRestoreProcess = true;
            _onPurchaseSuccessResult = successResult;
            _onPurcharseFailureResult = failureResult;

            Action action = () => {
                // 呼び出し元で処理するリストア処理が終わった時のイベント.
                restoreTransactionEndEvent();
                // リストア処理中フラグをオフにする
                _isRestoreProcess = false;
                // コールバックのリセット.
                _onPurchaseSuccessResult = null;
                _onPurcharseFailureResult = null;
                _onPurcharseDeferredResult = null;
            };

            return _iOSExtention.RestoreIfNeeded(action);
        }
    }
}
