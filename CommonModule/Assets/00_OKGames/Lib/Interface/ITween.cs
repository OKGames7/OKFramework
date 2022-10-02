using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OKGamesLib {

    /// <summary>
    /// fromからtoの値へ変化させる際、Funcのカーブ曲線に応じた指定経過時間の地点を返すデリゲート.
    /// </summary>
    /// <param name="passedTime">経過時間.</param>
    /// <returns></returns>
    public delegate float EasingFunc(float passedTime);

    /// <summary>
    /// Tween処理のコールバック.
    /// </summary>
    /// <param name="value">その地点でのtweenの値</param>
    public delegate void TweenCallback(float value);

    /// <summary>
    /// 汎用的なTween処理のインターフェース.
    /// </summary>
    public interface ITween {

        /// <summary>
        /// 状態の変化をさせる.
        /// </summary>
        /// <param name="from">開始値.</param>
        /// <param name="to">終了値.</param>
        /// <param name="duration">開始値から終了値まで何秒かけるか.</param>
        /// <param name="easingFunc">変化曲線の種類</param>
        /// <returns>ITween</returns>
        ITween FromTo(float from, float to, float duration, EasingFunc easingFunc = null);

        /// <summary>
        /// Tween挙動中に毎フレーム何かを処理する.
        /// </summary>
        /// <param name="onUpdate">処理内容.</param>
        /// <returns>ITween</returns>
        ITween OnUpdate(TweenCallback onUpdate);

        /// <summary>
        /// Tween挙動が終了した地点で何かを処理する.
        /// </summary>
        /// <param name="onComplete">処理内容.</param>
        /// <returns>ITween</returns>
        ITween OnComplete(TweenCallback onComplete);

        /// <summary>
        /// iTweenの処理を遅延させる.
        /// </summary>
        /// <param name="delaySec">遅延させる秒数.</param>
        /// <returns></returns>
        ITween Delay(float delaySec);

        /// <summary>
        /// TweenのIsCompletedが呼ばれるまで待機する.
        /// </summary>
        /// <param name="autoCancelOnSceneChange"></param>
        /// <returns>UniTask.</returns>
        UniTask Async(bool autoCancelOnSceneChange = true);

        /// <summary>
        /// アルファをTweenで変化させる.
        /// </summary>
        /// <param name="graphic">変化させるグラフィック.</param>
        /// <returns>ITween.</returns>
        ITween SetAlpha(Graphic graphic);

        /// <summary>
        /// アルファをTweenで変化させる.
        /// </summary>
        /// <param name="canvasGroup">変化させるCanvasのグループ.></param>
        /// <returns>ITween.</returns>
        ITween SetAlpha(CanvasGroup canvasGroup);
    }
}
