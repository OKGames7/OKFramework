using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// ダイアログを生成するために必要な情報を移送するためのクラス.
    /// </summary>
    public class DialogTransfer {

        public string PrefabAddress { get; set; } = AssetAddress.AssetAddressEnum.CommonDialogView.ToString();

        public string HeaderText { get; set; } = string.Empty;

        public string BodyText { get; set; } = string.Empty;

        public Action OnClickOkAction => _onClickOk;
        private Action _onClickOk = null;

        public Action OnClickNoAction => _onClickNo;
        private Action _onClickNo = null;

        public Func<UniTask> OnClickOkAsync => _onClickOkAsync;
        private Func<UniTask> _onClickOkAsync = null;

        public Func<UniTask> OnClickNoAsync => _onClickNoAsync;
        private Func<UniTask> _onClickNoAsync = null;

        public string OkButtonText { get; set; } = string.Empty;
        public string NoButtonText { get; set; } = string.Empty;

        public DialogButtonType ButtonType => _buttonType;
        private DialogButtonType _buttonType = DialogButtonType.OKOnly;

        public void SetButtonsData(Action onClickOk, Action onClickNo, string okButtonStr = "", string noButtonStr = "") {
            _buttonType = DialogButtonType.OKAndNO;
            _onClickOk = onClickOk;
            _onClickNo = onClickNo;

            OkButtonText = okButtonStr;
            NoButtonText = noButtonStr;
        }

        public void SetButtonsData(Func<UniTask> onClickOk, Func<UniTask> onClickNo, string okButtonStr = "", string noButtonStr = "") {
            _buttonType = DialogButtonType.OKAndNO;
            _onClickOkAsync = onClickOk;
            _onClickNoAsync = onClickNo;

            OkButtonText = okButtonStr;
            NoButtonText = noButtonStr;
        }

        public void SetButtonsData(Func<UniTask> onClickOk, Action onClickNo, string okButtonStr = "", string noButtonStr = "") {
            _buttonType = DialogButtonType.OKAndNO;
            _onClickOkAsync = onClickOk;
            _onClickNo = onClickNo;

            OkButtonText = okButtonStr;
            NoButtonText = noButtonStr;
        }

        public void SetOKButtonData(Action onClickOk, string okButtonStr = "") {
            _buttonType = DialogButtonType.OKOnly;
            _onClickOk = onClickOk;

            OkButtonText = okButtonStr;
        }

        public void SetOKButtonData(Func<UniTask> onClickOk, string okButtonStr = "") {
            _buttonType = DialogButtonType.OKOnly;
            _onClickOkAsync = onClickOk;

            OkButtonText = okButtonStr;
        }
    }
}
