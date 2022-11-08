using OKGamesLib;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesFramework {

    /// <summary>
    /// SceneContextの処理を指示/制御するクラスのインターフェース.
    /// </summary>
    public interface ISceneDirector {
        /// <summary>
        /// 現在所持しているSceneContext.
        /// </summary>
        ISceneContext CurrentSceneContext { get; }

        /// <summary>
        /// シーン遷移中かどうか.
        /// </summary>
        bool IsInTransition { get; }

        /// <summary>
        /// 次シーンへ遷移する処理でフェードアウトした直後に行うイベント.
        /// <see cref="IsInTransition"/>がtrueにし、画面をフェードアウトして以降<see cref="ISceneContext.Finalize"/>が呼ばれる前に呼ばれる.
        /// </summary>
        event Action SceneLoading;

        /// <summary>
        /// 次シーンへの遷移が完了したら行うイベント.
        /// <see cref="ISceneContext.InitAfterLoadScene"/>よりも後に呼ばれる.
        /// </summary>
        event Action SceneLoaded;

        /// <summary>
        /// UnityEngin.Updatedで処理させるアクション.
        /// </summary>
        event Action SceneUpdate;

        /// <summary>
        /// 外部から行う初期化.
        /// </summary>
        /// <param name="bootConfig">起動時設定処理を持つクラス.</param>
        /// <param name="resourceStore">AddressablesのリソースStore</param>
        /// <param name="fadeScreenRootObject">フェード管理するオブジェクトのrootオブジェクト</param>
        /// <returns>UniTask</returns>
        void Init(IBootConfig bootConfig, IResourceStore resourceStore, GameObject fadeScreenRootObject);

        /// <summary>
        /// 起動直後のシーン初期化.
        /// </summary>
        /// <returns>UniTask.</returns>
        UniTask InitSceneAsync();

        /// <summary>
        /// 次シーンへ遷移する。
        /// </summary>
        /// <param name="nextSceneContext">遷移先のSceneContext</param>
        /// <param name="fadeOutTime">フェードアウトにかける時間</param>
        /// <param name="fadeInTime">フェードインにかける時間</param>
        /// <returns>UniTask</returns>
        UniTask GoToNextScene(ISceneContext nextSceneContext, float fadeOutTime = 0.3f, float fadeInTime = 0.3f);

        /// <summary>
        /// 次シーンへ遷移する。
        /// </summary>
        /// <param name="nextSceneName">遷移先のシーン名</param>
        /// <param name="fadeOutTime">フェードアウトにかける時</param>
        /// <param name="fadeInTime">フェードインにかける時間</param>
        /// <returns>UniTask</returns>
        UniTask GoToNextScene(string nextSceneName, float fadeOutTime = 0.3f, float fadeInTime = 0.3f);

        /// <summary>
        /// デフォルトの遷移以外にカスタムで次シーンへ遷移させたい場合の処理.
        /// </summary>
        /// <param name="nextSceneContext">遷移先のSceneContext</param>
        /// <returns>UniTask</returns>
        UniTask GoToNextSceneWithCustomTransition(ISceneContext nextSceneContext);

        /// <summary>
        ///デフォルトの遷移以外にカスタムで次シーンへ遷移させたい場合の処理.
        /// </summary>
        /// <param name="nextSceneName">遷移先のシーン名</param>
        /// <returns>Unitask</returns>
        UniTask GoToNextSceneWithCustomTransition(string nextSceneName);
    }
}
