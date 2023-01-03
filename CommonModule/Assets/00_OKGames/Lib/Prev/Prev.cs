using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;


namespace OKGamesLib {

    /// <summary>
    /// 前に戻る機能の管理クラス.
    /// Androidのバックキーにも対応している.
    /// </summary>
    public class Prev : IPrev {

        private IPopNotify _popNotify;

        private IInputBlocker _inputBlocker;

        private Entity_text _textMaster;

        private Stack<Action> _prevActionStack = new Stack<Action>();

        private Func<bool> _validate;

        private readonly int _pushInterval = 1000;
        private bool _didProcessPrev = false;


        /// <summary>
        /// <see cref="IPrev.Inject"/>
        /// </summary>
        public void Inject(IPopNotify popNotify, IInputBlocker inputBlocker, Entity_text textMaster) {
            _popNotify = popNotify;
            _inputBlocker = inputBlocker;
            _textMaster = textMaster;
        }

        /// <summary>
        /// <see cref="IPrev.Bind"/>
        /// </summary>
        public void Bind(GameObject parent) {
            // 連打でバックキーを押されると挙動が保証できないのである程度間隔を空けないと効かないようにする.
            Observable.EveryUpdate()
                .Where(_ => !_didProcessPrev)
                .Subscribe(_ => {
                    CheckRunAndroidBack();
                })
                .AddTo(parent);
        }

        /// <summary>
        /// <see cref="IPrev.Push"/>
        /// </summary>
        public void Push(Action action) {
            _prevActionStack.Push(action);
        }

        /// <summary>
        /// <see cref="IPrev.Pop"/>
        /// </summary>
        public Action Pop() {
            return _prevActionStack.Pop();
        }

        /// <summary>
        /// <see cref="IPrev.Clear"/>
        /// </summary>
        public void Clear() {
            _prevActionStack.Clear();
        }

        /// <summary>
        /// AndroidのBackKeyが押された際に、スタックした前に戻る処理を実行する.
        /// </summary>
        private void CheckRunAndroidBack() {
            if (Application.platform != RuntimePlatform.Android) {
                // Android以外だと何も処理しない.
                return;
            }

            if (!Input.GetKeyDown(KeyCode.Escape)) {
                // バックキーは呼ばれていないので何も処理しない.
                return;
            }

            if (_inputBlocker.IsBlocking()) {
                // システム的にブロック中なら何も処理しない.
                return;
            }

            WaitInterval().Forget();
            if (_validate != null) {
                if (_validate()) {
                    // アプリケーション側で特別に弾きたいケースで弾く.
                    // 例: インゲーム中やチュートリアル中は反応しないようにする等.
                    return;
                }
            }

            if (_prevActionStack.Count <= 0) {
                // Prevするものがない旨を表示する
                var text = _textMaster.GetText("ANDROID_BACK_KEY_NO_STACK");
                _popNotify.ShowCenter(text);
                return;
            }

            var action = _prevActionStack.Pop();
            action.Invoke();

        }

        /// <summary>
        /// 連打防止のためのインターバルを待ってからフラグを下げる.
        /// </summary>
        /// <returns>UniTask.</returns>
        private async UniTask WaitInterval() {
            _didProcessPrev = true;
            await UniTask.Delay(_pushInterval);
            _didProcessPrev = false;
        }

        /// <summary>
        /// <see cref="IPrev.SetApplicationValidate"/>
        /// </summary>
        public void SetApplicationValidate(Func<bool> validate) {
            _validate = validate;
        }
    }
}
