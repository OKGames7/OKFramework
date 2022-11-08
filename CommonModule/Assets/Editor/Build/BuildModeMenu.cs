using OKGamesLib;
using UnityEngine;
using UnityEditor;


// ---------------------------------------------------------
// ビルド設定をカスタムメニューから行えるようにする.
// ---------------------------------------------------------
public class BuildModeMenu : MonoBehaviour {
    /// <summary>
    /// DevelopmentモードでAndroidビルドする際の設定.
    /// </summary>
    [MenuItem("o.k.games/09.Build/Android/SetupDevelopment", false)]
    private static void SetupDevelopmentAndroid() {
        SetupBuildSettings(BuildTarget.Android, true, false);
    }

    /// DevelopmentモードでAndroidビルドする際の設定.
    /// </summary>
    [MenuItem("o.k.games/09.Build/Android/SetupDevelopmentForStore", false)]
    private static void SetupDevelopmentAndroidForStore() {
        SetupBuildSettings(BuildTarget.Android, true, true);
    }

    /// <summary>
    /// リリースモードでAndroidビルドする際の設定.
    /// </summary>
    [MenuItem("o.k.games/09.Build/Android/SetupRelease", false)]
    private static void SetupReleaseAndroid() {
        SetupBuildSettings(BuildTarget.Android, false, false);
    }

    /// <summary>
    /// リリースモードでAndroidビルドする際の設定.
    /// </summary>
    [MenuItem("o.k.games/09.Build/Android/SetupReleaseForStore", false)]
    private static void SetupReleaseAndroidForStore() {
        SetupBuildSettings(BuildTarget.Android, false, true);
    }


    /// <summary>
    /// DevelopmentモードでiOSビルドする際の設定.
    /// </summary>
    [MenuItem("o.k.games/09.Build/iOS/SetupDevelopment", false)]
    private static void SetupDevelopmentIOS() {
        SetupBuildSettings(BuildTarget.iOS, true, false);
    }

    /// <summary>
    /// DevelopmentモードでiOSビルドする際の設定.
    /// </summary>
    [MenuItem("o.k.games/09.Build/iOS/SetupDevelopmentForStore", false)]
    private static void SetupDevelopmentIOSForStore() {
        SetupBuildSettings(BuildTarget.iOS, true, true);
    }

    /// <summary>
    /// リリースモードでiOSビルドする際の設定.
    /// </summary>
    [MenuItem("o.k.games/09.Build/iOS/SetupRelease", false)]
    private static void SetupReleaseIOS() {
        SetupBuildSettings(BuildTarget.iOS, false, false);
    }

    /// <summary>
    /// リリースモードでiOSビルドする際の設定.
    /// </summary>
    [MenuItem("o.k.games/09.Build/iOS/SetupReleaseForStore", false)]
    private static void SetupReleaseIOSForStore() {
        SetupBuildSettings(BuildTarget.iOS, false, true);
    }

    /// <summary>
    /// ビルド設定の実処理部.
    /// </summary>
    private static void SetupBuildSettings(BuildTarget target, bool isDevelopment, bool isUpStore) {
        Log.Notice("設定開始");
        // ビルドに必要なパラメータ設定をするSetterクラスを生成.
        IBuildSetting buildParameterSetter = isDevelopment ? new DevelopmentBuildSetting() : new ReleaseBuildSetting();

        // BuildSettingsの設定.
        buildParameterSetter.SetupBuildSettins(target, isUpStore);

        // PlayerSettings関係の設定.
        buildParameterSetter.SetupPlayerSettins(target, isUpStore);

        // その他の設定.
        buildParameterSetter.SetupOtherSettins(target, isUpStore);
        Log.Notice("設定終了");
    }

}
