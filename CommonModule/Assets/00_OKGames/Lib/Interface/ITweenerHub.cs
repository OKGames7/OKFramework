namespace OKGamesLib {

    /// <summary>
    /// <see cref="Tweener"/>と何かのクラス/そのクラスの処理を紐づけるためのハブクラスのインターフェース.
    /// </summary>
    public interface ITweenerHub {

        /// <summary>
        /// シーン遷移時に破棄される<see cref="Tweener"/>.
        /// </summary>
        Tweener SceneScopeTweener { get; }
    }
}
