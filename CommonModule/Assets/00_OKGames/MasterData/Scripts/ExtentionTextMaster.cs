using OKGamesFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Textマスターの拡張クラス.
/// </summary>
public static class ExtentionTextMaster {

    /// <summary>
    /// keyを基にレコードを取得する.
    /// </summary>
    /// <param name="key">キー情報.</param>
    /// <param name="sheetIndex">excelの元データのシートページ. 1ページしかない想定.</param>
    /// <returns></returns>
    private static Entity_text.Param GetRecordByKey(Entity_text master, string key, int sheetIndex = 0) {
        return master.sheets[sheetIndex].list.Find(data => (data.key == key));
    }

    /// <summary>
    /// キーと言語を基にテキストを取得する.
    /// </summary>
    /// <param name="key">キー.</param>
    /// <param name="lang">言語.</param>
    /// <returns>テキスト.</returns>
    public static string GetText(this Entity_text master, string key, Language lang) {
        Entity_text.Param data = GetRecordByKey(master, key, 0);

        string str = "";
        switch (lang) {
            case Language.Ja:
                str = data.ja;
                break;
            case Language.En:
                str = data.en;
                break;
        }

        return str;
    }

    /// <summary>
    /// キーを基にテキストを取得する.
    /// </summary>
    /// <param name="key">キー.</param>
    /// <returns>テキスト.</returns>
    public static string GetText(this Entity_text master, string key) {
        Language lang = OKGames.Context.UserDataStore.Data.Value.CurrentLanguage;
        string str = GetText(master, key, lang);

        return str;
    }
}
