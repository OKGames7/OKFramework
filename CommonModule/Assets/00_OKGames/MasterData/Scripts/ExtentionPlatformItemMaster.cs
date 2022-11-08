using OKGamesFramework;
using OKGamesLib;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// PlatformItemマスターの拡張クラス.
/// </summary>
public static class ExtentionPlatformItemMaster {

    /// <summary>
    /// アイテムタイプ
    /// </summary>
    public enum ProductType {
        // 消費型.
        consumable,
        // 非消費型.
        nonconsumable,
        // サブスク.
        subscription,
    }

    /// <summary>
    /// プラットフォーム
    /// </summary>
    public enum Platform {
        // UnityEditor.
        editor,
        // iOS.
        apple,
        // Android.
        android,
    }


    /// <summary>
    /// 現在のプラットフォームのアイテムをproductIDから取得する.
    /// </summary>
    /// <param name="master">マスター.</param>
    /// <param name="productId">取得したい商品のID.</param>
    /// <param name="sheetIndex">excelの元データのシートページ. 1ページしかない想定.</param>
    /// <returns>該当レコード.</returns>
    public static Entity_platform_item.Param GetItemByCurrentPlatform(this Entity_platform_item master, string productId, int sheetIndex = 0) {
        var platformType = GetCurrentPlatformType();
        return master.GetItem(productId, platformType, sheetIndex);
    }

    /// <summary>
    /// レコードのタイトル文字を取得する.
    /// </summary>
    /// <param name="record">レコード.</param>
    /// <returns>タイトル文字.</returns>
    public static string GetTitle(this Entity_platform_item.Param record) {
        var masterStore = OKGames.Context.ResourceStore;
        var textMaster = masterStore.GetObj<Entity_text>(AssetAddress.AssetAddressEnum.texts.ToString());
        return textMaster.GetText(record.titleKey);
    }

    /// <summary>
    /// レコードの説明文字を取得する.
    /// </summary>
    /// <param name="record">レコード.</param>
    /// <returns>説明文字.</returns>
    public static string GetDiscription(this Entity_platform_item.Param record) {
        var masterStore = OKGames.Context.ResourceStore;
        var textMaster = masterStore.GetObj<Entity_text>(AssetAddress.AssetAddressEnum.texts.ToString());
        return textMaster.GetText(record.discriptionKey);
    }

    /// <summary>
    /// 現在のプラットフォームのアイテムを全て取得する.
    /// </summary>
    /// <param name="master">マスター.</param>
    /// <param name="sheetIndex">excelの元データのシートページ. 1ページしかない想定.</param>
    /// <returns>該当レコードのリスト.</returns>
    public static List<Entity_platform_item.Param> GetItemsByCurrentPlatform(this Entity_platform_item master, int sheetIndex = 0) {
        var platformType = GetCurrentPlatformType();
        return master.GetItems(platformType, sheetIndex);
    }

    /// <summary>
    /// 現在のプラットフォームのアイテムで指定した種類(消費型、非消費型)のレコードを全て取得する.
    /// </summary>
    /// <param name="master">マスター.</param>
    /// <param name="productType">アイテム種類.</param>
    /// <param name="sheetIndex">excelの元データのシートページ. 1ページしかない想定.</param>
    /// <returns>該当レコードのリスト.</returns>
    public static List<Entity_platform_item.Param> GetItemsByCurrentPlatform(this Entity_platform_item master, ProductType productType, int sheetIndex = 0) {
        var platformType = GetCurrentPlatformType();
        return master.GetItems(platformType, productType, sheetIndex);
    }

    /// <summary>
    /// マスターからStoreItemのリスト形式に変換する.
    /// </summary>
    /// <param name="list">マスターのリスト.</param>
    /// <returns>StoreItemのリスト.</returns>
    public static List<StoreItem> ConvertToStoreItem(this List<Entity_platform_item.Param> list) {
        string storeName = GetCurrentPlatformStoreName();
        List<StoreItem> itemList = new List<StoreItem>();
        for (int i = 0; i < list.Count; ++i) {
            StoreItem item = new StoreItem(storeName, list[i].productID, list[i].productType);
            itemList.Add(item);
        }

        return itemList;
    }

    /// <summary>
    /// 商品IDとプラットフォーム情報からレコードを取得する.
    /// </summary>
    /// <param name="master">マスター.</param>
    /// <param name="productId">取得したい商品のID.</param>
    /// <param name="type">取得したい商品のプラットフォーム.</param>
    /// <param name="sheetIndex">excelの元データのシートページ. 1ページしかない想定.</param>
    /// <returns>該当レコード.</returns>
    private static Entity_platform_item.Param GetItem(this Entity_platform_item master, string productId, Platform type, int sheetIndex = 0) {
        return master.sheets[sheetIndex].list.Find(data => (data.productID == productId) && (data.platform == type.ToString()));
    }

    /// <summary>
    /// 該当するプラットフォーム情報のレコードを全て取得する.
    /// </summary>
    /// <param name="master">マスター.</param>
    /// <param name="type">取得したい商品のプラットフォーム.</param>
    /// <param name="sheetIndex">excelの元データのシートページ. 1ページしかない想定.</param>
    /// <returns>該当レコードのリスト.</returns>
    private static List<Entity_platform_item.Param> GetItems(this Entity_platform_item master, Platform type, int sheetIndex = 0) {
        return master.sheets[sheetIndex].list.Where(data => data.platform == type.ToString()).ToList();
    }

    /// <summary>
    /// 該当するプラットフォームのアイテムで指定した種類(消費型、非消費型)のレコードを全て取得する.
    /// </summary>
    /// <param name="master">マスター.</param>
    /// <param name="type">取得したい商品のプラットフォーム.</param>
    /// <param name="productType">アイテム種類.</param>
    /// <param name="sheetIndex">excelの元データのシートページ. 1ページしかない想定.</param>
    /// <returns>該当レコードのリスト.</returns>
    private static List<Entity_platform_item.Param> GetItems(this Entity_platform_item master, Platform type, ProductType productType, int sheetIndex = 0) {
        return master.sheets[sheetIndex].list.Where(data => (data.platform == type.ToString()) && (data.productType == productType.ToString())).ToList();
    }

    /// <summary>
    /// 現在のプラットフォームのストア名を取得する.
    /// </summary>
    /// <returns>プラットフォームのストア名.</returns>
    private static string GetCurrentPlatformStoreName() {
        Platform platform = GetCurrentPlatformType();
        string storeName;
        storeName = (platform == Platform.android)
            ? UnityEngine.Purchasing.GooglePlay.Name
            : (platform == Platform.apple) ? UnityEngine.Purchasing.AppleAppStore.Name
            : "Editor";

        return storeName;
    }

    /// <summary>
    /// 現在のプラットフォーム情報を取得する.
    /// </summary>
    /// <returns>プラットフォーム情報.</returns>
    private static Platform GetCurrentPlatformType() {
        Platform platformType = Platform.editor;
#if UNITY_ANDROID && !UNITY_EDITOR
            platformType = Platform.android;
#elif UNITY_IPHONE && !UNITY_EDITOR
            platformType = Platform.apple;
#else
        // テストしやすいようにAndroidで返している.
        platformType = Platform.android;
#endif
        return platformType;
    }

}
