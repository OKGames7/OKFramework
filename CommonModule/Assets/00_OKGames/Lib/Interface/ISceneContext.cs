using Cysharp.Threading.Tasks;
using System.Threading;

namespace OKGamesLib {

    /// <summary>
    /// シーン内のメインループ処理を管理するクラスのインターフェース.
    /// </summary>
    public interface ISceneContext {

        // SceneDataArgs Args;

        /// <summary>
        /// 外部から新たな処理を実行できる状態か.
        /// シーン遷移処理時<see cref="Finalize"/>の処理が終わって<see cref="InitBeforeLoadScene"/>の処理が始まる前にfalseになる.
        ///<see cref="InitAfterLoadScene"/>の処理が終わった直後にtrueになる.
        /// </summary>
        bool IsReady { get; set; }

        /// <summary>
        /// UniTaksの中断用.
        /// </summary>
        CancellationTokenSource CancelTokenSource { get; }

        /// <summary>
        /// シーン名の取得.
        /// </summary>
        /// <returns>シーン名.</returns>
        string SceneName();

        /// <summary>
        /// Addressablesリソースをキャッシュへ追加.
        /// </summary>
        void RetainResource();

        /// <summary>
        /// 自身のシーンへ遷移開始する直前に行う処理.
        /// この処理が終わった直後に次シーンへ遷移する.
        /// </summary>
        /// <returns>UniTask</returns>
        UniTask InitBeforeLoadScene();

        /// <summary>
        /// 自身のシーンへ遷移完了した時に行う処理.
        /// シーン遷移トランザクションの一番最後の部分で呼ばれる.
        /// </summary>
        void OnStartupScene();

        /// <summary>
        /// 自身のシーンへ遷移した直後に行う処理.
        /// </summary>
        /// <returns>UnitTask</returns>
        UniTask InitAfterLoadScene();

        /// <summary>
        /// シーン破棄時に行う処理で、リソース、メモリの解放などを行うこと.
        /// <see cref="ISceneDirector.SceneLoading"/>の処理の直後に呼ばれる.
        /// </summary>
        /// <returns>UniTask</returns>
        UniTask Finalize();

        /// <summary>
        /// デフォルト以外の方法でフェードインさせる場合に用いる.
        /// </summary>
        /// <returns>UniTask</returns>
        UniTask CustomFadeIn();

        /// <summary>
        /// デフォルト以外の方法でフェードアウトさせる場合に用いる.
        /// </summary>
        /// <returns>UniTask</returns>
        UniTask CustomFadeOut();

        /// <summary>
        /// UnityEngine.Update処理.
        /// </summary>
        void Update();
    }
}
