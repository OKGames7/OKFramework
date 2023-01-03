using OKGamesFramework;
using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace OKGamesLib {


    /// <summary>
    /// 共通ダイアログのプール処理.
    /// </summary>
    public class CommonDialogPool : MonoBehaviour {

        private IObjectPoolHub _poolHub;

        private readonly int _poolNum = 3;

        public bool IsSetPool = false;

        public async UniTask InitPoolAsync(GameObject parent, IResourceStore store, IObjectPoolHub poolHub) {
            var factory = new DialogFactory(store);
            var commonDialogObj = await factory.GetCommonDialogForPool();

            _poolHub = poolHub;
            _poolHub.GlobalScopeObjectPoolRegistry.CreatePool<DialogComposit>(commonDialogObj, _poolNum, parent);

            IsSetPool = true;
        }

        public ObjectPool<DialogComposit> GetPool() {
            return _poolHub.GlobalScopeObjectPoolRegistry.GetPool<DialogComposit>();
        }
    }
}
