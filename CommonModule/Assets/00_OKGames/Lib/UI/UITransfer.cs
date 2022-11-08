using UniRx;


namespace OKGamesLib {

    /// <summary>
    /// <see cref="UI"/>への依存性注入のためのデータを移送する.
    /// </summary>
    public class UITransfer : IUITransfer {
        public IReadOnlyReactiveProperty<UserData> UserData { get; private set; }

        public IResourceStore ResourceStore { get; private set; }

        public Language Lang { get; private set; }

        public Entity_text TextMaster { get; private set; }

        public IAudioPlayer SePlayer { get; private set; }

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="userData">ユーザーデータ.</param>
        /// <param name="resourceStore">アセットを取得するためのストア.</param>
        /// <param name="language">設定言語.</param>
        /// <param name="textMaster">テキストのマスター.</param>
        /// <param name="sePlayer">seを再生するためのプレイヤー.</param>
        public UITransfer(
            IReadOnlyReactiveProperty<UserData> userData,
            IResourceStore resourceStore,
            Language language,
            Entity_text textMaster,
            IAudioPlayer sePlayer
            ) {

            UserData = userData;
            ResourceStore = resourceStore;
            Lang = language;
            TextMaster = textMaster;
            SePlayer = sePlayer;
        }
    }
}
