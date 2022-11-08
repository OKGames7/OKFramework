using System.Collections.Generic;

namespace OKGamesLib {

    /// <summary>
    /// シーン内の全ボタンの状況を監視するためのクラスのインターフェース.
    /// </summary>
    public interface IButtonWatcher {

        /// <summary>
        /// シーン内に存在するButtonWrapper全て.
        /// </summary>
        List<ButtonWrapper> WrapperList { get; }

        /// <summary>
        /// Listへ登録する.
        /// </summary>
        /// <param name="button">登録するボタン.</param>
        void Add(ButtonWrapper button);

        /// <summary>
        /// 購読処理を紐づける.
        /// </summary>
        /// <param name="button">購読処理を紐づけるボタン.</param>
        void Bind(ButtonWrapper button);

        /// <summary>
        /// Listから削除する.
        /// </summary>
        /// <param name="button">登録削除するボタン.</param>
        void Remove(ButtonWrapper button);
    }
}
