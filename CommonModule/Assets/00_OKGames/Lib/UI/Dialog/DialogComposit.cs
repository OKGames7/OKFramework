using OKGamesFramework;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace OKGamesLib {

    /// <summary>
    /// ダイアログ共通のコンポジット.
    /// </summary>
    public class DialogComposit : PoolableBehaviour, IDialogComposit {

        // ダイアログ部(マスクは含まない).
        [SerializeField] private Transform _dialogTransform;

        // テキスト類.
        [SerializeField] private TextWrapper _headerText;
        [SerializeField] private TextWrapper _bodyText;

        // ボタン類.
        // OKボタンとNOボタンはLayoutGroupで配置しており、片方が非表示になればもう片方の配置は中央揃えになるようにしている.
        [SerializeField] private ButtonWrapper _okButton;
        [SerializeField] private ButtonWrapper _noButton;

        private ITween _showTween;
        private ITween _closeTween;

        private Action _callbackOpen;
        private Action _callbackClose;

        // 開く/閉じるアニメーションの時間(単位: s)
        private readonly float _intervalAnimation = 0.05f;

        private Dictionary<ButtonWrapper, Func<UniTask>> _buttonFuncAsyncMap = new Dictionary<ButtonWrapper, Func<UniTask>>();

        private readonly string _okButtonTextKey = "DIALOG_OK";
        private readonly string _noButtonTextKey = "DIALOG_NO";

        private IPrev _prev;

        /// <summary>
        /// <see cref="PoolableBehaviour.OnGetFromPool">.
        /// </summary>
        public override void OnGetFromPool() {
            // ダイアログ上テキストの表示をリセット.
            _headerText.SetText("");
            _bodyText.SetText("");

            // ボタン内テキストの表示をリセット.
            _okButton.SetText("");
            _noButton.SetText("");
            // クリックイベントを削除.
            _okButton.RemoveAllClickActions();
            _noButton.RemoveAllClickActions();

            _buttonFuncAsyncMap.Clear();
        }

        /// <summary>
        /// <see cref="IDialogComposit.Setup">.
        /// </summary>
        public void Setup(DialogTransfer transfer, Entity_text textMaster, IPrev prev) {
            _prev = prev;

            // ヘッダー部のテキスト設定.
            SetText(_headerText, transfer.HeaderText);
            // ボディー部のテキスト設定.
            SetText(_bodyText, transfer.BodyText);

            string setText = transfer.OkButtonText;
            string buttonText = setText != "" ? setText : textMaster.GetText(_okButtonTextKey);
            // ボタン表示の設定.
            switch (transfer.ButtonType) {
                case DialogButtonType.OKOnly:
                    // OKボタンの設定.
                    SetButton(_okButton, buttonText, transfer.OnClickOkAsync, transfer.OnClickOkAction);
                    // Noボタンは非表示.
                    _noButton.gameObject.SetActive(false);
                    break;
                case DialogButtonType.OKAndNO:
                    // OKボタンの設定.
                    SetButton(_okButton, buttonText, transfer.OnClickOkAsync, transfer.OnClickOkAction);
                    // NOボタンの設定.
                    setText = transfer.NoButtonText;
                    buttonText = setText != "" ? setText : textMaster.GetText(_noButtonTextKey);
                    SetButton(_noButton, buttonText, transfer.OnClickNoAsync, transfer.OnClickNoAction);
                    break;
            }
        }

        /// <summary>
        /// 指定したTextWrapperに文字列をセットする.
        /// </summary>
        /// <param name="textWrapper">表示先のTextWrapper.</param>
        /// <param name="str">表示する文字列.</param>
        private void SetText(TextWrapper textWrapper, string str) {
            // 表示する文字列が空文字の場合は表示不要とみなしている.
            bool enable = str != null;
            if (enable) {
                textWrapper.SetText(str);
            }
            textWrapper.gameObject.SetActive(enable);
        }

        /// <summary>
        /// 指定したButtonWrapperにクリック時アクションを設定する.
        /// Asyncの方にイベントがあればそちらを優先して設定する.
        /// </summary>
        /// <param name="buttonWrapper">ボタン押下でイベントを起こす元のボタン.</param>
        /// <param name="buttonStr">ボタン内テキスト.</param>
        /// <param name="callbackAsync">ボタン押下時の非同期処理.</param>
        /// <param name="callback">ボタン押下時の処理.</param>
        private void SetButton(ButtonWrapper buttonWrapper, string buttonStr, Func<UniTask> callbackAsync, Action callback) {
            buttonWrapper.SetText(buttonStr);

            Func<UniTask> func = null;
            if (callbackAsync != null) {
                func = async () => {
                    await callbackAsync();
                    await Close();
                };
            } else if (callback != null) {
                func = async () => {
                    callback();
                    await Close();
                };
            } else {
                Debug.LogWarning("【DialogComposit】Not setup buttons event.");
                return;
            }

            buttonWrapper.SetClickActionAsync(func);
            buttonWrapper.gameObject.SetActive(true);
        }

        /// <summary>
        /// <see cref="IDialogComposit.Open">.
        /// </summary>
        public async UniTask Open() {

            _prev.Push(() => { Close(true).Forget(); });

            _callbackOpen?.Invoke();

            // ダイアログの表示.
            bool isEnd = false;
            _showTween = OKGames.Tween(_dialogTransform.gameObject)
                //   0f = Scaleが0.0f, 0.0f, 0.0f.
                // 1.0f = Scaleが1.0f, 1.0f, 1.0f.
                .FromTo(0f, 1.0f, _intervalAnimation)
                .OnUpdate(size => {
                    if (_dialogTransform == null) {
                        isEnd = true;
                        return;
                    }
                    _dialogTransform.SetLocalScale(size);
                })
                .OnComplete(_ => {
                    isEnd = true;
                });

            await UniTask.WaitUntil(() => isEnd);
        }

        /// <summary>
        /// <see cref="IDialogComposit.Close">.
        /// </summary>
        public async UniTask Close(bool fromBackKey = false) {
            // ダイアログの非表示.
            bool isEnd = false;
            float currentSize = _dialogTransform.localScale.x;
            _closeTween = OKGames.Tween(_dialogTransform.gameObject)
                // 1.0f = Scaleが1.0f, 1.0f, 1.0f.
                //   0f = Scaleが0.0f, 0.0f, 0.0f.
                .FromTo(currentSize, 0f, _intervalAnimation)
                .OnUpdate(size => {
                    if (_dialogTransform == null) {
                        isEnd = true;
                        return;
                    }
                    _dialogTransform.SetLocalScale(size);
                })
                .OnComplete(_ => {
                    isEnd = true;
                });

            await UniTask.WaitUntil(() => isEnd);

            if (!fromBackKey) {
                // UIで閉じた場合は登録したprevのstackをpopしておく.
                _prev.Pop();
            }

            _callbackClose?.Invoke();
        }

        /// <summary>
        /// <see cref="IDialogComposit.SetOpenCallback">.
        /// </summary>
        public void SetOpenCallback(Action callback) {
            _callbackOpen = callback;
        }

        /// <summary>
        /// <see cref="IDialogComposit.SetCloseCallback">.
        /// </summary>
        public void SetCloseCallback(Action callback) {
            _callbackClose = callback;
        }

        /// <summary>
        /// 破棄時の処理.
        /// </summary>
        private void OnDestroy() {
            _buttonFuncAsyncMap = null;

            _dialogTransform = null;
            _okButton = null;
            _noButton = null;

            _showTween = null;
            _closeTween = null;
        }
    }
}
