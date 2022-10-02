using OKGamesFramework;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesLib {

    // ---------------------------------------------------------
    // ストレージへ保存するセーブデータを取り扱う.
    // セーブデータは、JSON形式のデータをbyteに変換しAESで暗号化した内容で独自にファイルを作成する形式を取る。
    // 尚、暗号キーはプロジェクト毎に用意すること.
    // ファイルの保存先はApplication.persistentDataPath + /save.bytesの階層以下.指定したkeyを基に暗号化かけたファイル名を作りそのファイルの中に暗号化をかけたじつデータを格納する.
    // PlayerPrefsを使わない理由.
    //  ・保存できるデータの型がint, float, stringのみで独自の型が保存できず使い勝手がよくない.
    //  ・Windowsで保存先がレジストリになり、レジストリは他の消すとマズイデータが多い管理場所なので怖い.
    //  ・処理負荷が高い
    // ---------------------------------------------------------
    public class StorageUtility {

        // セーブデータを保存する階層.
        //　Windows:   C:\Users\User(ユーザーネーム)\AppData\LocalLow\DefaultCompany（プロジェクトカンパニー）\(プロジェクト)
        //  Mac    :    /Users/(ユーザーネーム)/Library/Application Support/（プロジェクトカンパニー）/(プロジェクト)/saveData
        private static readonly string _saveFilePath = Application.persistentDataPath + "/saveData/";


        /// <summary>
        /// キーを基にセーブデータを読み込む
        /// </summary>
        public static object Load(string key) {
            // セーブデータのファイルまでのパスを取得する.
            string path = GetEncryptSaveFilePath(key);
            // 返却するobjectデータ.
            object outData = null;

            try {
                using (CryptoFileStream cfs = new CryptoFileStream(path, FileMode.Open, FileAccess.Read)) {
                    IFormatter formatter = new BinaryFormatter();
                    outData = formatter.Deserialize(cfs);
                }
            }
            catch (FileNotFoundException e) {
                Log.Error(e);
                outData = null;
            }

            return outData;
        }

        /// <summary>
        /// キーを基にセーブデータを読み込む(非同期).
        /// </summary>
        public static async UniTask SaveAsync(string key, object data) {
            var path = GetEncryptSaveFilePath(key);
            await UniTask.RunOnThreadPool(() => Save(key, data));

#if UNITY_IOS && !UNITY_EDITOR
            SetNoBackUpOSFlag(path);
#endif
        }

        /// <summary>
        /// 指定したkeyでデータをローカルに保存する(同期).
        /// </summary>
        public static void Save(string key, object data) {
            string path = GetEncryptSaveFilePath(key);

            // セーブデータ破損があった場合に備えて、復旧用に用意するバックアップのファイルパス.
            string backupPath = $"{path}_backup";
            // 格納先フォルダ名.
            string directoryName = Path.GetDirectoryName(path);

            try {
                if (!Directory.Exists(directoryName)) {
                    // 今回格納するセーブデータを保存するフォルダ先がまだなければフォルダを作成する.
                    Directory.CreateDirectory(directoryName);
                }

                if (File.Exists(path)) {
                    // 保存前に、万が一データが破損があった場合に備えて、バックアップをとっておく.
                    File.Copy(path, backupPath, true);
                }

                using (CryptoFileStream cfs = new CryptoFileStream(path, FileMode.Create, FileAccess.Write)) {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(cfs, data);
                }
            }
            catch {
                if (File.Exists(backupPath)) {
                    // エラーをキャッチした場合はバックアップからデータを復元する.
                    File.Copy(backupPath, path, true);
                }
                throw;
            }

#if UNITY_IOS && !UNITY_EDITOR
            SetNoBackUpOSFlag(path);
#endif
        }

        /// <summary>
        /// 指定キーのファイルを削除する.
        /// </summary>
        public static void Delete(string key) {
            string path = GetEncryptSaveFilePath(key);
            if (File.Exists(path)) {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 全セーブデータを削除する.
        /// 削除を除外するキーをexcluedKeysで複数指定できる.
        /// </summary>
        public static void DeleteAll(string[] excludeKeys = null) {
            string[] excluedFilePaths = excludeKeys?.Select(GetEncryptSaveFilePath).ToArray();
            string[] deleteFilesPaths = Directory.GetFiles(_saveFilePath);
            foreach (string filePath in deleteFilesPaths) {
                if ((excluedFilePaths != null) && excluedFilePaths.Contains(filePath)) {
                    continue;
                }
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// 指定キーのファイルが存在するか.
        /// </summary>
        public static bool Exists(string key) {
            string path = GetEncryptSaveFilePath(key);
            return File.Exists(path);
        }

        /// <summary>
        /// 暗号化されたファイルへのパスを取得する.
        /// keyごとにセーブするファイルは分けている.
        /// ファイル名がkey名のままだとセーブデータが破られたときに簡単に操作できてしまうのでファイル名も暗号化している).
        /// </summary>
        private static string GetEncryptSaveFilePath(string key) {
            return $"{_saveFilePath}{Crypto.Encrypt(key)}";
        }

#if UNITY_IOS && !UNITY_EDITOR
        /// <summary>
        /// iOSだとApplication.persistentDataPathはiCloudでバックアップされる領域なので、そのままにすると審査に落ちる(規約2.23).
        /// 規約2.23 → 外部からファイルダウンロードするときはiCloudのバックアップ対象外にする必要がある.
        /// </summary>
        /// <param name="path"></param>
        private static void SetNoBackUpOSFlag(string path) {
            UnityEngine.iOS.Device.SetNoBackupFlag(path);
        }
#endif
    }
}
