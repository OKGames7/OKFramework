using UnityEngine;
using UniRx;

namespace OKGamesLib {

    // ---------------------------------------------------------
    // タップエフェクト機能の処理.
    // ---------------------------------------------------------
    public class TapEffect : MonoBehaviour {

        // 描画順がゲーム内で一番上の専用のカメラを用意する.
        [SerializeField]
        private Camera _camera = null;
        // タップエフェクトで使用するパーティクル.
        [SerializeField]
        private ParticleSystem _particle = null;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Start() {
            _particle.Stop();

            InputEventProvider.OnTap
            // タップされたら
            .Subscribe(_ => {
                // タップエフェクトの再生.
                Play();
            })
            .AddTo(this);
        }

        /// <summary>
        /// タップエフェクトの再生.
        /// </summary>
        private void Play() {
            _particle.transform.position = _camera.ScreenToWorldPoint(Input.mousePosition + _camera.transform.forward * 10);
            _particle.Emit(1);
        }
    }
}
