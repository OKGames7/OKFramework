using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// UIの初期化処理を担当するクラス.
    /// </summary>
    public class UIInitializer {
        public class ResponseData {
            public ITextAdapter TextAdapter { get; set; }
            public IFontLoader FontLoader { get; set; }
            public IButtonAdapter ButtonAdapter { get; set; }
            public IFader Fader { get; set; }
            public IPopNotify PopNotify { get; set; }
            public IDialog SystemDialog { get; set; }
            public IDialog CommonDialog { get; set; }
            public IDialog CustomeDialog { get; set; }

        }

        /// <summary>
        /// 初期化.
        /// </summary>
        /// <returns>ResponseData.</returns>
        public ResponseData Init() {
            var res = new ResponseData();
            res.FontLoader = new FontLoader();
            res.ButtonAdapter = new ButtonAdapter();
            res.TextAdapter = new TextAdapter();

            return res;
        }

        /// <summary>
        /// 非同期が必要な部分の初期化.
        /// </summary>
        /// <param name="parent">生成したUIオブジェクトの親に設定したいGameObject.</param>
        /// <param name="resourceStore">アセット取得先のStore.</param>
        /// <param name="poolHub">Pool処理のハブ.</param>
        /// <returns>ResponseData.</returns>
        public async UniTask<ResponseData> InitAsync(GameObject parent, IResourceStore resourceStore, IObjectPoolHub poolHub, IPrev prev) {
            // 各種UI管理系のオブジェクトをAddressableからロードする.
            string fadeAddress = AssetAddress.AssetAddressEnum.Fader.ToString();
            string popNotifyAddress = AssetAddress.AssetAddressEnum.PopNotify.ToString();
            string systemDialogAddress = AssetAddress.AssetAddressEnum.SystemDialog.ToString();
            string commonDialogAddress = AssetAddress.AssetAddressEnum.CommonDialog.ToString();
            string customeDialogAddress = AssetAddress.AssetAddressEnum.CustomeDialog.ToString();
            string[] addresses = new string[] {
                fadeAddress,
                popNotifyAddress,
                systemDialogAddress,
                commonDialogAddress,
                customeDialogAddress };
            await resourceStore.RetainGlobalWithAutoLoad(addresses);

            // 返却データの箱を用意.
            var res = new ResponseData();

            // フェード表現を管理するオブジェクトを生成し親階層をセットする.
            res.Fader = GetChildCreateComponent<Fade>(parent, resourceStore, fadeAddress);
            res.Fader.Init();

            // ポップ通知を管理するオブジェクトを生成し、親階層をセットする.
            res.PopNotify = GetCreateComponent<PopNotify>(parent, resourceStore, popNotifyAddress);
            res.PopNotify.Init();

            // システムダイアログを管理するオブジェクトを生成し、親階層をセットする.
            var textMaster = resourceStore.GetObj<Entity_text>(AssetAddress.AssetAddressEnum.texts.ToString());
            var systemDialog = GetCreateComponent<SystemDialog>(parent, resourceStore, systemDialogAddress);
            res.SystemDialog = systemDialog as IDialog;
            res.SystemDialog.Init(resourceStore, textMaster, prev);

            // 共通ダイアログを管理するオブジェクトを生成し、親階層をセットする.
            var commonDialog = GetCreateComponent<CommonDialog>(parent, resourceStore, commonDialogAddress);
            res.CommonDialog = commonDialog as IDialog;
            var pool = commonDialog.gameObject.AddComponent<CommonDialogPool>();
            await pool.InitPoolAsync(res.CommonDialog.ViewRoot, resourceStore, poolHub);
            res.CommonDialog.Init(resourceStore, textMaster, prev);

            // カスタムダイアログを管理するオブジェクトを生成し、親階層をセットする.
            var customeDialog = GetCreateComponent<CustomeDialog>(parent, resourceStore, customeDialogAddress);
            res.CustomeDialog = customeDialog as IDialog;
            res.CustomeDialog.Init(resourceStore, textMaster, prev);

            return res;
        }

        private T GetCreateComponent<T>(GameObject parent, IResourceStore store, string address) where T : MonoBehaviour {
            var prefab = store.GetGameObj(address);
            var obj = prefab.Instantiate(parent);
            return obj.GetComponent<T>();
        }

        private T GetChildCreateComponent<T>(GameObject parent, IResourceStore store, string address) where T : MonoBehaviour {
            var prefab = store.GetGameObj(address);
            var obj = prefab.Instantiate(parent);
            return obj.GetComponentInChildren<T>();
        }
    }
}
