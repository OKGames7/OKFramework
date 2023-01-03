namespace OKGamesLib {

    /// <summary>
    /// <see cref="ObjectPoolRegistry"/>>と何かのクラス/そのクラスの処理を紐づけるためのハブのインターフェース.
    /// </summary>
    public interface IObjectPoolHub {
        ObjectPoolRegistry SceneScopeObjectPoolRegistry { get; }
        ObjectPoolRegistry GlobalScopeObjectPoolRegistry { get; }
    }
}
