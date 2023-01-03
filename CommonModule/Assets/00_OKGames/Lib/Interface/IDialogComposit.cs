using System;
using Cysharp.Threading.Tasks;

namespace OKGamesLib {
    /// <summary>
    /// ダイアログ許通のコンポジットのインターフェース.
    /// </summary>
    public interface IDialogComposit {

        /// <summary>
        /// ダイアログを開く前に行う設定.
        /// </summary>
        /// <param name="transfer">ダイアログのセットアップに必要な移送データ.</param>
        /// <param name="textMaster">テキストのマスター.</param>
        /// <param name="prev">Backキーで閉じるために使用する.</param>
        void Setup(DialogTransfer transfer, Entity_text textMaster, IPrev prev);

        /// <summary>
        /// ダイアログを開く.
        /// </summary>
        /// <returns>UniTask.</returns>
        UniTask Open();

        /// <summary>
        /// ダイアログを閉じる.
        /// </summary>
        /// <param name="fromBackKey">Backキーを押すことで呼ばれたかどうか.</param>
        /// <returns>UniTask.</returns>
        UniTask Close(bool fromBackKey);

        /// <summary>
        /// ダイアログを開いた時のコールバック処理を設定する.
        /// </summary>
        /// <param name="callback">開いた時に行う処理.</param>
        void SetOpenCallback(Action callback);

        /// <summary>
        /// ダイアログを閉じた時のコールバック処理を設定する.
        /// </summary>
        /// <param name="callback">閉じた時に行う処理.</param>
        void SetCloseCallback(Action callback);
    }
}
