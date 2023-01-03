using OKGamesFramework;
using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace OKGamesLib {

    /// <summary>
    /// システムダイアログの管理クラス.
    /// ダイアログの生成をFactoryクラスを経由して行う
    /// 複数ダイアログのライフサイクル管理と描画順の制御を行う
    /// </summary>
    public class SystemDialog : MonoBehaviour, IDialog {

        // ダイアログ管理機構のrootのGameObject.
        // 使用しない時は描画負荷節約のために非表示にしておく.
        [SerializeField] private GameObject _self;

        public GameObject ViewRoot => _viewRoot;
        // 実際にダイアログのViewを生成したらここにぶら下げて描画制御を行う(新しいものほど上に表示されるように).
        [SerializeField] private GameObject _viewRoot;

        // ダイアログより描画優先度の低いものにユーザー操作介入させないようのマスク.
        [SerializeField] private Image _maskImage;

        private DialogFactory _factory;

        private IResourceStore _resourceStore;
        private Entity_text _textMaster;

        // 表示中のダイアログの数.
        private int _openCounter = 0;

        private IPrev _prev;

        /// <summary>
        /// <see cref="IDialog.InitAsync"/>
        /// </summary>
        public void Init(IResourceStore resourceStore, Entity_text textMaster, IPrev prev) {
            _resourceStore = resourceStore;
            _textMaster = textMaster;
            _prev = prev;

            _factory = new DialogFactory(_resourceStore);
            _self.SetActive(false);
        }

        /// <summary>
        /// <see cref="IDialog.GenerateCommonDialogView"/>
        /// </summary>
        public async UniTask GenerateDialogView(DialogTransfer transfer) {
            var composit = await SetupDialogView(transfer);
            await composit.Open();
        }

        /// <summary>
        /// <see cref="IDialog.SetupCommonDialogView"/>
        /// </summary>
        public async UniTask<IDialogComposit> SetupDialogView(DialogTransfer transfer) {
            ++_openCounter;
            _self.SetActive(true);

            var dialog = await _factory.InstantiateDialog(transfer);
            // 最も手前に表示される位置に親子づけしている.
            dialog.SetParentAsFirstSibling(_viewRoot);

            var composit = dialog.GetComponent<DialogComposit>();
            Action closeCallback = () => {
                Destroy(composit.gameObject);
                CallbackDialogClose();
            };
            return GetSetupDialogView(composit, transfer, closeCallback);
        }

        /// <summary>
        /// ダイアログ設定してその状態のダイアログを返す.
        /// </summary>
        /// <param name="composit">設定先のダイアログコンポジット.</param>
        /// <param name="transfer">設定に必要なデータがまとまったトランスファー.</param>
        /// <returns>セットアップ済み状態のダイアログ.</returns>
        private IDialogComposit GetSetupDialogView(DialogComposit composit, DialogTransfer transfer, Action closeCallback) {
            composit.Setup(transfer, _textMaster, _prev);
            composit.SetOpenCallback(() => { SetEnableMask(true); });
            composit.SetCloseCallback(closeCallback);

            return composit;
        }

        /// <summary>
        /// ダイアログを閉じた時のコールバック.
        /// </summary>
        private void CallbackDialogClose() {
            --_openCounter;
            if (_openCounter <= 0) {
                _self.SetActive(false);
                SetEnableMask(false);
                _openCounter = 0;
            }
        }

        /// <summary>
        /// ダイアログ後ろを覆う画面全体マスクの表示/非表示.
        /// </summary>
        /// <param name="enable"></param>
        private void SetEnableMask(bool enable) {
            if (_maskImage.enabled == enable) {
                // 状態が一致していれば何もしない.
                return;
            }
            _maskImage.enabled = enable;
        }
    }
}
