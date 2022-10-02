using System;
using System.Collections;
using System.Collections.Generic;

namespace OKGamesLib {
    /// <summary>
    /// アクションのスタック、実行を制御するSignalクラスを保持する.
    /// </summary>
    public class SignalRegistry {

        /// <summary>
        /// 登録されているSignalクラスのキャッシュ.
        /// </summary>
        private Dictionary<Type, ISignal> _signalDict = new Dictionary<Type, ISignal>();

        /// <summary>
        /// 登録されているSignalの数.
        /// </summary>
        public int Count { get { return _signalDict.Count; } }

        /// <summary>
        /// <see cref="ISignal"/>を継承したシグナルクラスの処理を取得/作成する.
        /// </summary>
        /// <typeparam name="T"><see cref="ISignal"/>を継承したカスタムクラス.</typeparam>
        /// <returns></returns>
        public T GetOrCreate<T>() where T : ISignal, new() {
            Type signalType = typeof(T);
            ISignal signal;
            if (_signalDict.TryGetValue(signalType, out signal)) {
                return (T)signal;
            }

            signal = (ISignal)Activator.CreateInstance(signalType);
            _signalDict.Add(signalType, signal);
            return (T)signal;
        }


        /// <summary>
        /// シグナルのActionと辞書を破棄する.
        /// </summary>
        public void Clear() {
            foreach (var signal in _signalDict.Values) {
                signal.Clear();
            }
            _signalDict.Clear();
        }
    }
}
