using OKGamesLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace OKGamesLib {
    /// <summary>
    /// オブジェクトをプール処理するためのクラス.
    /// </summary>
    public class ObjectPool<T> : IObjectPool where T : PoolableBehaviour {

        /// <summary>
        /// プール数.
        /// </summary>
        public int ReservedNum { get; private set; } = 0;

        /// <summary>
        /// プール元のオブジェクト.
        /// </summary>
        private GameObject _original;

        /// <summary>
        /// プールの実態はStackで管理している.
        /// </summary>
        private Stack<T> _pool = new Stack<T>();

        /// <summary>
        /// 実際にプールしている数.
        /// </summary>
        public int RemainCount {
            get { return _pool.Count; }
        }

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="original">プールするオブジェクト.</param>
        /// <param name="reserveNum">プールしたい数.</param>
        /// <param name="parent">生成したオブジェクトの親階層.</param>
        public ObjectPool(GameObject original, int reserveNum = 64, GameObject parent = null) {
            _original = original;
            Reserve(reserveNum, parent);
        }

        /// <summary>
        /// オブジェクトのプールを生成する.
        /// </summary>
        /// <param name="num">プールしたい数.</param>
        /// <param name="parent">生成したオブジェクトの親階層.</param>
        public void Reserve(int num, GameObject parent = null) {
            for (var i = 0; i < num; ++i) {
                var obj = Create();
                if (parent != null) {
                    obj.gameObject.SetParentAsFirstSibling(parent);
                }
                Return(obj);
            }
        }

        /// <summary>
        /// プールから<see cref="PoolableBehaviour"/>を継承したクラスを取得する.
        /// プールがまだなければ生成し取得する.
        /// </summary>
        /// <returns>プールから取得したGameObejectに紐づくクラス.</returns>
        public T Get() {
            T obj;
            if (_pool.Count > 0) {
                obj = _pool.Pop();
            } else {
                obj = Create();
                Log.Notice($"[ObjectPool] Pool ({typeof(T)}) is empty"
                        + $" (now total is <color=#{Log.COLOR_WARN}>{ReservedNum}</color>)");
            }

            obj.gameObject.SetActive(true);
            obj.OnGetFromPool();
            return obj;
        }

        /// <summary>
        /// プールがあればプールから<see cref="PoolableBehaviour"/>を継承したクラスを取得する.
        /// </summary>
        /// <returns>プールから取得したGameObejectに紐づくクラス.</returns>
        public T GetIfAvaliable() {
            if (_pool.Count == 0) {
                return null;
            }
            return Get();
        }

        /// <summary>
        /// 指定したオブジェクトを非活性にしてプールに戻す.
        /// </summary>
        /// <param name="obj"></param>
        public void Return(T obj) {
            CheckMultipleReturn(obj);
            obj.gameObject.SetActive(false);
            obj.OnReturnToPool();
            _pool.Push(obj);
        }

        public void Return(PoolableBehaviour obj) {
            Return(obj as T);
        }
        public void Clear() {
            _pool.Clear();
        }

        /// <summary>
        /// <see cref="_original"/>のオブジェクトを新規生成する.
        /// 生成直後の処理(OnCreate)の実行後、プールにセットもする.
        /// プール数のカウンタも増やす.
        /// </summary>
        /// <returns>プールしたオブジェクトのクラス.</returns>
        private T Create() {
            var newObj = GameObject.Instantiate<GameObject>(_original);
            var obj = newObj.GetComponent<T>();
            obj.OnCreate();
            obj.SetPool(this);
            ++ReservedNum;

            return obj;
        }

        /// <summary>
        /// デバッグ用.
        /// 重複してreturnさせていないか確認する.
        /// </summary>
        /// <param name="obj"></param>
        [Conditional("DEVELOPMENT")]
        private void CheckMultipleReturn(T obj) {
            if (_pool.Contains(obj)) {
                Log.Warning($"[ObjectPool] Multiple return detected : {typeof(T)}");
            }
        }
    }
}
