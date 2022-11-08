using OKGamesFramework;
using System.Collections.Generic;
using UniRx;
using Cysharp.Threading.Tasks;

namespace OKGamesLib {

    /// <summary>
    /// ボタンを制御するために必要なクラスの注入や設定を仲介するアダプター.
    /// </summary>
    public class ButtonAdapter : IButtonAdapter {

        /// <summary>
        /// シーン全体のボタンを監視制御するクラス.
        /// </summary>
        private IButtonWatcher _watcher;

        /// <summary>
        /// アセット取得用のストア.
        /// </summary>
        private IResourceStore _store;

        /// <summary>
        /// SE再生プレイヤー.
        /// </summary>
        private IAudioPlayer _sePlayer;

        private bool _isInject = false;


        public void Inject(IUITransfer transfer) {
            _watcher = new ButtonWatcher();

            _store = transfer.ResourceStore;
            _sePlayer = transfer.SePlayer;

            _isInject = true;
        }

        public async UniTask Setup(ButtonWrapper wrapper) {
            // 依存性注入されるまで待つ.
            await UniTask.WaitUntil(() => _isInject);

            wrapper.Setup(_store, _sePlayer);

            _watcher.Add(wrapper);
            _watcher.Bind(wrapper);
        }

        public void Remove(ButtonWrapper button) {
            _watcher.Remove(button);
        }
    }
}
