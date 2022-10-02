using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// Objectへ起こす値や値自信の変化(Tween)が複数のTweenで構成されることがあるので、その対応用のList.
    /// </summary>
    public class TweenList {

        /// <summary>
        /// TweenのList.
        /// </summary>
        public List<Tween> tweenList { get; } = new List<Tween>();

        /// <summary>
        /// 保留状態のTweenのリスト.
        /// </summary>
        public List<Tween> pendingTweenList { get; } = new List<Tween>();

        /// <summary>
        /// 全てのTweenが完了しているかどうか.
        /// </summary>
        public bool IsCompleted { get; private set; } = false;

        /// <summary>
        /// いずれかのTweenがUpdate処理中か.
        /// </summary>
        private bool _isUpdating = false;


        /// <summary>
        /// TweenをListに加える.
        /// </summary>
        /// <param name="tween">Listへ追加する<see cref="Tween"/>.</param>
        public void Add(Tween tween) {
            if (_isUpdating) {
                pendingTweenList.Add(tween);
            } else {
                tweenList.Add(tween);
            }

            IsCompleted = false;
        }

        /// <summary>
        /// 外部から呼ばれる毎フレーム行う処理.
        /// </summary>
        /// <param name="deltaTime">deltaTime.</param>
        public void Update(float deltaTime) {
            _isUpdating = true;
            bool containsUnfinished = false;
            foreach (var tween in tweenList) {
                tween.Update(deltaTime);
                if (!tween.IsCompleted()) {
                    containsUnfinished = true;
                }
            }

            IsCompleted = !containsUnfinished;
            _isUpdating = false;

            if (pendingTweenList.Count > 0) {
                tweenList.AddRange(pendingTweenList);
                pendingTweenList.Clear();
                IsCompleted = false;
            }
        }

        /// <summary>
        /// List内のTweenの処理を終わらせる.
        /// </summary>
        public void Finish() {
            foreach (var tween in tweenList) {
                if (!tween.IsCompleted()) {
                    tween.Complete();
                }
            }
        }

        /// <summary>
        /// (Pendingも含め)List内のTweenを破棄する.
        /// </summary>
        public void Clear() {
            tweenList.Clear();
            pendingTweenList.Clear();
        }
    }
}
