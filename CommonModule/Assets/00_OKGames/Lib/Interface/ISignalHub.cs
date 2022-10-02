namespace OKGamesLib {

    /// <summary>
    /// <see cref="SignalRegistry"/>と何かのクラス/そのクラスの処理を紐づけるためのハブのインターフェース..
    /// </summary>
    public interface ISignalHub {
        /// <summary>
        /// シーン遷移時にも破棄されない<see cref="SignalRegistry"/>.
        /// </summary>
        SignalRegistry GlobalScopeSignalRegistry { get; }

        /// <summary>
        /// シーン遷移時に破棄される<see cref="SignalRegistry"/>.
        /// </summary>
        SignalRegistry SceneScopeSignalRegistry { get; }
    }
}
