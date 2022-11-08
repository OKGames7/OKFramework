using OKGamesFramework;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace OKGamesLib {
    /// <summary>
    /// ボタンのサウンドを再生するためのクラス.
    /// </summary>
    public class ButtonSound : MonoBehaviour {

        /// <summary>
        /// サウンドのタイプ.
        /// </summary>
        private enum Type {
            OK,
            NG,
        }

        /// <summary>
        /// 設定するサウンドのタイプ
        /// </summary>
        [SerializeField] private Type _type;

        private IAudioPlayer _sePlaer;
        private IResourceStore _resourceStore;

        /// <summary>
        /// 初期化.
        /// </summary>
        /// <param name="button">紐付けするボタン.</param>
        /// <returns>UniTask,</returns>
        public async UniTask Init(Button button, IAudioPlayer sePlayer, IResourceStore store) {
            // サウンドデータのロード.
            _sePlaer = sePlayer;
            _resourceStore = store;

            string address = _type == Type.OK ? AssetAddress.AssetAddressEnum.common_ok.ToString() : AssetAddress.AssetAddressEnum.common_ng.ToString();
            if (!_resourceStore.Contains(address)) {
                // まだTextマスターを取得していなかったらAddressablesから取得.
                string[] addresses = new string[1] { address };
                await _resourceStore.RetainGlobalWithAutoLoad(addresses);
            }

            // クリック時にSE再生するようにバインド.
            button.OnPointerDownAsObservable()
                .Where(_ => button.interactable)
                .Subscribe(_ => PlaySE(address))
                .AddTo(button);
        }

        /// <summary>
        /// SEを再生する.
        /// </summary>
        /// <param name="address">再生するSEのAudioClipのAddressablesのアドレス.</param>
        private void PlaySE(string address) {
            // 再生するSEの取得.
            AudioClip clip = _resourceStore.GetAudio(address);

            // SEの再生.
            _sePlaer.Play(clip);
        }
    }
}
