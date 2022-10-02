using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OKGamesLib {

    /// <summary>
    /// パーティクル用のプール可能なカスタムクラス.
    /// </summary>
    public class PoolableParticleSystem : PoolableBehaviour {

        public ParticleSystem Particle;

        public override void OnCreate() {
            Particle = GetComponent<ParticleSystem>();
        }

        /// <summary>
        /// パーティクルの再生がsトップした際に呼ばれるイベント.
        /// </summary>
        private void OnParticleSystemStopped() {
            ReturnToPool();
        }
    }
}
