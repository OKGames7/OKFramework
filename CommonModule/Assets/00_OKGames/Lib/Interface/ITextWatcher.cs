using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace OKGamesLib {

    /// <summary>
    /// シーン内の全テキストの状況を監視するためのクラスのインターフェース.
    /// </summary>
    public interface ITextWatcher {

        /// <summary>
        /// シーン内に存在するTextWrapper全て.
        /// </summary>
        List<TextWrapper> WrapperList { get; }

        /// <summary>
        /// リストへ加える.
        /// </summary>
        /// <param name="wrapper">追加するTextWrapper.</param>
        void Add(TextWrapper wrapper);

        /// <summary>
        /// リストから削除する.
        /// </summary>
        /// <param name="wrapper">リストから削除するTextWrapper.</param>
        void Remove(TextWrapper wrapper);

        /// <summary>
        /// シーン内に存在する全てのテキストを更新する.
        /// </summary>
        /// <param name="language">設定言語.</param>
        UniTask UpdateTexts(Language language);

    }
}
