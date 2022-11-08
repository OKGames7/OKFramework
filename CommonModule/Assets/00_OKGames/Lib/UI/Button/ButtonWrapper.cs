using OKGamesFramework;
using UnityEngine.UI;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// デフォルトのButtonコンポーネントだと連打や同時押し防止、そのほか対応されていない事項が多いのでカスタムしている.
    /// Button押下時のイベントは<see cref ="ButtonEvent"/>を利用して仕込むこと.
    /// 鍵(ロック)ボタンとして押せないことを実現するためにinteractableを使用しないこと(その場合は鍵付きのボタンオブジェクトを別途用意すること).
    /// 最初はButtonクラスを継承する方向で進めていたがプレイ時にStartより前のタイミングで何故かOnDestroyが呼ばれるのでButtonコンポーネントをWrapする形にしている.
    /// </summary>
    public class ButtonWrapper : MonoBehaviour {

        /// <summary>
        /// ボタン押下時のイベントが走っている最中か.
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsRunningProcess => _isRunningProcess;
        private readonly ReactiveProperty<bool> _isRunningProcess = new ReactiveProperty<bool>();

        // 機能をラップするボタン.
        public Button Button => _button;
        [SerializeField] private Button _button;

        [SerializeField] private ButtonAnimation _animation;

        [SerializeField] private ButtonSound _sound;

        /// <summary>
        /// UniTaksの中断用.
        /// </summary>
        private readonly CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        /// <summary>
        /// 初期化処理
        /// adapter経由でwatcherへのBind処理をPJ側のボタンイベントの紐付けよりも先のタイミングにし、
        /// ボタンイベント時の最初処理でボタンを押したら全ボタンを非アクティブにさせたいためAwake処理を使用している.
        /// </summary>
        private void Awake() {
            _isRunningProcess.AddTo(this);

            var adapter = OKGames.Context.UI.ButtonAdapter;
            adapter.Setup(this).Forget();
        }

        public void Setup(IResourceStore store, IAudioPlayer sePlayer) {
            _animation.Init(_button);

            // サウンドは鳴らせる準備が終わるまで待たなくてもOKとするのでawaitしていない.
            _sound.Init(_button, sePlayer, store)
                .AttachExternalCancellation(_cancelTokenSource.Token)
                .Forget();
        }

        /// <summary>
        /// ボタン押下時のイベントが走っている最中かのフラグを設定する.
        /// </summary>
        /// <param name="isRunning">処理中かどうか.</param>
        public void SetRunning(bool isRunning) {
            _isRunningProcess.Value = isRunning;
        }

        /// <summary>
        /// ボタンコンポーネントのinteractableを設定する.
        /// </summary>
        /// <param name="isActive">活性化させるかどうか.</param>
        public void SetInteractable(bool isActive) {
            _button.interactable = isActive;
        }

        /// <summary>
        /// 破棄時に行う処理.
        /// </summary>
        private void OnDestroy() {
            _cancelTokenSource.Cancel();

            OKGames.Context.UI.ButtonAdapter.Remove(this);

            _animation = null;
            _sound = null;
            _button = null;
        }
    }
}
