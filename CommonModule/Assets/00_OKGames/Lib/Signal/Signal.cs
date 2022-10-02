using System;

namespace OKGamesLib {
    /// <summary>
    ///引数を伴わないActionの蓄積と発火を制御するクラス.
    /// </summary>
    public abstract class Signal : ISignal {
        /// <summary>
        /// スタックしているアクション(複数登録可能).
        /// </summary>
        private Action _callbacks;

        /// <summary>
        /// スタックするアクション(複数登録可能).
        /// アクションはOneShot実行すれば忘れさられる.
        /// </summary>
        private Action _oneShotCallbacks;

        /// <summary>
        /// アクションをスタックする.
        /// </summary>
        /// <param name="action"><see cref="_callback"/>へ追加登録するAction.</param>
        public void Connect(Action action) {
            _callbacks += action;
        }

        /// <summary>
        /// アクションをスタックする.
        /// </summary>
        /// <param name="action"><see cref="_oneShotCallbacks"/>へ追加登録するAction.</param>
        public void ConnectOnce(Action action) {
            _oneShotCallbacks += action;
        }

        /// <summary>
        /// 指定したアクションをスタックしたアクションから無くす.
        /// </summary>
        /// <param name="action">蓄積から除きたいアクション.</param>
        public void Disconnect(Action action) {
            _callbacks -= action;
            _oneShotCallbacks -= action;
        }

        /// <summary>
        /// スタックしていたアクションを全て実行する.
        /// </summary>
        public void Emit() {
            _callbacks?.Invoke();
            _oneShotCallbacks?.Invoke();
            _oneShotCallbacks = null;
        }

        /// <summary>
        /// スタックしていたアクションを削除する.
        /// </summary>
        public void Clear() {
            _callbacks = null;
            _oneShotCallbacks = null;
        }
    }

    /// <summary>
    ///引数を1つ伴うActionの蓄積と発火を制御するクラス.
    /// </summary>

    public abstract class Signal<T> : ISignal {
        private Action<T> _callbacks;

        private Action<T> _oneShotCallbacks;

        public void Connect(Action<T> action) {
            _callbacks += action;
        }

        public void ConnectOnce(Action<T> action) {
            _oneShotCallbacks += action;
        }

        public void Disconnect(Action<T> action) {
            _callbacks -= action;
            _oneShotCallbacks -= action;
        }

        public void Emit(T arg) {
            _callbacks?.Invoke(arg);
            _oneShotCallbacks?.Invoke(arg);
            _oneShotCallbacks = null;
        }

        public void Clear() {
            _callbacks = null;
            _oneShotCallbacks = null;
        }
    }

    /// <summary>
    ///引数を2つ伴うActionの蓄積と発火を制御するクラス.
    /// </summary>

    public abstract class Signal<T1, T2> : ISignal {
        private Action<T1, T2> _callbacks;
        private Action<T1, T2> _oneShotCallbacks;

        public void Connect(Action<T1, T2> action) {
            _callbacks += action;
        }

        public void ConnectOnce(Action<T1, T2> action) {
            _oneShotCallbacks += action;
        }

        public void Disconnect(Action<T1, T2> action) {
            _callbacks -= action;
            _oneShotCallbacks -= action;
        }

        public void Emit(T1 arg1, T2 arg2) {
            _callbacks?.Invoke(arg1, arg2);
            _oneShotCallbacks?.Invoke(arg1, arg2);
            _oneShotCallbacks = null;
        }

        public void Clear() {
            _callbacks = null;
            _oneShotCallbacks = null;
        }
    }
}
