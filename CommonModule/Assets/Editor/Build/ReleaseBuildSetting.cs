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
    BuildOptions IBuildSetting.BuildOptions => _buildOptions;
    private BuildOptions _buildOptions;


    // シンボル情報
    //(AA;BB;CCという形で列挙する).
    private readonly string _scriptingDefineSymbols = "RELEASE";


    void IBuildSetting.SetupBuildSettins(BuildTarget target, bool isUpStore) {
        // developモードをOFFにする.
        EditorUserBuildSettings.development = false;

        if (target == BuildTarget.Android) {
            // Android固有の設定.
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC2;
            // Google Play StoreではApp BundleしかUpできない.
            // AppCenterなどは逆にapk形式でしたUpできない.
            EditorUserBuildSettings.buildAppBundle = isUpStore;

        }

        if (target == BuildTarget.iOS) {
            // iOS固有の設定.
            EditorUserBuildSettings.iOSXcodeBuildConfig = XcodeBuildConfig.Release;
        }

    }

    void IBuildSetting.SetupPlayerSettins(BuildTarget target, bool isUpStore) {
        // 開発者名とアプリ名の設定.
        PlayerSettings.companyName = CommonConst.CompanyName;
        PlayerSettings.productName = AppConst.AppName;

        // version系
        var version = (BuildArgs.AppVersionCode == string.Empty) ? Application.version : BuildArgs.AppVersionCode;
        // iOSとAndroidプラットフォームで共有されるアプリケーションのバンドルバージョン(1.0.0の3桁形式、前二桁がversion, 後一桁にbuildVersionを用いる).
        PlayerSettings.bundleVersion = !string.IsNullOrEmpty(BuildArgs.BuildVersion) ? $"{version}.{BuildArgs.BuildVersion}" : "0.0.1";

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

            if (isUpStore) {
                // Google Play StoreへUpする際はキーストアによる署名がされていないとUpできない.
                if (!string.IsNullOrEmpty(BuildArgs.KeyStorePath) && !string.IsNullOrEmpty(BuildArgs.KeyStorePass)
                && !string.IsNullOrEmpty(BuildArgs.KeyAliasName) && !string.IsNullOrEmpty(BuildArgs.KeyAliasPass)) {
                    PlayerSettings.Android.useCustomKeystore = true;
                    // keystore設定.
                    PlayerSettings.Android.keystoreName = BuildArgs.KeyStorePath;
                    PlayerSettings.Android.keystorePass = BuildArgs.KeyStorePass;
                    // keystoreのエイリアス設定.
                    PlayerSettings.Android.keyaliasName = BuildArgs.KeyAliasName;
                    PlayerSettings.Android.keyaliasPass = BuildArgs.KeyAliasPass;
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                } else {
                    PlayerSettings.Android.useCustomKeystore = false;
                }
            } else {
                // AppCenter等でUpする際は署名は不要.
                PlayerSettings.Android.useCustomKeystore = false;
            }

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

    void IBuildSetting.SetupOtherSettins(BuildTarget target, bool isUpStore) {
        // ログは表示されないようにする(負荷対策とチート対策).
        UnityEngine.Debug.unityLogger.logEnabled = false;

        // ビルドオプションはNoneで設定する.
        _buildOptions = BuildOptions.None;

        if (target == BuildTarget.Android) {
            // キーストアを使用する、しないを行き来するとSDKとJDKが見つからなくなるUnityの不具合がある.
            // 一旦External ToolsのJDKとSDKのチェックボックスを外して保存→つけて保存とすると見つかるようになるのでその部分を自動化している.
            string jdkKey = "KEY_JDK_USE_EMBEDDED";
            string sdkKey = "KEY_SDK_USE_EMBEDDED";
            EditorPrefs.SetBool(jdkKey, false);
            EditorPrefs.SetBool(sdkKey, false);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorPrefs.SetBool(jdkKey, true);
            EditorPrefs.SetBool(sdkKey, true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        if (target == BuildTarget.iOS) {
            // 広告対応でcocoapodsを入れたが、デフォルトだとxcworkspaceも出力されるになった。
            // xcprojのみの出力で良いのでその設定をここでしている.
            var integration = Google.IOSResolver.CocoapodsIntegrationMethod.Project;
            Google.IOSResolver.CocoapodsIntegrationMethodPref = integration;
        }
    }
}
