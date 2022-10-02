using OKGamesFramework;
using UnityEngine.UI;
using UniRx;
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


        // ボタン内のテキスト.
        public TextWrapper TextWrapper => _textWrapper;
        [SerializeField] private TextWrapper _textWrapper;


        /// <summary>
        /// 初期化処理
        /// watcherへのBind処理をPJ側のボタンイベントの紐付けよりも先のタイミングにし、
        /// ボタンイベント時の最初処理でボタンを押したら全ボタンを非アクティブにさせたいためAwake処理を使用している.
        /// </summary>
        private void Awake() {

            _animation.Init(_button);

            // サウンドは鳴らせる準備が終わるまで待たなくてもOKとするのでawaitしていない.
            _sound.Init(_button).Forget();

            var watcher = OKGames.Context.UI.ButtonWatcher;
            watcher.Add(this);
            watcher.Bind(this);
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
        // /// </summary>
        // protected override void OnDestroy() {
        private void OnDestroy() {
            OKGames.Context.UI.ButtonWatcher.Remove(this);
        }
    }
}
