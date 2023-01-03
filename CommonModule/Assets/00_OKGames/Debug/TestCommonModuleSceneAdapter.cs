using OKGamesFramework;
using OKGamesLib;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesTest {

    /// <summary>
    /// Bootシーンのアダプタ.
    /// </summary>
    public class TestCommonModuleSceneAdapter : BaseSceneAdapter {

        [SerializeField] private ButtonWrapper _goIAPTestButton;
        public override async UniTask InitAfterLoadScene(CancellationTokenSource token) {
            await base.InitAfterLoadScene(token);
            Bind();

            var dialog = OKGames.UI.CommonDialog;

            var transfar = new DialogTransfer();
            transfar.HeaderText = "header XXX";
            transfar.BodyText = "body YYY";
            transfar.SetButtonsData(
                () => { Log.Notice("ok"); },
                () => { Log.Notice("no"); }
            );
            dialog.GenerateDialogView(transfar).Forget();
        }

        private void Bind() {
            // IAPテストシーンへの遷移関数.
            var action = GetGoSceneAction<TestIAPContext>();
            _goIAPTestButton.SetClickAction(action);
        }

        /// <summary>
        /// シーンへ遷移する処理を作成し呼び出し元で取得する.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private Action GetGoSceneAction<T>() where T : ISceneContext, new() {
            return () => {
                ISceneContext context = new T();
                OKGames.Context.SceneDirector.GoToNextScene(context).Forget();
            };
        }
    }
}
