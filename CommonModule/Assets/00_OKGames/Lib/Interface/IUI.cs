using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// UI全般に関するアクセスポイントのインターフェース.
    /// </summary>
    public interface IUI {
        /// <summary>
        /// フォントアセットを取得するためのローダー.
        /// </summary>
        IFontLoader FontLoader { get; }

        /// <summary>
        /// シーン全体のボタンを制御するためのアダプター.
        /// </summary>
        IButtonAdapter ButtonAdapter { get; }

        /// <summary>
        /// シーン全体のテキストを制御するためのアダプター.
        /// </summary>
        ITextAdapter TextAdapter { get; }

        /// <summary>
        /// 画面全体を覆うフェーダー.
        /// </summary>
        public IFader Fader { get; }

        /// <summary>
        /// 画面上のポップ通知の管理クラス.
        /// </summary>
        public IPopNotify PopNotify { get; }

        /// <summary>
        /// システムダイアログの管理クラス.
        /// </summary>
        public IDialog SystemDialog { get; }

        /// <summary>
        /// 共通ダイアログの管理クラス.
        /// </summary>
        public IDialog CommonDialog { get; }

        /// <summary>
        /// カスタムダイアログの管理クラス.
        /// </summary>
        public IDialog CustomeDialog { get; }

        /// <summary>
        /// 初期化処理.
        /// 主にメンバ変数の初期化処理がされることを想定している.
        /// </summary>
        /// <param name="parent">親階層.</param>
        void Init(Transform parent);

        /// <summary>
        /// 依存関係の注入.
        /// </summary>
        /// <param name="transfar">依存性注入するデータが入っているtransfer.</param>
        void Inject(IUITransfer transfar);

        /// <summary>
        /// 非同期初期化.
        /// リソースストアからUI系のAddressablesを読み込みセットアップすることを想定している.
        /// </summary>
        /// <param name="resourceStore">リソースストア.</param>
        /// <param name="poolHub">オブジェクトプールのハブ.</param>
        /// <param name="prev">前に戻る機能を管理するクラス.</param>
        /// <returns></returns>
        UniTask InitAsync(IResourceStore resourceStore, IObjectPoolHub poolHub, IPrev prev);

        /// <summary>
        /// 全テキストのテキスト表示を更新する.
        /// </summary>
        /// <param name="lang"></param>
        void UpdateTexts(Language lang);

        /// <summary>
        /// 画面真ん中にテキスト通知する.
        /// </summary>
        /// <param name="text">通知テキスト.</param>
        void NotifyCenter(string text);
    }
}
