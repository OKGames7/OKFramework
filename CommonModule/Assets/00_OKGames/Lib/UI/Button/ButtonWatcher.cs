using System.Collections.Generic;
using UniRx;
using Cysharp.Threading.Tasks;

namespace OKGamesLib {

    /// <summary>
    /// シーン内の全ボタンの状況を監視する.
    /// </summary>
    public class ButtonWatcher : IButtonWatcher {

        public List<ButtonWrapper> WrapperList => _wrapperList;

        /// 現在シーン上に存在する<see cref = "ButtonWrapper"/>のリスト(DontDestoryなものも含む).
        private List<ButtonWrapper> _wrapperList = new List<ButtonWrapper>();

        /// <summary>
        /// ボタン押下時のイベントが走っている最中の数.
        /// </summary>
        public static int RunningProcessNum => _runningProcessNum;
        private static int _runningProcessNum = 0;

        public void Add(ButtonWrapper wrapper) {
            _wrapperList.Add(wrapper);
        }

        public void Bind(ButtonWrapper wrapper) {

            wrapper.IsRunningProcess
                .SkipLatestValueOnSubscribe() // 初期値の設定時は無視する.
                .Subscribe(
                    isRunning => {
                        if (isRunning) {
                            ++_runningProcessNum;
                            SetAllButtonsInteractable(false);
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
            foreach (var button in _wrapperList) {
                button.SetInteractable(isActive);
            }
        }

        public void Remove(ButtonWrapper button) {
            _wrapperList.Remove(button);
            AdjustRunningProcess();
        }

        /// <summary>
        /// ButtonWrapperが処理の途中で破棄された場合の調整処理.
        /// ボタン押下でシーン遷移をした場合はシーン遷移の処理を全工程を待つ前に破棄されることがあるのでそういった時にこの関数で監視変数を調整する.
        /// </summary>
        private void AdjustRunningProcess() {
            if (_wrapperList.Count < _runningProcessNum) {
                _runningProcessNum = _wrapperList.Count;

                if (_runningProcessNum < 1) {
                    SetAllButtonsInteractable(true);
                }
            }
        }
    }
}
