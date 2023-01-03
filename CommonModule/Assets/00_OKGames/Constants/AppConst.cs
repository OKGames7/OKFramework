using UnityEngine;
// ---------------------------------------------------------
// アプリケーションに関する定数.
// PJ側のソースコードなので本来は01_PJフォルダ以下で管理したいがアセンブリの参照関係でややこしくなるので00_OKGames以下へ置いている.
// ---------------------------------------------------------
public class AppConst {
    // ゲームのFPS.
    public const int FrameRate = 30;

    // 画面解像度(横 x 縦).
    public static readonly Vector2 Resolution = new Vector2(1080.0f, 1920.0f);

    // アプリ名. パスカルケースにすること
    //  ※iOSではアンダーバーが使えない(アンダーバー部分がハイフンに置き換わる)
    //  ※Androidではアンダーバーが使えない(アンダーバー部分が削除される)
    public const string AppName = "CommonModule";
    // アプリ名(開発用). パスカルケースにすること.
    public const string AppNameDevelopment = "DevelopmentCommonModule";

    public const string AdmobBannerIDIOS = "";
    public const string AdmobBannerIDAndroid = "";

    public const string AdmobInterstitialIDIOS = "";
    public const string AdmobInterstitialIDAndroid = "";

    public const string AppleStoreAppURL = "";
    public const string GoogleStoreAppURL = "";

}
