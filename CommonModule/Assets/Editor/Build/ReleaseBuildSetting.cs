using OKGamesLib;
using UnityEngine;
using UnityEditor;

// ---------------------------------------------------------
// Releaseビルド時のビルドパラメータを保持、設定するクラス.
// ---------------------------------------------------------
public class ReleaseBuildSetting : IBuildSetting {
    /// <summary>
    /// ビルドオプション.
    /// </summary>
    BuildOptions IBuildSetting.BuildOptions => BuildOptions.None;

    // シンボル情報
    //(AA;BB;CCという形で列挙する).
    private readonly string _scriptingDefineSymbols = "RELEASE";


    void IBuildSetting.SetupBuildSettins(BuildTarget target) {
        // developモードをOFFにする.
        EditorUserBuildSettings.development = false;

        if (target == BuildTarget.Android) {
            // Android固有の設定.
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC2;
            EditorUserBuildSettings.buildAppBundle = false; // apkで出力する.
        }

        if (target == BuildTarget.iOS) {
            // iOS固有の設定.
            EditorUserBuildSettings.iOSXcodeBuildConfig = XcodeBuildConfig.Release;
        }

    }

    void IBuildSetting.SetupPlayerSettins(BuildTarget target) {
        // 開発者名とアプリ名の設定.
        PlayerSettings.companyName = CommonConst.CompanyName;
        PlayerSettings.productName = AppConst.AppName;

        // version系
        var version = (BuildArgs.AppVersionCode == string.Empty) ? Application.version : BuildArgs.AppVersionCode;
        // iOSとAndroidプラットフォームで共有されるアプリケーションのバンドルバージョン(1.0.0の3桁形式、前二桁がversion, 後一桁にbuildVersionを用いる).
        PlayerSettings.bundleVersion = $"{version}.{BuildArgs.BuildVersion}";

        // 使用していないMeshコンポーネントをゲームのビルドから除外する.
        PlayerSettings.stripUnusedMeshComponents = true;

        int platformVersionCode = 0;
        if (target == BuildTarget.Android) {
            // Android固有設定.
            if (int.TryParse(BuildArgs.AndroidVersionCode, out platformVersionCode)) {
                PlayerSettings.Android.bundleVersionCode = platformVersionCode;
            } else {
                Log.Warning("bundleVersionCodeの設定はされていません");
            }

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            // 対象のアーキテクチャ設定.
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            // TODO: keyStoreの設定

            // シンボルの設定.
            PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Android, _scriptingDefineSymbols);
        }

        if (target == BuildTarget.iOS) {
            // iOS固有設定.
            if (int.TryParse(BuildArgs.IOSVersionCode, out platformVersionCode)) {
                PlayerSettings.iOS.buildNumber = platformVersionCode.ToString();
            } else {
                Log.Warning("buildNumberの設定はされていません");
            }
            PlayerSettings.iOS.applicationDisplayName = AppConst.AppName;
            PlayerSettings.iOS.appleEnableAutomaticSigning = false;

            // IL2CPP設定.
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);

            // リリース用は速度優先.エラーも取りきれているはずなので例外処理は投げないようにする.
            PlayerSettings.iOS.scriptCallOptimization = ScriptCallOptimizationLevel.FastButNoExceptions;

            // シンボル設定.
            PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.iOS, _scriptingDefineSymbols);
        }
    }

    void IBuildSetting.SetupOtherSettins(BuildTarget target) {
        // ログは表示されないようにする(負荷対策とチート対策).
        UnityEngine.Debug.unityLogger.logEnabled = false;

        if (target == BuildTarget.Android) {

        }


        if (target == BuildTarget.iOS) {

        }
    }
}
