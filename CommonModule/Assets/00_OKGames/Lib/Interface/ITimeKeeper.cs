using Cysharp.Threading.Tasks;
using System;

namespace OKGamesLib {
    /// <summary>
    /// プレイ中全体の時間系統の処理を統括するクラスのインターフェース.
    /// </summary>
    public interface ITimeKeeper {

        /// <summary>
        ///  Time.deltaTimeのラッパー.
        /// </summary>
        float dt { get; }

        /// <summary>
        /// シーンでの経過時間.
        /// </summary>
        float t { get; }

        /// <summary>
        /// 一定時間待つ
        /// </summary>
        /// <param name="seconds">待機する秒数.</param>
        /// <returns>UniTask</returns>
        UniTask Wait(float seconds);

        /// <summary>
        /// 一定時間待って処理を行う.
        /// </summary>
        /// <param name="seconds">待機する秒数.</param>
        /// <param name="action">指定した秒数を待ってから行う処理.</param>
        /// <returns>UniTask</returns>
        UniTask Wait(float seconds, Action action);

        /// <summary>
        /// 一定フレーム待つ.
        /// </summary>
        /// <param name="frame">待機するフレーム.</param>
        /// <returns>UniTask</returns>
        UniTask WaitFrame(int frame);

        /// <summary>
        /// 一定時間待って処理を行う.
        /// </summary>
        /// <param name="seconds">待つ秒数.</param>
        /// <param name="action">行う処理.</param>
        void Delay(float seconds, Action action);

        /// <summary>
        /// 一定フレーム待つ.
        /// actionが引数にあれば一定フレーム後にactionを処理する.
        /// </summary>
        /// <param name="frame">待つフレーム数.</param>
        /// <param name="action">行う処理.</param>
        void DelayFrame(int frame, Action action = null);
    }
}
