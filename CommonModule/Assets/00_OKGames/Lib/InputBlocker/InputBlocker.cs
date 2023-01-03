using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// ユーザーの入力ブロック状態を管理をするクラス.
    /// </summary>
    public class InputBlocker : IInputBlocker {

        private int _processCounter = 0;

        /// <summary>
        /// <see cref="IInputBlocker.AddBusyProcess"/>
        /// </summary>
        public void AddBusyProcess() {
            ++_processCounter;
        }

        /// <summary>
        /// <see cref="IInputBlocker.ReduceBusyProcess"/>
        /// </summary>
        public void ReduceBusyProcess() {
            --_processCounter;
        }

        /// <summary>
        /// <see cref="IInputBlocker.IsBlocking"/>
        /// </summary>
        public bool IsBlocking() {
            // プロセス中の処理があればブロック中とみなす.
            return _processCounter > 0;
        }
    }
}
