using System;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// ローカルに保存するユーザーデータ.
    /// </summary>
    [Serializable]
    public class UserData {

        /// <summary>
        /// 選択言語.
        /// 初期値は日本語.
        /// </summary>
        public Language CurrentLanguage = Language.Ja;
    }
}
