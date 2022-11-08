using OKGamesFramework;
using OKGamesLib;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesTest {

    /// <summary>
    /// TestIAPシーンのアダプタ.
    /// </summary>
    public class TestIAPAdapter : BaseSceneAdapter {

        /// <summary>
        /// シーンのpresenter.
        /// </summary>
        [SerializeField] private TestIAPPresenter _presenter;

        private IIAP _iap;

        /// <summary>
        /// <see cref="BaseSceneAdapter.InitAfterLoadScene"/>.
        /// </summary>
        public override async UniTask InitAfterLoadScene(CancellationTokenSource token) {
            await base.InitAfterLoadScene(token);

            // 初期化関連の処理.
            _iap = OKGames.Iap;
            var store = OKGames.Context.ResourceStore;
            _presenter.Init();
            _presenter.Inject(_iap, store);
            _presenter.Bind();

            // セットアップ.
            _presenter.SetupIAPItems();
        }

        /// <summary>
        /// <see cref="BaseSceneAdapter.Finalize"/>.
        /// </summary>
        public override UniTask Finalize() {
            _presenter.Dispose();
            _presenter = null;

            _iap = null;

            return base.Finalize();
        }
    }
}
