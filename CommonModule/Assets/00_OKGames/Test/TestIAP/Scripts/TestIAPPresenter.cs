using OKGamesLib;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using System;

namespace OKGamesTest {

    /// <summary>
    /// IAPのテストシーン用のPresnter.
    /// </summary>
    public class TestIAPPresenter : MonoBehaviour, ITestIAPPresenter {

        /// <summary>
        /// モデル.
        /// </summary>
        private ITestIAPModel _model;

        /// <summary>
        /// ビュー.
        /// </summary>
        [SerializeField] private TestIAPView _view;

        /// <summary>
        /// 課金商品マスター.
        /// </summary>
        private Entity_platform_item _master;


        /// <summary>
        /// <see cref="ITestIAPPresenter.Init"/>.
        /// </summary>
        public void Init() {
            _model = new TestIAPModel();
            _model.Init();
        }

        /// <summary>
        /// <see cref="ITestIAPPresenter.Inject"/>.
        /// </summary>
        public void Inject(IIAP iap, IResourceStore store) {
            var master = store.GetObj<Entity_platform_item>(AssetAddress.AssetAddressEnum.platform_items.ToString());
            _master = master;

            _model.Inject(iap);
        }

        /// <summary>
        /// <see cref="ITestIAPPresenter.Bind"/>.
        /// </summary>
        public void Bind() {
            // 課金商品データが追加された際にViewを表示する.
            _model.Prodcts.ObserveAdd()
                .Subscribe(wrapper => {
                    if (!wrapper.Value.Product.availableToPurchase) {
                        return;
                    }
                    var transfer = Convert(wrapper.Value);
                    _view.Create(transfer);
                }).AddTo(this);

            // 課金商品データに変動があった場合はViewの表示を変える.
            _model.Prodcts.ObserveReplace()
                           .Subscribe(wrapper => {
                               var transfer = Convert(wrapper.NewValue);
                               _view.UpdateNode(wrapper.NewValue.Product.definition.id, transfer);
                           }).AddTo(this);

            // リストア処理を行う.
            _view.RestoreButton.SetClickActionAsync(async () => {
                _model.RestoreIfNeeded();
                await UniTask.WaitUntil(() => !_model.IsProcessingPurchase);
            });
        }

        /// <summary>
        /// <see cref="ITestIAPPresenter.SetupIAPItems"/>.
        /// </summary>
        public void SetupIAPItems() {
            _model.Setup();
        }

        /// <summary>
        /// ProductWrapperの情報からViewで必要なデータへ変換する.
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        private TestIAPNodeTransfer Convert(ProductWrapper wrapper) {
            // ストアの商品情報とマスターのレコード情報を取得.
            var productID = wrapper.Product.definition.id;
            var currencyCode = wrapper.Product.metadata.isoCurrencyCode;
            var titleInStore = wrapper.Product.metadata.localizedTitle;
            var descriptionInStore = wrapper.Product.metadata.localizedDescription;
            var priceInStore = wrapper.Product.metadata.localizedPriceString;
            var masterRecord = _master.GetItemByCurrentPlatform(productID);

            // 課金商品購入処理.
            Func<UniTask> buttonFunc = async () => {
                _model.BuyProduct(productID);
                await UniTask.WaitUntil(() => !_model.IsProcessingPurchase);
                if (_model.BoughtProduct != null) {
                    // 購入成功していたら格納されて至るのでリストから削除する.
                    _model.Update(_model.BoughtProduct);
                }
            };

            // Viewで使用するデータ群をまとめたTransfer.
            var tranfer = new TestIAPNodeTransfer(
                productID,
                masterRecord.titleKey,
                masterRecord.discriptionKey,
                currencyCode,
                priceInStore,
                wrapper.PurchaseState,
                buttonFunc
            );

            return tranfer;
        }

        /// <summary>
        /// <see cref="ITestIAPPresenter.Dispose"/>.
        /// </summary>
        public void Dispose() {
            _model.Dispose();
            _model = null;

            _view.Dispose();
            _view = null;

            _master = null;
        }
    }
}
