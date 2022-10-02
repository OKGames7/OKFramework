using OKGamesLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OKGamesLib {
    /// <summary>
    /// Pool可能なクラス.
    /// </summary>
    public class PoolableBehaviour : MonoBehaviour {

        protected IObjectPool _pool;

        /// <summary>
        /// プールへ返す.
        /// </summary>
        protected void ReturnToPool() {
            if (_pool == null) {
                Log.Warning("[PoolableBehaviour] Pool is not set.");
                Destroy(gameObject);
                return;
            }

            _pool.Return(this);
        }

        /// <summary>
        /// プールへセットする.
        /// </summary>
        /// <param name="pool">セットするオブジェクト.</param>
        internal void SetPool(IObjectPool pool) {
            _pool = pool;
        }

        /// <summary>
        /// インスタンス生成した直後にさせたい処理.
        /// </summary>
        public virtual void OnCreate() {
        }

        /// <summary>
        /// プールから取得した直後にさせたい処理.
        /// </summary>
        public virtual void OnGetFromPool() {
        }

        /// <summary>
        /// プールへ戻した直後にさせたい処理.
        /// </summary>
        public virtual void OnReturnToPool() {
        }
    }
}
