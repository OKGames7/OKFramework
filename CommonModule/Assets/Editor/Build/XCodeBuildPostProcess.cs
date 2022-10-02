using OKGamesLib;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

// ---------------------------------------------------------
// Buildのポスト処理を記述する.
// Xcodeで追加したいLinkerやFrameworkが増えた場合はここへ記述する.
// ---------------------------------------------------------
public class XCodeBuildPostProcess {


    /// <summary>
    /// Build後の自動処理.
    /// </summary>
    [PostProcessBuild(100)]
    public static void OnPostProcessBuild(BuildTarget target, string path) {
        if (target == BuildTarget.iOS) {
            PostProcessBuildiOS(path);
        }
    }

    /// <summary>
    /// iOSのBuild後の自動処理.
    /// </summary>
    private static void PostProcessBuildiOS(string path) {
        Log.Notice("Xcodeのプロセス開始.");
        string projectPath = PBXProject.GetPBXProjectPath(path);
        PBXProject pbxProject = new PBXProject();

        pbxProject.ReadFromString(File.ReadAllText(projectPath));
        // string targetGuid = pbxProject.GetUnityFrameworkTargetGuid();

        // OtherLinker追加
        // 必要あれば↓のように記載.
        // pbxProject.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "追加したいLinker名");

        // Framework追加 第三引数は Requiredならtrue,Optionalならfalse
        // 必要あれば↓のように記載.
        // pbxProject.AddCapability(targetGuid, PBXCapabilityType.InAppPurchase); アプリ内課金
        // 他CRIや、SmartBeat, Firebase, Adjust等のプラグインを使う場合はフレームワーク追加が必要.

        // plistに情報追加.
        var plist = new PlistDocument();
        var plistPath = Path.Combine(path, "Info.plist");
        plist.ReadFromFile(plistPath);
        // 日本語設定にする.
        plist.root.SetString("CFBundleDevelopmentRegion", "ja");
        var localizationsArray = plist.root.CreateArray("CFBundleLocalizations");
        localizationsArray.AddString("ja");

        // Admob使用のために必要.
        // これらを追加しないとAdmob関連起動時やOSでATT利用承認ダイアログを出す際などにクラッシュする.
        var frameworks = pbxProject.TargetGuidByName("UnityFramework");
        pbxProject.AddFrameworkToProject(frameworks, "AppTrackingTransparency.framework", true);
        plist.root.SetString("NSUserTrackingUsageDescription", "本アプリでは広告が表示されますが、その際あなたの好みに合わせたものを表示するために使用されます");
        plist.root.SetBoolean("GADIsAdManagerApp", true);

        // 変更結果を上書きする.
        plist.WriteToFile(plistPath);
        File.WriteAllText(projectPath, pbxProject.WriteToString());

        Log.Notice("Xcodeのプロセス終了.");
    }
}
