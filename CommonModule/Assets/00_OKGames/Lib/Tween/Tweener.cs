using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// Tween管理の大元.
    /// 複数のTweenの生成、状態、実行を管理する.
    /// </summary>
    /// <remarks>
    /// オブジェクトを指定しなかった場合はnullオブジェクトに登録されたとみなしてまとめて管理される.
    /// Tweenはオブジェクト単位で動作中のものが1つもなくなったタイミングでまとめて削除される.
    /// そのため現状の実装では、常に新しいTweenが生み出され常時１つ以上のTWEENが存在している場合に
    /// TWEENリストが肥大化する可能性があるので注意.
    /// </remarks>
    public class Tweener : ITweener {

        public int Count {
            get { return _tweenDict.Count; }
        }

        /// <summary>
        /// 全体のTween格納用の辞書.
        /// </summary>
        private Dictionary<object, TweenList> _tweenDict = new Dictionary<object, TweenList>();

        /// <summary>
        /// 対象のObjectがなくても値の変化だけでもTweenは処理可能なので、その際に使用する用のダミーObject.
        /// </summary>
        private static object _nullObject = new System.Object();


        public void Go(
            object obj, float from, float to, float duration,
            EasingFunc easingFunc,
            TweenCallback onUpdate
        ) {
            var tween = new Tween(from, to, duration, easingFunc, onUpdate);
            tween.Init();
            AddTween(obj, tween);
        }

        public ITween NewTween(object obj = null) {
            var tween = new Tween();
            AddTween(obj, tween);
            return tween;
        }

        /// <summary>
        /// 外部から実行用の毎フレーム行う処理.
        /// </summary>
        /// <param name="deltaTime">deltaTime.</param>
        public void Update(float deltaTime) {
            if (Count == 0) {
                return;
            }

            bool containsCompleted = false;
            foreach (var tweenList in _tweenDict.Values) {
                tweenList.Update(deltaTime);
                if (tweenList.IsCompleted) {
                    containsCompleted = true;
                }
            }

            if (!containsCompleted) {
                return;
            }

            var keys = _tweenDict.Keys.Where(key => _tweenDict[key].IsCompleted).ToArray();
            foreach (var key in keys) {
                _tweenDict.Remove(key);
            }
        }

        public void Finish(object obj) {
            object dictKey = (obj != null) ? obj : _nullObject;
            TweenList tweenList;
            if (!_tweenDict.TryGetValue(dictKey, out tweenList)) {
                return;
            }

            tweenList.Finish();
            tweenList.Clear();
            _tweenDict.Remove(dictKey);
        }

        public void ClearAll() {
            foreach (var tweenList in _tweenDict.Values) {
                tweenList.Clear();
            }
            _tweenDict.Clear();
        }

        /// <summary>
        /// 指定した<see cref="object"/>をkeyに指定した<see cref="Tween"/>をListと辞書へ登録する.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tween"></param>
        private void AddTween(object obj, Tween tween) {
            TweenList tweenList;
            object dictKey = (obj != null) ? obj : _nullObject;
            if (_tweenDict.TryGetValue(dictKey, out tweenList)) {
                tweenList.Add(tween);
                return;
            }

            tweenList = new TweenList();
            tweenList.Add(tween);
            _tweenDict.Add(dictKey, tweenList);
        }
    }
}
