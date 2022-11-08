using Cysharp.Threading.Tasks;
using System;

namespace OKGamesLib {

    /// <summary>
    /// テキストを制御するために必要なクラスの注入や設定を仲介するアダプター.
    /// </summary>
    public class TextAdapter : ITextAdapter {

        /// <summary>
        /// シーン全体のテキストを監視制御するクラス.
        /// </summary>
        private ITextWatcher _watcher;

        /// <summary>
        /// 設定言語.
        /// </summary>
        private Language _language;

        /// <summary>
        /// フォント関連を取得するローダー.
        /// </summary>
        private IFontLoader _fontLoader;

        /// <summary>
        /// テキストマスター.
        /// </summary>
        private Entity_text _textMaster;

        private bool _isInject = false;

        public void Inject(IUITransfer transfer, IFontLoader loader) {
            _watcher = new TextWatcher();

            _language = transfer.Lang;
            _fontLoader = loader;
            _textMaster = transfer.TextMaster;

            _isInject = true;
        }

        public async UniTask Setup(TextWrapper wrapper) {
            // 初期化を待つ.
            await UniTask.WaitUntil(() => _isInject);

            _watcher.Add(wrapper);

            // 依存性の注入.
            wrapper.Inject(_fontLoader, _textMaster);
            wrapper.SetLanguage(_language);
            try {
                // テキストのセットアップ.
                await wrapper.Setup().AttachExternalCancellation(wrapper.CancelTokenSource.Token);
            }
            catch (OperationCanceledException) {
                // テキストラッパーのセットアップ途中にシーン破棄が走った場合にここを通る.
                // UniTaskのキャンセル処理は例外処理扱いでしか処理できないがエラー扱いはしたくない(想定内挙動）のためログのみを出すようにしている.
                Log.Notice("【TextAdapter】Setup TextWapper is cancel.");
            }
        }

        public void Remove(TextWrapper wrapper) {
            _watcher.Remove(wrapper);
        }

        public void UpdateTexts(Language lang) {
            _language = lang;
            _watcher.UpdateTexts(_language);
        }
    }
}
