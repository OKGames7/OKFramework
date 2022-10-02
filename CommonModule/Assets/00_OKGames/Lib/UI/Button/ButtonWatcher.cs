using OKGamesLib;
using System.Collections.Generic;
using UniRx;

namespace OKGamesLib {

    /// <summary>
    /// 全ボタンの状況を監視し任意のボタンの状態変化をさせる.
    /// </summary>
    public class ButtonWatcher : IButtonWatcher {

        /// 現在シーン上に存在する<see cref = "CustomButton"/>のリスト(DontDestoryなものも含む).
        private List<ButtonWrapper> wrapperList = new List<ButtonWrapper>();

        /// <summary>
        /// ボタン押下時のイベントが走っている最中の数.
        /// </summary>
        public static int RunningProcessNum => _runningProcessNum;
        private static int _runningProcessNum = 0;

        public void Add(ButtonWrapper wrapper) {
            wrapperList.Add(wrapper);
        }

        public void Bind(ButtonWrapper wrapper) {
            wrapper.Button.OnClickAsObservable()
                .Subscribe(_ => {
                    SetAllButtonsInteractable(false);
                })
                .AddTo(wrapper);

            wrapper.IsRunningProcess
                .SkipLatestValueOnSubscribe() // 初期値の設定時は無視する.
                .Subscribe(
                    isRunning => {
                        if (isRunning) {
                            ++_runningProcessNum;
                        } else {
                            --_runningProcessNum;
                            if (_runningProcessNum < 1) {
                                // ボタン押下時の処理がされているボタンがなくなったら全てのボタンのinteractableをONにする.
                                SetAllButtonsInteractable(true);
                            }
                        }
                    }).AddTo(wrapper);
        }

        /// <summary>
        /// リストの全てのボタンコンポーネントのintaractableを設定する.
        /// </summary>
        /// <param name="isActive">活性化させるかどうか.</param>
        private void SetAllButtonsInteractable(bool isActive) {
            foreach (var button in wrapperList) {
                button.SetInteractable(isActive);
            }
        }

        public void Remove(ButtonWrapper button) {
            wrapperList.Remove(button);
        }
    }
}
