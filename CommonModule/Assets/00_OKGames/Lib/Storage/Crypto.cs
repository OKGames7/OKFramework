using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace OKGamesLib {

    // ---------------------------------------------------------
    // AES形式での暗号化、復号機能を持つクラス.
    // IVとKeyの情報はPJ毎に変更すること.
    // ---------------------------------------------------------
    public class Crypto {

        /// <summary>
        /// 平文を暗号化して返す.
        /// </summary>
        public static string Encrypt(string data) {
            // 暗号化したい情報をbyte配列に変換.
            byte[] bytes = Encoding.UTF8.GetBytes(data);

            byte[] encryptedBytes;
            using (AesCryptoServiceProvider aes = CreateAesProvier()) {
                using (ICryptoTransform encrypt = aes.CreateEncryptor()) {
                    // byte配列を基にAES形式で暗号化を行う.
                    encryptedBytes = encrypt.TransformFinalBlock(bytes, 0, bytes.Length);
                }
            }
            // 暗号化した情報をBase64形式の文字列へ変換して返却する.
            return Uri.EscapeDataString(Convert.ToBase64String(encryptedBytes));
        }

        /// <summary>
        /// 複号したデータを返す.
        /// </summary>
        public static string Decrypt(string data) {
            // Base64形式の文字列をbyte配列に変換する.
            byte[] bytes = Convert.FromBase64String(Uri.UnescapeDataString(data));

            byte[] decryptedBytes;
            using (AesCryptoServiceProvider aes = CreateAesProvier()) {
                using (ICryptoTransform decrypt = aes.CreateDecryptor()) {
                    // 暗号化されているbyte配列を複合したbyte配列に変換する.
                    decryptedBytes = decrypt.TransformFinalBlock(bytes, 0, bytes.Length);
                }
            }
            // 複合した情報を文字列にして返却する.
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// 暗号化と複合に必要なプロバイダを生成、初期化し、返却する.
        /// </summary>
        private static AesCryptoServiceProvider CreateAesProvier() {
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.IV = Encoding.UTF8.GetBytes(SystemConst.AesIv);
            aes.Key = Encoding.UTF8.GetBytes(SystemConst.AesKey);
            aes.Padding = PaddingMode.PKCS7;
            return aes;
        }
    }
}
