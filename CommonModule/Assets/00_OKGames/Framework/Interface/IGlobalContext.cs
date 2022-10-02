using OKGamesLib;
using Cysharp.Threading.Tasks;

namespace OKGamesFramework {

    /// <summary>
    /// フレームワークに関するグローバルな情報を制御するクラスのインターフェース.
    /// </summary>
    public interface IGlobalContext {

        ISceneDirector SceneDirector { get; }

        IUI UI { get; }

        ITimeKeeper TimeKeeper { get; }

        IResourceStore ResourceStore { get; }

        IUserDataStore UserDataStore { get; }

        IAudioPlayer BgmPlayer { get; }

        IAudioPlayer SePlayer { get; }

        ISignalHub SignalHub { get; }

        ITweenerHub TweenerHub { get; }

        IObjectPoolHub ObjectPoolHub { get; }

        /// <summary>
        /// 非同期で済む部分の初期化処理.
        /// 外部から呼び出す.
        /// </summary>
        void Init();

        /// <summary>
        /// 同期が必要な部分の初期化処理.
        /// 外部から呼び出す.
        /// </summary>
        /// <param name="bootConfig">起動時設定する内容</param>
        UniTask InitAsync(IBootConfig bootConfig = null);
    }
}
