using System;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesLib {

    // ---------------------------------------------------------
    // プラットフォーム毎のInput処理の違いを吸収し、イベント通知をするクラス.
    // ---------------------------------------------------------
    public sealed class InputEventProvider : MonoBehaviour {

        // タップ判定.
        public static IObservable<Unit> OnTap => _tapSubject;
        private static readonly Subject<Unit> _tapSubject = new Subject<Unit>();

        /// <summary>
        /// 初期化.
        /// </summary>

        private void Start() {
            _tapSubject.AddTo(this);

            var ct = this.GetCancellationTokenOnDestroy();

            // PC: 　 画面を左クリックしたら
            // スマホ: 画面をタップしたら発火.
            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ => { _tapSubject.OnNext(Unit.Default); })
                .AddTo(this);
        }
    }
}
