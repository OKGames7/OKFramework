using OKGamesLib;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UniRx;

namespace OKGamesLib {

    /// <summary>
    /// フォント関連の読み込み処理を行うクラスのインターフェース.
    /// </summary>
    public interface IFontLoader {

        /// <summary>
        /// 依存性の注入.
        /// </summary>
        /// <param name="transfar">依存性注入するデータが入っているtransfer.</param>
        void Inject(IUITransfer transfer);

        /// <summary>
        /// 現在設定している言語で使用するフォントアセットを取得する.
        /// </summary>
        /// <param name="lang">表示するテキストの言語.</param>
        UniTask<TMP_FontAsset> GetFontByCurrentLang();

        /// <summary>
        /// 言語ごとに使用するフォントアセットを取得する.p
        /// </summary>
        /// <param name="lang">表示するテキストの言語.</param>
        UniTask<TMP_FontAsset> GetFont(Language lang);

        /// <summary>
        /// 言語とテキスト表示部のテーマごとにフォントサイズを取得する.
        /// </summary>
        /// <param name="lang">表示するテキストの言語.</param>
        /// <param name="theme">表示するテキストのテーマ.</param>
        /// <returns>フォントサイズ.</returns>
        int GetFontSize(Language lang, TextConst.Theme theme);

        /// <summary>
        /// フォントテキストの色を取得.
        /// </summary>
        /// <param name="type">カラータイプ.</param>
        /// <returns>フォントテキストの色.</returns>
        Color GetFontColor(TextConst.ColorType type);







    }
}
