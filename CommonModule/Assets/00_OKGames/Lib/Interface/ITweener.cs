namespace OKGamesLib {

    /// <summary>
    /// Tweenを扱うクラスのインターフェース.
    /// </summary>
    public interface ITweener {

        /// <summary>
        /// 現在所持しているTweenListの数.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Tweenを生成して、初期化し、TweenListへ追加。さらにそのListを辞書へ追加する.
        /// </summary>
        /// <param name="obj">格納する辞書のkeyとなるオブジェクト(Tweenの対象).</param>
        /// <param name="from">開始値.</param>
        /// <param name="to">終了値.</param>
        /// <param name="duration">開始値から終了値まで変化に何秒かけるか.</param>
        /// <param name="easingFunc">変化曲線の種類.</param>
        /// <param name="onUpdate">Tween処理中にさせる処理.</param>
        void Go(
            object obj, float from, float to, float duration,
            EasingFunc easingFunc,
            TweenCallback onUpdate
        );

        /// <summary>
        /// Tweenを生成し、TweenListへ追加. その後Listを辞書へ格納する.
        /// </summary>
        /// <param name="obj">格納する辞書のkeyとなるオブジェクト(Tweenの対象).</param>
        /// <returns>ITween.</returns>
        ITween NewTween(object obj = null);

        /// <summary>
        /// 辞書のkeyとなるobjのTweenListで所持しているTweenを全て完了させてListの破棄を行う.
        /// </summary>
        /// <param name="obj">Tweenを終了させるオブジェクト.</param>
        void Finish(object obj);

        /// <summary>
        /// 辞書に登録している、TweenList内のTweenと辞書共に全て破棄する.
        /// </summary>
        void ClearAll();
    }
}
