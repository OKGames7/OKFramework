using OKGamesFramework;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace OKGamesLib {

    /// <summary>
    /// 共通ダイアログの管理クラス.
    /// ダイアログの生成をFactoryクラスを経由して行う
    /// 複数ダイアログのライフサイクル管理と描画順の制御を行う
    /// </summary>
    public class CommonDialog : MonoBehaviour, IDialog {

        // ダイアログ管理機構のrootのGameObject.
        // 使用しない時は描画負荷節約のために非表示にしておく.
        [SerializeField] private GameObject _self;

        public GameObject ViewRoot => _viewRoot;
        // 実際にダイアログのViewを生成したらここにぶら下げて描画制御を行う(新しいものほど上に表示されるように).
        [SerializeField] private GameObject _viewRoot;

        // ダイアログより描画優先度の低いものにユーザー操作介入させないようのマスク.
        [SerializeField] private Image _maskImage;

        private Entity_text _textMaster;

        private CommonDialogPool _pool;

        // 表示中のダイアログの数.
        private int _openCounter = 0;

        private IPrev _prev;


        /// <summary>
        /// <see cref="IDialog.InitAsync"/>
        /// </summary>
        public void Init(IResourceStore resourceStore, Entity_text textMaster, IPrev prev) {
            _textMaster = textMaster;
            _prev = prev;

            _pool = gameObject.GetComponent<CommonDialogPool>();
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

            // プールになかったら作成する、あるならプールのものをgetし際セットアップする.
            await UniTask.WaitUntil(() => _pool.IsSetPool);
            var pool = _pool.GetPool();
            var dialogComposit = pool.Get();
            dialogComposit.gameObject.SetParentAsFirstSibling(_viewRoot);

            Action closeCallback = () => {
                pool.Return(dialogComposit);
                CallbackDialogClose();
            };
            return GetSetupDialogView(dialogComposit, transfer, closeCallback);
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

