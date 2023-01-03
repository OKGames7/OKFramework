using Cysharp.Threading.Tasks;
using System;

namespace OKGamesLib {

    /// <summary>
    /// Admob広告機能へのアクセスを持つクラスのインターフェース.
    /// </summary>
    public interface IAdmob {

        /// <summary>
        /// 初期化済みか.
        /// </summary>
        bool IsInitialized();

        /// <summary>
        /// 初期化処理
        /// </summary>
        UniTask InitAsync();

        /// <summary>
        /// バナーを表示する.
        /// </summary>
        void ShowBanner();

        /// <summary>
        /// バナーを非表示にする.
        /// </summary>
        void HideBanner();

        /// <summary>
        /// インタースティシャル広告をを表示する.
        /// </summary>
        /// <param name="wrapper">広告閉じた際に行うコールバック.</param>
        void ShowInterstitial(Action endCallback);


    }
}
