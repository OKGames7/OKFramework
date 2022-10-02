using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// 起動時に行う設定処理を持.
    /// </summary>
    public class DefaultBootConfig : IBootConfig {

        public int numBgmSourcePool => 2;
        public int numSeSourcePool => 16;

        public bool useGlobalAudioListener => true;

        public virtual void OnGameBoot() {

        }
    }
}
