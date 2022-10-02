using OKGamesFramework;
using TMPro;
using Cysharp.Threading.Tasks;
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
        [SerializeField] private int _sizeOffset = 0;
        [SerializeField] private TextConst.Theme _theme = TextConst.Theme.Free;
        [SerializeField] private FontStyles _stryle = FontStyles.Normal;
        [SerializeField] private TextConst.ColorType _colorType = TextConst.ColorType.Normal;
        [SerializeField] private TextAlignmentOptions _alignment = TextAlignmentOptions.Center;
        [SerializeField] private TextMeshProUGUI _text = null;

        private Language _lang = Language.Ja;


        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Start() {
            // Inspectorビューで設定されている内容を基にテキストを表示する.
            Display().Forget();
        }

        /// <summary>
        /// 現在のメンバ変数の情報を基にテキストを表示する.
        /// 外部クラスからテキスト表示するための唯一のアクセスポイント.
        /// </summary>
        [ContextMenu("Display")]
        public async UniTask Display() {

            // メンバ変数の内容を基にTextMeshProコンポーネントへ設定する.
            await Setup();

            // マスターからテキストデータを取得しUIへ表示.
            var store = OKGames.Context.ResourceStore;
            string address = AssetAddress.AssetAddressEnum.texts.ToString();
            if (!store.Contains(address)) {
                // まだTextマスターを取得していなかったらAddressablesから取得.
                string[] textAddress = new string[1] { address };
                await store.RetainGlobalWithAutoLoad(textAddress);
            }

            if (!string.IsNullOrEmpty(_key)) {
                var str = store.GetObj<Entity_text>(address).GetText(_key, _lang);
                _text.text = str;
            }
        }

        /// <summary>
        /// TextMeshProのコンポーネントへ各種設定を行う.
        /// </summary>
        private async UniTask Setup() {

            // 本当はフレームワークの参照はここからはさせたくない..。
            var userData = OKGames.Context.UserDataStore.Data.Value;
            _lang = userData.CurrentLanguage;

            var fontLoader = OKGames.Context.UI.FontLoader;

            // フォント設定.
            TMP_FontAsset font = await fontLoader.GetFont(_lang);
            _text.font = font;

            // スタイル設定.
            _text.fontStyle = _stryle;

            // サイズ設定.
            int fontSize = fontLoader.GetFontSize(_lang, _theme);
            _text.fontSize = fontSize + _sizeOffset;

            // オートサイズは処理が重いのと、決めのサイズ指定ができなくなるので使わない.
            _text.enableAutoSizing = false;

            // 折り返しにする.
            _text.enableWordWrapping = true;
            _text.overflowMode = TextOverflowModes.Truncate;

            // 整列設定.
            _text.alignment = _alignment;

            // カラー設定.
            Color color = fontLoader.GetFontColor(_colorType);
            _text.color = color;

            // タグを使用できるようにON.
            _text.richText = true;

            // 文字だけにヒット判定する必要はないのでOFF.
            _text.raycastTarget = false;
        }

        /// <summary>
        /// TextマスターのIDをセットする.
        /// </summary>
        /// <param name="key">Textマスターのキー情報.</param>
        public void SetTextID(string key) {
            _key = key;
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
    }
}
