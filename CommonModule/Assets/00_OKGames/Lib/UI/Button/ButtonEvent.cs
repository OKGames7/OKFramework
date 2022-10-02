using OKGamesFramework;
using System;
using Cysharp.Threading.Tasks;
using UniRx;

namespace OKGamesLib {

    /// <summary>
    /// InspectorビューのButtonコンポーネントのOnClick登録の部分は使わない.
    /// コード内以外のところでイベントを仕込んでいる部分があるとバグ発見が遅れるため。
    /// 基本は<see cref="CustomeButton"/>側から本クラスを介してイベント登録し、登録されている内容がイベントに応じて発火される仕組みとしている.
    /// </summary>
    public static class ButtonEvent {

        /// <summary>
        /// バリデーション.
        /// 現在のボタンイベント処理中数が1つ以上あればNG.
        /// </summary>
        /// <returns>ボタンイベント発行して良いか.</returns>
        private static bool Validation() {
            return 0 >= ButtonWatcher.RunningProcessNum;
        }

        /// <summary>
        /// クリックアクション設定(引数なし)
        /// </summary>
        public static void SetClickAction(this ButtonWrapper self, Action _onAction) {
            self.Button.OnClickAsObservable()
                .Subscribe(_ => {
                    if (!Validation()) {
                        return;
                    }
                    self.SetRunning(true);
                    _onAction();
                    self.SetRunning(false);
                })
                .AddTo(self);
        }

        /// <summary>
        /// クリックアクション設定(引数なし, 非同期処理用)
        /// </summary>
        public static void SetClickActionAsync(this ButtonWrapper self, Func<UniTask> _onFunc) {
            self.Button.onClick.AddListener(
                async () => {
                    if (!Validation()) {
                        return;
                    }
                    self.SetRunning(true);
                    await _onFunc();
                    self.SetRunning(false);
                });
        }

        /// <summary>
        /// クリックアクション設定(引数１つ)
        /// </summary>
        public static void SetClickAction<T>(this ButtonWrapper self, Action<T> _onAction, T arg) {
            self.Button.OnClickAsObservable()
                .Subscribe(_ => {
                    if (!Validation()) {
                        return;
                    }
                    self.SetRunning(true);
                    _onAction(arg);
                    self.SetRunning(false);
                })
                .AddTo(self);
        }

        /// <summary>
        /// クリックアクション設定(引数１つ, 非同期処理用)
        /// </summary>
        public static void SetClickActionAsync<T>(this ButtonWrapper self, Func<T, UniTask> _onFunc, T arg) {
            self.Button.onClick.AddListener(
                async () => {
                    if (!Validation()) {
                        return;
                    }
                    self.SetRunning(true);
                    await _onFunc(arg);
                    self.SetRunning(false);
                });
        }

        /// <summary>
        /// クリックアクション設定(引数２つ)
        /// </summary>
        public static void SetClickAction<T1, T2>(this ButtonWrapper self, Action<T1, T2> _onAction, T1 arg1, T2 arg2) {
            self.Button.OnClickAsObservable()
                .Subscribe(_ => {
                    if (!Validation()) {
                        return;
                    }
                    self.SetRunning(true);
                    _onAction(arg1, arg2);
                    self.SetRunning(false);
                })
                .AddTo(self);
        }

        /// <summary>
        /// クリックアクション設定(引数2つ, 非同期処理用)
        /// </summary>
        public static void SetClickActionAsync<T1, T2>(this ButtonWrapper self, Func<T1, T2, UniTask> _onFunc, T1 arg1, T2 arg2) {
            self.Button.onClick.AddListener(
                async () => {
                    if (!Validation()) {
                        return;
                    }
                    self.SetRunning(true);
                    await _onFunc(arg1, arg2);
                    self.SetRunning(false);
                });
        }

        /// <summary>
        /// クリックアクション設定(引数３つ)
        /// </summary>
        public static void SetClickAction<T1, T2, T3>(this ButtonWrapper self, Action<T1, T2, T3> _onAction, T1 arg1, T2 arg2, T3 arg3) {
            self.Button.OnClickAsObservable()
                .Subscribe(_ => {
                    if (!Validation()) {
                        return;
                    }
                    self.SetRunning(true);
                    _onAction(arg1, arg2, arg3);
                    self.SetRunning(false);
                })
                .AddTo(self);
        }

        /// <summary>
        /// クリックアクション設定(引数3つ, 非同期処理用)
        /// </summary>
        public static void SetClickActionAsync<T1, T2, T3>(this ButtonWrapper self, Func<T1, T2, T3, UniTask> _onFunc, T1 arg1, T2 arg2, T3 arg3) {
            self.Button.onClick.AddListener(
                async () => {
                    if (!Validation()) {
                        return;
                    }
                    self.SetRunning(true);
                    await _onFunc(arg1, arg2, arg3);
                    self.SetRunning(false);
                });
        }
    }
}
