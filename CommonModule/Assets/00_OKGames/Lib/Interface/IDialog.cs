using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// ダイアログを管理するクラスのインターフェース.
    /// </summary>
    public interface IDialog {

        GameObject ViewRoot { get; }

        /// <summary>
        /// 初期化.
        /// </summary>
        void Init(IResourceStore resourceStore, Entity_text textMaster, IPrev prev);

        /// <summary>
        /// ダイアログの生成から表示まで一気通貫して行う.
        /// </summary>
        /// <returns>UniTask.</returns>
        UniTask GenerateDialogView(DialogTransfer transfer);

        /// <summary>
        /// ダイアログのセットアップ.
        /// 返り値でOpen関数を呼ぶとダイアログが表示される.
        /// </summary>
        /// <returns>UniTask.</returns>
        UniTask<IDialogComposit> SetupDialogView(DialogTransfer transfer);

    }
}
