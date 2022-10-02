using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;
using Cysharp.Threading.Tasks;
using OKGamesFramework;
using OKGamesLib;

namespace UnitTest {

    /// <summary>
    /// Textマスターに関するテスト.
    /// </summary>
    public class TestTextMaster {

        /// <summary>
        /// Addressablesでtextのマスターが取得できるかのテスト.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestGetTexts() => UniTask.ToCoroutine(async () => {

            var masterStore = OKGames.Context.ResourceStore;
            string[] textAddress = new string[1] { AssetAddress.AssetAddressEnum.texts.ToString() };

            await masterStore.RetainGlobalWithAutoLoad(textAddress);

            var textMaster = masterStore.GetObj<Entity_text>(AssetAddress.AssetAddressEnum.texts.ToString());

            // nullチェック.
            Assert.That(textMaster != null);

            string jaText = textMaster.GetText("TEST1");
            string enText = textMaster.GetText("TEST1", Language.En);

            // テキストを言語ごとで取得できるかチェック.
            Assert.That(jaText == "A JA");
            Assert.That(enText == "A EN");

            jaText = textMaster.GetText("TEST6");
            enText = textMaster.GetText("TEST6", Language.En);

            // 先頭以外の要素も取得できるかチェック.
            Assert.That(jaText == "FFFFFF JA");
            Assert.That(enText == "FFFFFF EN");
        });
    }
}
