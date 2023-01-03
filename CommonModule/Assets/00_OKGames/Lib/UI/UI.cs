using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// UI全般に関するアクセスポイント.
    /// MonoBehaviourは必要なければ使う必要ないため本クラスには継承していない.
    /// </summary>
    public class UI : IUI {

        // MonoBehaviourを各UIコンポーネントを子に持つオブジェクト.
        private GameObject _parentGameObj;

        public ITextAdapter TextAdapter => _textAdapter;
        private ITextAdapter _textAdapter;

        public IFontLoader FontLoader => _fontLoader;
        private IFontLoader _fontLoader;

        public IButtonAdapter ButtonAdapter => _buttonAdapter;
        private IButtonAdapter _buttonAdapter;

        public IFader Fader => _fader;
        private IFader _fader;

        public IPopNotify PopNotify => _popNotify;
        private IPopNotify _popNotify;

        public IDialog SystemDialog => _systemDialog;
        private IDialog _systemDialog;

        public IDialog CommonDialog => _commondialog;
        private IDialog _commondialog;

        public IDialog CustomeDialog => _customeDialog;
        private IDialog _customeDialog;

        /// <summary>
        /// <see cref="IUI.Init"/>.
        /// </summary>
        public void Init(Transform parent) {
            // UI系はGameObject(Prefab)のものもあり、それらの階層構造がわかりやすいよう親となるオブジェクトを作っておく.
            _parentGameObj = new GameObject("UI");
            GameObject.DontDestroyOnLoad(_parentGameObj);
            _parentGameObj.SetParent(parent);

            var Initializer = new UIInitializer();
            var res = Initializer.Init();

            _fontLoader = res.FontLoader;
            _buttonAdapter = res.ButtonAdapter;
            _textAdapter = res.TextAdapter;
        }

        /// <summary>
        /// <see cref="IUI.Inject"/>.
        /// </summary>
        public void Inject(IUITransfer transfar) {
            FontLoader.Inject(transfar);
            ButtonAdapter.Inject(transfar);
            TextAdapter.Inject(transfar, FontLoader);
        }

        /// <summary>
        /// <see cref="IUI.InitAsync"/>.
        /// </summary>
        public async UniTask InitAsync(IResourceStore resourceStore, IObjectPoolHub poolHub, IPrev prev) {
            var initializer = new UIInitializer();
            var res = await initializer.InitAsync(_parentGameObj, resourceStore, poolHub, prev);

            _fader = res.Fader;
            _popNotify = res.PopNotify;
            _systemDialog = res.SystemDialog;
            _commondialog = res.CommonDialog;
            _customeDialog = res.CustomeDialog;
        }

        // 以下によく使う機能は簡易アクセスできるように関数に出している.

        /// <summary>
        /// <see cref="IUI.UpdateTexts"/>.
        /// </summary>
        public void UpdateTexts(Language language) {
            TextAdapter.UpdateTexts(language);
        }

        /// <summary>
        /// <see cref="IUI.NotifyCenter"/>.
        /// </summary>
        public void NotifyCenter(string text) {
            PopNotify.ShowCenter(text);
        }
    }
}
