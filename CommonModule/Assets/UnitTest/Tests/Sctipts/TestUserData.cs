using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;
using Cysharp.Threading.Tasks;
using OKGamesFramework;

namespace UnitTest {

    /// <summary>
    /// ユーザーデータに関するテスト.
    /// </summary>
    public class TestUserData {

        /// <summary>
        /// ストレージのユーザーデータの取得、保存ができるかのテスト.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestLocalUserData() => UniTask.ToCoroutine(async () => {
            var userStore = OKGames.Context.UserDataStore;
            await userStore.Save(Language.En);

            Assert.That(userStore.Data.Value.CurrentLanguage == Language.En);

            await userStore.Save(Language.Ja);
            Assert.That(userStore.Data.Value.CurrentLanguage == Language.Ja);
        });
    }
}
