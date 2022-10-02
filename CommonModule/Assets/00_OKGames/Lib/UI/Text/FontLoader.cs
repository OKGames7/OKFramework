using OKGamesLib;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// フォントのローダー.
    /// </summary>
    public class FontLoader : IFontLoader {

        public async UniTask<TMP_FontAsset> GetFontByCurrentLang() {
            Language lang;
            var userData = OKGamesFramework.OKGames.Context.UserDataStore.Data;
            if (userData != null) {
                lang = userData.Value.CurrentLanguage;
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

            var store = OKGamesFramework.OKGames.Context.ResourceStore;

            if (!store.Contains(str)) {
                // まだフォントアセットをAddressablesから取得していなかったらロード
                string[] fontAddress = new string[1] { str };
                await store.RetainGlobalWithAutoLoad(fontAddress);
            }

            // フォントの取得.
            var font = store.GetObj<TMP_FontAsset>(str);
            return font;
        }

        public int GetFontSize(Language lang, TextConst.Theme theme) {
            int size = -1;
            switch (theme) {
                case TextConst.Theme.Free:
                    // offsetの値 = 文字サイズとする.
                    size = 0;
                    break;
                case TextConst.Theme.DialogTitle:
                    size = 36;
                    break;
                case TextConst.Theme.ADVTtitle:
                    size = 32;
                    break;
                case TextConst.Theme.ButtonLavel:
                case TextConst.Theme.ListItemLabel:
                case TextConst.Theme.DialogContent:
                case TextConst.Theme.ADVContent:
                    size = 30;
                    break;
            }

            if (lang != Language.Ja) {
                // 日本語を基準にそれ以外(アルファベット)だったら一律4pt大きいサイズとする.
                size += 4;
            }

            return size;
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
