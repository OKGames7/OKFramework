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

            string jaText = textMaster.GetText("DIALOG_OK");
            string enText = textMaster.GetText("DIALOG_OK", Language.En);

            // テキストを言語ごとで取得できるかチェック.
            Assert.That(jaText == "決定");
            Assert.That(enText == "OK");

            jaText = textMaster.GetText("DIALOG_NO");
            enText = textMaster.GetText("DIALOG_NO", Language.En);

            // 先頭以外の要素も取得できるかチェック.
            Assert.That(jaText == "キャンセル");
            Assert.That(enText == "NO");
        });
    }
}
