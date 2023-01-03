using OKGamesFramework;
using OKGamesLib;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesTest {

    /// <summary>
    /// TestScrollシーンのアダプタ.
    /// </summary>
    public class TestScrollAdapter : BaseSceneAdapter {

        /// <summary>
        /// シーンのpresenter.
        /// </summary>
        [SerializeField] private TestScrollPresenter _presenter;

        /// <summary>
        /// <see cref="BaseSceneAdapter.InitAfterLoadScene"/>.
        /// </summary>
        public override async UniTask InitAfterLoadScene(CancellationTokenSource token) {
            await base.InitAfterLoadScene(token);

            // 初期化関連の処理.
            _presenter.Init();

        }

        /// <summary>
        /// <see cref="BaseSceneAdapter.Finalize"/>.
        /// </summary>
        public override UniTask Finalize() {
            _presenter.Dispose();
            _presenter = null;

            return base.Finalize();
        }
    }
}
