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

    [PostProcessBuildAttribute(45)]//must be between 40 and 50 to ensure that it's not overriden by Podfile generation (40) and that it's added before "pod install" (50)
    private static void PostProcessBuildiOS(BuildTarget target, string buildPath) {
        if (target == BuildTarget.iOS) {

            using (StreamWriter sw = File.AppendText(buildPath + "/Podfile")) {
                //in this example I'm adding an app extension
                sw.WriteLine("post_install do |installer|");
                sw.WriteLine("installer.pods_project.build_configurations.each do |config|");
                sw.WriteLine("config.build_settings['PROVISIONING_PROFILE_SPECIFIER'] = ''");
                sw.WriteLine("config.build_settings['CODE_SIGNING_REQUIRED'] = 'NO'");
                sw.WriteLine("config.build_settings['CODE_SIGNING_ALLOWED'] = 'NO'");
                sw.WriteLine("end");
                sw.WriteLine("end");
            }
        }
    }

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
        string targetGuid = pbxProject.GetUnityFrameworkTargetGuid();

        // OtherLinker追加
        // 必要あれば↓のように記載.
        // pbxProject.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "追加したいLinker名");

        // plistに情報追加.
        var plist = new PlistDocument();
        var plistPath = Path.Combine(path, "Info.plist");
        plist.ReadFromFile(plistPath);
        // 日本語設定にする.
        plist.root.SetString("CFBundleDevelopmentRegion", "ja");
        var localizationsArray = plist.root.CreateArray("CFBundleLocalizations");
        localizationsArray.AddString("ja");

        // アプリ内課金の有効設定.
        pbxProject.AddCapability(targetGuid, PBXCapabilityType.InAppPurchase);

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
