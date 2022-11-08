using OKGamesFramework;
using OKGamesLib;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesTest {

    /// <summary>
    /// TestBootシーンのアダプタ.
    /// </summary>
    public class TestBootAdapter : BaseSceneAdapter {

        [SerializeField] private ButtonWrapper _goIAPTestButton;

        public override async UniTask InitAfterLoadScene(CancellationTokenSource token) {
            await base.InitAfterLoadScene(token);
            Bind();
        }

        private void Start() {
            Bind();
        }

        private void Bind() {
            // IAPテストシーンへの遷移関数.
            var func = GetGoSceneFunc<TestIAPContext>();
            _goIAPTestButton.SetClickActionAsync(func);
        }

        /// <summary>
        /// シーンへ遷移する処理を作成し呼び出し元で取得する.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private Func<UniTask> GetGoSceneFunc<T>() where T : ISceneContext, new() {
            return async () => {
                ISceneContext context = new T();
                await OKGames.Context.SceneDirector.GoToNextScene(context);
            };
        }

    }
}
