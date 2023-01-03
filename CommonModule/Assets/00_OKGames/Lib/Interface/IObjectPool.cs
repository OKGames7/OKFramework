using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// オブジェクトをプール処理するためのクラスのインターフェース.
    /// </summary>
    public interface IObjectPool {

        /// <summary>
        /// 指定数オブジェクトを生成する.
        /// </summary>
        /// <param name="num">プールを生成する数.</param>
        void Reserve(int num, GameObject parent);

        /// <summary>
        /// 使用していたGameObjectを非活性にしてプールに返す.
        /// </summary>
        /// <param name="obj">プールに返すGameObjectに紐づけている<see cref="PoolableBehaviour"/>を継承しているクラス.</param>
        void Return(PoolableBehaviour obj);

        /// <summary>
        /// プールを破棄する.
        /// </summary>
        void Clear();
    }
}
