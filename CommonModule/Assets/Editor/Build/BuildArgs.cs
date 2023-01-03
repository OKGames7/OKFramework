using OKGamesLib;
using System.Collections.Generic;
using UnityEditor;

// ---------------------------------------------------------
// ビルドでコマンドラインから引数取得し、そのデータを格納する
// ---------------------------------------------------------
public class BuildArgs {
    // アプリ出力するパス.
    public static string ExportAppPath { get; private set; }
    // アプリのバージョンコード.
    public static string AppVersionCode { get; private set; }
    // ビルドバージョン.
    public static string BuildVersion { get; private set; }
    // IOSのバージョンコード.
    public static string IOSVersionCode { get; private set; }
    // Androidのバージョンコード
    public static string AndroidVersionCode { get; private set; }

    // googlePlayStoreのkeystoreのパス.
    public static string KeyStorePath { get; private set; }
    // keystoreのパスはEditorで設定しても保持されないため変数に格納.
    public static string KeyAliasName { get; private set; }
    public static string KeyStorePass { get; private set; }
    public static string KeyAliasPass { get; private set; }

    // 開発用ビルドか.
    public static bool IsDevelopment { get; private set; }

    // FPSとメモリ量のデバッグ表示するか
    public static bool IsDebugSimpleProfileUI { get; private set; }

    // CommonModuleの機能をデバッグするシーンから始めるか
    public static bool IsStartCommonModuleDebugScene { get; private set; }

    // プラットフォームストアにUpする用か.
    public static bool IsUploadStore { get; private set; }


    /// <summary>
    /// コマンドラインから引数に渡した情報をメンバ変数へ格納する.
    /// </summary>

    public static void SetArgs(string[] args) {
        List<string> setVariabeList = new List<string>();
        // 引数によって固有にデータを扱って操作したい場合は以下へ追記する.
        for (int i = 0; i < args.Length; i++) {
            switch (args[i]) {
                case "-outputPath":
                    ExportAppPath = args[i + 1];
                    setVariabeList.Add("ExportAppPath: " + ExportAppPath);
                    break;
                case "-development":
                    IsDevelopment = true;
                    setVariabeList.Add("IsDevelopment :" + IsDevelopment.ToString());
                    break;
                case "-debugSimpleProfileUI":
                    IsDebugSimpleProfileUI = System.Convert.ToBoolean(args[i + 1]);
                    setVariabeList.Add("IsDebugSimpleProfiler :" + IsDebugSimpleProfileUI.ToString());
                    break;
                case "-debugCommonModule":
                    IsStartCommonModuleDebugScene = System.Convert.ToBoolean(args[i + 1]);
                    setVariabeList.Add("IsDebugSimpleProfiler :" + IsStartCommonModuleDebugScene.ToString());
                    break;
                case "-uploadStore":
                    IsUploadStore = System.Convert.ToBoolean(args[i + 1]);
                    setVariabeList.Add("IsUploadStore :" + IsUploadStore.ToString());
                    break;
                case "-appVersionCode":
                    AppVersionCode = args[i + 1];
                    setVariabeList.Add("AppVersionCode: " + AppVersionCode);
                    break;
                case "-buildVersion":
                    BuildVersion = args[i + 1];
                    setVariabeList.Add("BuildVersion: " + BuildVersion);
                    break;
                case "-androidVersionCode":
                    AndroidVersionCode = args[i + 1];
                    setVariabeList.Add("AndroidVersionCode: " + AndroidVersionCode);
                    break;
                case "-iOSVersionCode":
                    IOSVersionCode = args[i + 1];
                    setVariabeList.Add("iOSVersionCode: " + IOSVersionCode);
                    break;
                case "-keyStorePath":
                    KeyStorePath = args[i + 1];
                    setVariabeList.Add("KeyStorePath: " + KeyStorePath);
                    break;
                case "-keyStorePass":
                    KeyStorePass = args[i + 1];
                    setVariabeList.Add("KeyStorePass: " + KeyStorePass);
                    break;
                case "-keyAliasName":
                    KeyAliasName = args[i + 1];
                    setVariabeList.Add("KeyAliasName: " + KeyAliasName);
                    break;
                case "-keyAliasPass":
                    KeyAliasPass = args[i + 1];
                    setVariabeList.Add("KeyAliapPass: " + KeyAliasPass);
                    break;
                default:
                    break;
            }
        }
        Log.Notice("コマンドライン引数で設定した変数\n\t" + string.Join("\n\t", setVariabeList.ToArray()));
    }

    /// <summary>
    /// コマンドラインから引数に渡すべき情報が入っているかを確認する.
    /// </summary>
    public static bool Validation(BuildTarget target) {
        bool isOK;
        if (target == BuildTarget.Android) {
            isOK = BuildArgs.ExportAppPath != ""
                && BuildArgs.BuildVersion != ""
                && BuildArgs.AppVersionCode != ""
                && BuildArgs.AndroidVersionCode != ""
                && BuildArgs.KeyStorePath != ""
                && BuildArgs.KeyStorePass != ""
                && BuildArgs.KeyAliasName != ""
                && BuildArgs.KeyAliasPass != "";
        } else {
            isOK = BuildArgs.ExportAppPath != ""
                && BuildArgs.BuildVersion != ""
                && BuildArgs.AppVersionCode != ""
                && BuildArgs.AndroidVersionCode != ""
                && BuildArgs.IOSVersionCode != ""; ;
        }

        return isOK;
    }
}
