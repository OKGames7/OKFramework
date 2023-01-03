using OKGamesLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace OKGamesLib {
    /// <summary>
    /// 複数のオブジェクトプールの生成、状態管理、取得を管理する.
    /// </summary>
    public class ObjectPoolRegistry {

        /// <summary>
        /// プールのキャッシュ.
        /// </summary>
        private Dictionary<Type, IObjectPool> _poolDict = new Dictionary<Type, IObjectPool>();

        /// <summary>
        /// オブジェクトのプールを生成しプールのキャッシュへつい以下する.
        /// </summary>
        /// <typeparam name="T"><see cref="PoolableBehaviour"/>を継承したクラス.</typeparam>
        /// <param name="original">プールさせたいオブジェクト.</param>
        /// <param name="reserveNum">プールさせたい数.</param>
        /// <param name="parent">プールしたオブジェクトの親階層とさせたいオブジェクト.</param>
        /// <returns>オブジェクトのプール.</returns>
        public ObjectPool<T> CreatePool<T>(GameObject original, int reserveNum, GameObject parent) where T : PoolableBehaviour {
            Type behaviourType = typeof(T);
            CheckMultipleCreate(behaviourType);

            var objectPool = new ObjectPool<T>(original, reserveNum, parent);
            _poolDict.Add(behaviourType, objectPool);
            return objectPool;
        }

        /// <summary>
        /// オブジェクトのプールを取得する.
        /// </summary>
        /// <typeparam name="T">取得したいプールのクラス.</typeparam>
        /// <returns>オブジェクトのプール.</returns>
        public ObjectPool<T> GetPool<T>() where T : PoolableBehaviour {
            Type behaviourType = typeof(T);
            IObjectPool objectPool;
            if (!_poolDict.TryGetValue(behaviourType, out objectPool)) {
                throw new Exception($"[ObjectPoolRegistry] Object Pool not initialized : {behaviourType}");
            }
            return (ObjectPool<T>)objectPool;
        }

        /// <summary>
        /// オブジェクトのプールとキャッシュを削除する.
        /// </summary>
        public void Clear() {
            foreach (var pool in _poolDict.Values) {
                pool.Clear();
            }
            _poolDict.Clear();
        }

        /// <summary>
        /// デバッグ用.
        /// 重複して生成していないか確認する.
        /// </summary>
        /// <param name="behaviourType">><see cref="PoolableBehaviour"/>を継承したクラス.</param>
        [Conditional("DEVELOPMENT")]
        private void CheckMultipleCreate(Type behaviourType) {
            if (_poolDict.ContainsKey(behaviourType)) {
                Log.Warning($"[ObjectPoolRegistry] Multiple creation detected : {behaviourType}");
            }
        }
    }
}
