using OKGamesLib;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

// ---------------------------------------------------------
// Jenkinsからのビルドジョブ実行時に処理する内容を記述する.
// ---------------------------------------------------------
public class BuildCommandLine {

    // ビルドに含めるシーンのパス配列
    private static string[] _scenePaths;

    /// <summary>
    /// Androidビルドする.
    /// </summary>
    public static void BuildAndroid() {
        BuildTargetGroup group = BuildTargetGroup.Android;
        BuildTarget target = BuildTarget.Android;
        BuildByPlatform(group, target);
    }

    /// <summary>
    /// iOSビルドする.
    /// </summary>
    public static void BuildIOS() {
        BuildTargetGroup group = BuildTargetGroup.iOS;
        BuildTarget target = BuildTarget.iOS;
        BuildByPlatform(group, target);
    }

    /// <summary>
    /// プラットフォームによって行うビルド設定、ビルド自体の処理.
    /// </summary>
    private static void BuildByPlatform(BuildTargetGroup group, BuildTarget target) {

        // コマンドラインで渡されている引数を取得してメンバ変数へ格納する.
        string[] args = System.Environment.GetCommandLineArgs();
        BuildArgs.SetArgs(args);

        // プラットフォーム設定をスイッチする.
        EditorUserBuildSettings.SwitchActiveBuildTarget(group, target);

        // ビルドに含めるシーンを取得する.
        _scenePaths = GetScenes();
        Log.Notice("格納されたシーン: " + string.Join("\n\t", _scenePaths));

        if (!BuildArgs.Validation(target)) {
            Log.Error("引数が合っていない");
            EditorApplication.Exit(1);
        }

        // ビルドに必要なパラメータ設定をするSetterクラスを生成.
        IBuildSetting buildParameterSetter = BuildArgs.IsDevelopment ? new DevelopmentBuildSetting() : new ReleaseBuildSetting();
        bool isUpStore = BuildArgs.IsUploadStore;
        // BuildSettingsの設定.
        buildParameterSetter.SetupBuildSettins(target, isUpStore);

        // PlayerSettings関係の設定.
        buildParameterSetter.SetupPlayerSettins(target, isUpStore);

        // その他の設定.
        buildParameterSetter.SetupOtherSettins(target, isUpStore);

        // 現在設定しているプラットフォームでアセットバンドルのビルドを行う.
        AddressableAssetSettings.BuildPlayerContent();

        // ビルドの実行.
        var result = BuildPipeline.BuildPlayer(_scenePaths, BuildArgs.ExportAppPath, target, buildParameterSetter.BuildOptions);

        // ビルド結果の確認.
        ComfirmResult(result);
    }

    /// <summary>
    /// ビルドに含めるシーンのパス配列を取得する.
    /// </summary>
    private static string[] GetScenes() {
        return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
    }

    /// <summary>
    /// ビルド結果を確認する.
    /// </summary>
    private static void ComfirmResult(UnityEditor.Build.Reporting.BuildReport report) {
        var result = report.summary.result;

        var messages = report.steps.SelectMany(x => x.messages)
            .ToLookup(x => x.type, x => x.content);

        Log.Notice("BuildResult : " + result);

        switch (result) {
            case UnityEditor.Build.Reporting.BuildResult.Succeeded:
                EditorApplication.Exit(0);
                break;
            case UnityEditor.Build.Reporting.BuildResult.Failed:
                Log.Notice(string.Join("\n\t", messages[LogType.Error].ToArray()));
                EditorApplication.Exit(1);
                break;
            case UnityEditor.Build.Reporting.BuildResult.Cancelled:
            case UnityEditor.Build.Reporting.BuildResult.Unknown:
                EditorApplication.Exit(1);
                break;
        }
    }
}
