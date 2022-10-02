using UnityEditor;

// ---------------------------------------------------------
// DevelopビルドかReleaseビルドによって設定を
// 使い分けるビルドパラメータを持ったインターフェース.
// ---------------------------------------------------------
public interface IBuildSetting {

    /// <summary>
    /// ビルドオプション.
    /// </summary>
    BuildOptions BuildOptions { get; }

    /// <summary>
    /// BuildSettinsの設定.
    /// </summary>
    void SetupBuildSettins(BuildTarget target);

    /// <summary>
    /// PlayerSettingsの設定.
    /// </summary>
    void SetupPlayerSettins(BuildTarget target);

    /// <summary>
    /// BuildSettigsとPlayerSettings以外の設定.
    /// </summary>
    void SetupOtherSettins(BuildTarget target);

}
