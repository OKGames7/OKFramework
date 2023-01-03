using System;
using Cysharp.Threading.Tasks;

namespace OKGamesLib {

    /// <summary>
    /// フェード表現を管理するクラスのインターフェース.
    /// </summary>
    public interface IFader {

        /// <summary>
        /// 初期化.
        /// </summary>
        void Init();

        /// <summary>
        /// フェードアウトさせる(画面を見せる)
        /// </summary>
        /// <param name="time">フェード時間.</param>
        /// <param name="callback">フェード後にさせる処理.</param>
        /// <returns>UniTask.</returns>
        UniTask FadeOut(float time, Action callback);

        /// <summary>
        /// フェードアウトさせる(画面を見せる)
        /// </summary>
        /// <param name="time">フェード時間.</param>
        /// <returns>UniTask.</returns>
        UniTask FadeOut(float time);

        /// <summary>
        /// フェードインさせる(画面を見えないように覆う)
        /// </summary>
        /// <param name="time">フェード時間.</param>
        /// <param name="callback">フェード後にさせる処理.</param>
        /// <returns>UniTask.</returns>
        UniTask FadeIn(float time, Action callback);

        /// <summary>
        /// フェードインさせる(画面を見えないように覆う)
        /// </summary>
        /// <param name="time">フェード時間.</param>
        /// <returns>UniTask.</returns>
        UniTask FadeIn(float time);
    }
}
