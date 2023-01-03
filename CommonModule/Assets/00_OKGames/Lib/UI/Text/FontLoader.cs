using OKGamesLib;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UniRx;

namespace OKGamesLib {

    /// <summary>
    /// フォントのローダー.
    /// </summary>
    public class FontLoader : IFontLoader {

        private IReadOnlyReactiveProperty<UserData> _userData;
        private IResourceStore _resourceStore;


        public void Inject(IUITransfer transfer) {
            _userData = transfer.UserData;
            _resourceStore = transfer.ResourceStore;
        }

        public async UniTask<TMP_FontAsset> GetFontByCurrentLang() {
            Language lang;
            if (_userData != null) {
                lang = _userData.Value.CurrentLanguage;
            } else {
                // デフォルトは日本語とする.
                lang = Language.Ja;
            }

            var font = await GetFont(lang);

            return font;
        }


        public async UniTask<TMP_FontAsset> GetFont(Language lang) {
            string str = "";
            switch (lang) {
                case Language.Ja:
                    str = AssetAddress.AssetAddressEnum.NotoSansJP_Medium_SDF.ToString();
                    break;
                case Language.En:
                    str = AssetAddress.AssetAddressEnum.NotoSans_Medium_SDF.ToString();
                    break;
            }

            if (!_resourceStore.Contains(str)) {
                // まだフォントアセットをAddressablesから取得していなかったらロード
                string[] fontAddress = new string[1] { str };
                await _resourceStore.RetainGlobalWithAutoLoad(fontAddress);
            }

            // フォントの取得.
            var font = _resourceStore.GetObj<TMP_FontAsset>(str);
            return font;
        }

        public float GetFontSize(float japTextSize, Language lang) {
            float offset = 0.0f;
            switch (lang) {
                case Language.Ja:
                    offset = 0.0f;
                    break;
                case Language.En:
                    offset = 4.0f;
                    break;
            }
            return japTextSize + offset;
        }

        public Color GetFontColor(TextConst.ColorType type) {
            Color color = Color.white;
            switch (type) {
                case TextConst.ColorType.Normal:
                    // 白.
                    color = Color.black;
                    break;
                case TextConst.ColorType.Caution:
                    // 黄色形.
                    color = Color.yellow;
                    break;
                case TextConst.ColorType.Important:
                    // 赤色形.
                    color = Color.red;
                    break;
                case TextConst.ColorType.CompareUp:
                    // 赤色形.
                    color = Color.red;
                    break;
                case TextConst.ColorType.CompareDown:
                    // 青色形.
                    color = Color.blue;
                    break;
            }

            return color;
        }
    }
}
