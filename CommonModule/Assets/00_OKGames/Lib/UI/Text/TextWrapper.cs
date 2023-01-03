using OKGamesFramework;
using TMPro;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// Text Mesh Proのコンポーネントに対して、設定の上書きをすることができるラッパー.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    [DisallowMultipleComponent]
    public class TextWrapper : MonoBehaviour {

        // デバッグしやすいようにInspectorViewへ出している.
        [SerializeField] private string _key = "";

        [SerializeField] private TextConst.FontType _type = TextConst.FontType.Self;
        [SerializeField] private FontStyles _stryle = FontStyles.Normal;
        [SerializeField] private TextConst.ColorType _colorType = TextConst.ColorType.Normal;

        [SerializeField] private TextAlignmentOptions _alignment = TextAlignmentOptions.Center;

        [SerializeField] private TextMeshProUGUI _text = null;

        private Language _lang = Language.Ja;

        private IFontLoader _loader;
        private Entity_text _textMaster;

        /// <summary>
        /// UniTaksの中断用.
        /// </summary>
        public CancellationTokenSource CancelTokenSource => _cancelTokenSource;
        private readonly CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        private bool _isSetup = false;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private async void Start() {
            // アダプター側でウォッチャーへ登録し、本コンポーネントの設定を行う.
            var adapter = OKGames.Context.UI.TextAdapter;
            await adapter.Setup(this);

            if (!string.IsNullOrEmpty(_key)) {
                // テキストの表示.
                SetTextByKey();
            }
        }

        /// <summary>
        /// 依存性の注入.
        /// </summary>
        /// <param name="loader">フォント関連を取得するローダー.</param>
        /// <param name="textMaster">テキストマスター.</param>
        public void Inject(IFontLoader loader, Entity_text textMaster) {
            _loader = loader;
            _textMaster = textMaster;
        }

        /// <summary>
        /// 現在の変数情報からTextMeshProのコンポーネントへ各種設定を行う.
        /// </summary>
        public async UniTask Setup() {
            if (_type == TextConst.FontType.Auto) {
                // フォント設定.
                TMP_FontAsset font = await _loader.GetFont(_lang);
                // _text.font = font;
                _text.font = font;
            }

            // スタイル設定.
            _text.fontStyle = _stryle;

            // サイズ設定.
            float fontSize = _loader.GetFontSize(_text.fontSize, _lang);
            _text.fontSize = fontSize;

            // オートサイズは処理が重いのと、決めのサイズ指定ができなくなるので使わない.
            _text.enableAutoSizing = false;

            // 折り返しにする.
            _text.enableWordWrapping = true;
            _text.overflowMode = TextOverflowModes.Truncate;

            // 整列設定.
            _text.alignment = _alignment;

            // カラー設定.
            if (_colorType != TextConst.ColorType.Self) {
                Color color = _loader.GetFontColor(_colorType);
                _text.color = color;
            }

            // タグを使用できるようにON.
            _text.richText = true;

            // 文字だけにヒット判定する必要はないのでOFF.
            if (_text.raycastTarget) {
                _text.raycastTarget = false;
            }

            _isSetup = true;
        }

        /// <summary>
        /// TextマスターのIDを取得する.
        /// </summary>
        public string GetTextID() {
            return _key;
        }

        /// <summary>
        /// TextマスターのIDをセットする.
        /// </summary>
        /// <param name="key">Textマスターのキー情報.</param>
        public void SetTextID(string key) {
            _key = key;
        }

        /// <summary>
        /// テキストマスターのキーを基にテキスト表示する.
        /// </summary>
        /// <param name="key"></param>
        public void SetTextByKey(string key = null) {
            SetTextByKeyAsync(key).Forget();
        }

        private async UniTask SetTextByKeyAsync(string key = null) {
            await UniTask.WaitUntil(() => _isSetup);

            if (!string.IsNullOrEmpty(key)) {
                _key = key;
            }

            if (!string.IsNullOrEmpty(_key)) {
                var str = _textMaster.GetText(_key, _lang);
                _text.text = str;
            }
        }

        /// <summary>
        /// テキストを設定する.
        /// </summary>
        /// <param name="text">表示するテキスト.</param>
        public void SetText(string text) {
            _text.text = text;
        }

        /// <summary>
        /// FontのStyle情報をセットする.
        /// </summary>
        /// <param name="style">スタイル.</param>
        public void SetStyle(FontStyles style) {
            _stryle = style;
        }

        /// <summary>
        /// Fontの色タイプをセットする.
        /// </summary>
        /// <param name="colorType">色タイプ.</param>
        public void SetColorType(TextConst.ColorType colorType) {
            _colorType = colorType;
        }

        /// <summary>
        /// Fontの整列情報をセットする.
        /// </summary>
        /// <param name="alignment">整列情報.</param>
        public void SetAlignment(TextAlignmentOptions alignment) {
            _alignment = alignment;
        }

        /// <summary>
        /// テキスト言語をセットする.
        /// </summary>
        /// <param name="lang">言語.</param>
        public void SetLanguage(Language lang) {
            _lang = lang;
        }

        /// <summary>
        /// 破棄時に行う処理.
        /// </summary>
        private void OnDestroy() {
            _cancelTokenSource.Cancel();

            var adapter = OKGames.Context.UI.TextAdapter;
            adapter.Remove(this);

            _text = null;
        }
    }
}
