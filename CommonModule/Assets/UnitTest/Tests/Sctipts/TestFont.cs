using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;
using Cysharp.Threading.Tasks;
using OKGamesFramework;
using OKGamesLib;
using TMPro;

namespace UnitTest {

    /// <summary>
    /// Fontに関するテスト.
    /// </summary>
    public class TestFont {

        /// <summary>
        /// AddressablesからFontが取得できるかのテスト.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestGetFont() => UniTask.ToCoroutine(async () => {
            var store = OKGames.Context.ResourceStore;

            string jaFontAddress = AssetAddress.AssetAddressEnum.NotoSansJP_Medium_SDF.ToString();
            string enFontAddress = AssetAddress.AssetAddressEnum.NotoSans_Medium_SDF.ToString();

            string[] addresses = new string[2] { jaFontAddress, enFontAddress };
            await store.RetainGlobalWithAutoLoad(addresses);

            var jaFont = store.GetObj<TMP_FontAsset>(jaFontAddress);
            Assert.That(jaFont != null);


            var enFont = store.GetObj<TMP_FontAsset>(enFontAddress);
            Assert.That(enFont != null);
        });

        /// <summary>
        /// FontLoader経由でのフォント取得のテスト.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestGetFontWithLoader() => UniTask.ToCoroutine(async () => {

            var fontLoader = OKGames.Context.UI.FontLoader;

            var font = await fontLoader.GetFont(Language.Ja);
            Assert.That(font.name == AssetAddress.AssetAddressEnum.NotoSansJP_Medium_SDF.ToString());


            font = await fontLoader.GetFont(Language.En);
            Assert.That(font.name == AssetAddress.AssetAddressEnum.NotoSans_Medium_SDF.ToString());

            var userStore = OKGames.Context.UserDataStore;
            await userStore.Save(Language.Ja);

            font = await fontLoader.GetFontByCurrentLang();
            Assert.That(font.name == AssetAddress.AssetAddressEnum.NotoSansJP_Medium_SDF.ToString());
        });
    }
}
