using System;

namespace OKGamesLib {

    /// <summary>
    /// Actionの蓄積と発火を制御するクラスのインターフェース.
    /// </summary>
    public interface ISignal {
        /// <summary>
        /// 登録されているActionの破棄.
        /// </summary>
        void Clear();
    }
}
