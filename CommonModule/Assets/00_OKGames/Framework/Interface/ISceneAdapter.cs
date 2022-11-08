using Cysharp.Threading.Tasks;
using System.Threading;

namespace OKGamesFramework {

    /// <summary>
    /// シーンのアダプタのインターフェース.
    /// </summary>
    public interface ISceneAdapter {

        /// <summary>
        /// UniTaksの中断用.
        /// </summary>
        CancellationTokenSource CancelTokenSource { get; }

        /// <summary>
        /// 自身のシーンへ遷移した直後に行う処理.
        /// この時点ではまだIsReadyはfalse.
        /// </summary>
        /// <param name="provier">SceneContextで生成したtoken.</param>
        /// <returns>UnitTask</returns>
        UniTask InitAfterLoadScene(CancellationTokenSource token);

        /// <summary>
        /// 自身のシーンへ遷移完了した時に行う処理.
        /// シーン遷移トランザクションの一番最後の部分で呼ばれる.
        /// この処理が呼ばれる時点でIsReadyフラグはTrueに、フェードも開けている.
        /// </summary>
        void OnStartupScene();

        /// <summary>
        /// シーン破棄時に行う処理で、リソース、メモリの解放などを行うこと.
        /// <see cref="ISceneDirector.SceneLoading"/>の処理の直後に呼ばれる.
        /// </summary>
        /// <returns>UniTask</returns>
        UniTask Finalize();
    }
}
