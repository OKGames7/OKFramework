using UnityEngine;

namespace OKGamesLib {
    /// <summary>
    /// 起動時に行う設定処理を持つクラスのインターフェース.
    /// </summary>
    public interface IBootConfig {
        /// <summary>
        /// bgmの発生器のプール数.
        /// </summary>
        int numBgmSourcePool { get; }

        /// <summary>
        /// seの発生器のプール数.
        /// </summary>
        int numSeSourcePool { get; }

        /// <summary>
        /// 3Dサウンドを使用するか.
        /// </summary>
        bool useGlobalAudioListener { get; }

        /// <summary>
        /// 起動時(プレイ開始直後)に行う設定処理.
        /// </summary>
        void OnGameBoot();
    }
}
