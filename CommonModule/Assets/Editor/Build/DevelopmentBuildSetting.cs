using OKGamesLib;
using OKGamesFramework;
using UnityEngine;
using UnityEditor;

// ---------------------------------------------------------
// Developビルド時のビルドパラメータを保持、設定するクラス.
// ---------------------------------------------------------
public class DevelopmentBuildSetting : IBuildSetting {
    /// <summary>
    /// ビルドオプション.
    /// </summary>
    BuildOptions IBuildSetting.BuildOptions => (BuildOptions.Development | BuildOptions.ConnectWithProfiler);

    // シンボル情報.
    //(AA;BB;CCという形で列挙する).
    private readonly string _scriptingDefineSymbols = "DEVELOPMENT";

    void IBuildSetting.SetupBuildSettins(BuildTarget target) {

        EditorUserBuildSettings.development = true;

        if (target == BuildTarget.Android) {
            // Android固有の設定.
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC2;
            EditorUserBuildSettings.buildAppBundle = false; // apkで出力する.
        }

        if (target == BuildTarget.iOS) {
            // iOS固有の設定.
            // developモードをONにする.
            EditorUserBuildSettings.iOSXcodeBuildConfig = XcodeBuildConfig.Debug;
        }
    }

    void IBuildSetting.SetupPlayerSettins(BuildTarget target) {
        // 開発者名とアプリ名の設定.
        PlayerSettings.companyName = CommonConst.CompanyName;
        PlayerSettings.productName = AppConst.AppNameDevelopment; // 開発アプリは申請で本番アプリと同じところには上げない.

        // version系
        var version = (BuildArgs.AppVersionCode == string.Empty) ? Application.version : BuildArgs.AppVersionCode;
        // iOSとAndroidプラットフォームで共有されるアプリケーションのバンドルバージョン(1.0.0の3桁形式、前二桁がversion, 後一桁にbuildVersionを用いる).
        PlayerSettings.bundleVersion = $"{version}.{BuildArgs.BuildVersion}";

        int platformVersionCode = 0;
        if (target == BuildTarget.Android) {
            // Android固有設定.
            if (int.TryParse(BuildArgs.AndroidVersionCode, out platformVersionCode)) {
                PlayerSettings.Android.bundleVersionCode = platformVersionCode;
            } else {
                Log.Warning("bundleVersionCodeの設定はされていません");
            }

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
            // 対象のアーキテクチャ設定.
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;

            // シンボル設定.
            PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Android, _scriptingDefineSymbols);
        }

        if (target == BuildTarget.iOS) {
            // iOS固有設定.
            if (int.TryParse(BuildArgs.IOSVersionCode, out platformVersionCode)) {
                PlayerSettings.iOS.buildNumber = platformVersionCode.ToString();
            } else {
                Log.Warning("buildNumberの設定はされていません");
            }
            PlayerSettings.iOS.applicationDisplayName = AppConst.AppNameDevelopment; // 開発用を設定.
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;

            // IL2CPP設定.
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);

            // エラー時にExceptionが出て不具合原因の特定がしやすくなる.
            PlayerSettings.iOS.scriptCallOptimization = ScriptCallOptimizationLevel.SlowAndSafe;

            // シンボルの設定.
            PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.iOS, _scriptingDefineSymbols);
        }
    }

    void IBuildSetting.SetupOtherSettins(BuildTarget target) {
        // ログは表示されるように
        UnityEngine.Debug.unityLogger.logEnabled = true;

        if (target == BuildTarget.Android) {

        }


        if (target == BuildTarget.iOS) {

        }
    }
}
