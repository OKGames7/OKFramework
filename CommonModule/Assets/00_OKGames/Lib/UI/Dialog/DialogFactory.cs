using OKGamesFramework;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// ダイアログを生成するファクトリ.
    /// </summary>
    public class DialogFactory {

        private IResourceStore _resourceStore;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="store"></param>
        public DialogFactory(IResourceStore store) {
            _resourceStore = store;
        }

        public async UniTask<GameObject> GetCommonDialogForPool() {
            var transfar = new DialogTransfer();
            transfar.HeaderText = "header XXX";
            transfar.BodyText = "body YYY";
            transfar.SetButtonsData(
                () => { Log.Notice("yes"); },
                () => { Log.Notice("no"); },
                "ok button",
                "no button"
            );
            return await GetDialog(transfar);
        }

        /// <summary>
        /// ダイアログを取得し生成する.
        /// </summary>
        /// <param name="transfer"></param>
        /// <returns></returns>
        public async UniTask<GameObject> InstantiateDialog(DialogTransfer transfer) {
            var dialog = await GetDialog(transfer);
            var obj = GameObject.Instantiate(dialog);
            return obj;
        }

        /// <summary>
        /// ダイアログを取得する
        /// </summary>
        private async UniTask<GameObject> GetDialog(DialogTransfer transfer) {
            string dialogViewAddress = transfer.PrefabAddress;
            await _resourceStore.RetainGlobalWithAutoLoad(dialogViewAddress);
            var dialog = _resourceStore.GetGameObj(dialogViewAddress);
            return dialog;
        }
    }
}
