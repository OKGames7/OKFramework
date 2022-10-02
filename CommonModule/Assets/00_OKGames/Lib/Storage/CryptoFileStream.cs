using System;
using System.IO;
using System.Security.Cryptography;

namespace OKGamesLib {

    // ---------------------------------------------------------
    // 暗号化/復号化されたファイルのストリーム.
    // ---------------------------------------------------------
    public class CryptoFileStream : Stream {

        private CryptoStream _cryptoStream = null;
        private long _length;

        // 暗号処理の際に必要なパラメータ.
        private const int _saltLength = 16;
        private const int _ivLength = 16;
        private const int _blockSize = _ivLength << 3;
        private const int _keySize = _saltLength << 3;
        private const int _headerSize = _saltLength + _ivLength;

        /// 現在位置のセッター/ゲッター.
        public override long Position {
            get { return _cryptoStream.Position; }
            set { _cryptoStream.Position = value; }
        }

        // Streamの状態がわかるbool値.
        public override bool CanSeek => _cryptoStream.CanSeek;
        public override bool CanRead => _cryptoStream.CanRead;
        public override bool CanWrite => _cryptoStream.CanWrite;
        public override long Length => _length;

        /// <summary>
        /// 外部公開するコンストラクタ.
        /// </summary>
        public CryptoFileStream(string path, FileMode mode, FileAccess access) : this(
            new FileStream(path, mode, access)) {
        }

        /// <summary>
        /// 内部コンストラクタ.
        /// </summary>
        private CryptoFileStream(Stream fileStream)
            : this(
                fileStream,
                fileStream.CanRead ? CryptoStreamMode.Read : CryptoStreamMode.Write) {
        }

        /// <summary>
        /// 内部コンストラクタ.
        /// </summary>
        private CryptoFileStream(Stream fileStream, CryptoStreamMode mode) {
            // 暗号キー, 復号キーのヘッダーサイズを格納する.
            byte[] header = new byte[_headerSize];
            // 暗号化に必要なパスワード
            string password = SystemConst.CryptoFilePassword;
            // 暗号化のための定義データ.
            ICryptoTransform cryptoTransform;

            if (mode == CryptoStreamMode.Write) {
                // 書き込みの場合.

                //暗号キーを書き込む.
                // headerには暗号化されたデータがrefで返ってくる.
                cryptoTransform = CreateAesEncryptor(header, password);
                fileStream.Write(header, 0, header.Length);
                _length = header.Length;
            } else {
                // 読み込みの場合.

                //復号キーを読み込む.
                fileStream.Read(header, 0, header.Length);
                // headerには暗号化されたデータがrefで返ってくる.
                cryptoTransform = CreateAesDecryptor(header, password);
                _length = fileStream.Length - header.Length;
            }
            // 暗号/復号した情報を書き込む/読み取る
            _cryptoStream = new CryptoStream(fileStream, cryptoTransform, mode);

            byte[] bytes = new byte[16];
            if (mode == CryptoStreamMode.Write) {
                // 書き込みの場合.
                Write(bytes, 0, bytes.Length);
            } else {
                // 読み込みの場合.
                Read(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// 暗号情報の生成.
        /// </summary>
        private static ICryptoTransform CreateAesEncryptor(byte[] header, string password) {
            using (var aes = new RijndaelManaged()) {
                SetupRijndealManaged(aes);

                Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, _saltLength);
                Array.Copy(deriveBytes.Salt, 0, header, 0, deriveBytes.Salt.Length);

                // KeyとIVを設定.
                aes.Key = deriveBytes.GetBytes(_saltLength);
                aes.GenerateIV();
                // IVの情報をheaderにコピーする.
                Array.Copy(aes.IV, 0, header, aes.Key.Length, aes.IV.Length);
                // 暗号情報の生成.
                return aes.CreateEncryptor(aes.Key, aes.IV);
            }
        }

        /// <summary>
        /// 復号情報の生成.
        /// </summary>
        static ICryptoTransform CreateAesDecryptor(byte[] header, string password) {
            using (var aes = new RijndaelManaged()) {
                SetupRijndealManaged(aes);
                // IVの長さを定義.
                aes.IV = new byte[_ivLength];
                // 暗号の長さを定義.
                var salt = new byte[_saltLength];
                // headerから暗号分の情報を抜き取りをsaltにコピーする.
                Array.Copy(header, 0, salt, 0, salt.Length);
                // 暗号化に必要なキー情報を取得する.
                var deriveBytes = new Rfc2898DeriveBytes(password, salt);
                aes.Key = deriveBytes.GetBytes(_saltLength);
                // 暗号化とセーブデータがまざった情報から暗号部の長さまでをIVへ格納する.
                Array.Copy(header, salt.Length, aes.IV, 0, aes.IV.Length);
                // 復号情報の生成.
                return aes.CreateDecryptor(aes.Key, aes.IV);
            }
        }

        /// <summary>
        /// RijindeaManagerのセットアップに必要な情報を代入する.
        /// </summary>
        private static void SetupRijndealManaged(RijndaelManaged aes) {
            aes.BlockSize = _blockSize;
            aes.KeySize = _keySize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
        }

        /// <summary>
        /// 長さの設定.
        /// </summary>
        public override void SetLength(long length) {
            _cryptoStream.SetLength(length);
        }

        /// <summary>
        /// 検索する.
        /// </summary>
        public override long Seek(long offset, SeekOrigin origin) {
            return _cryptoStream.Seek(offset, origin);
        }

        /// <summary>
        /// 読み込む.
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count) {
            return _cryptoStream.Read(buffer, offset, count);
        }
        /// <summary>
        /// 書き込む.
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count) {
            _cryptoStream.Write(buffer, offset, count);
            _length += count;
        }

        /// <summary>
        /// 閉じる.
        /// </summary>
        protected override void Dispose(bool disposing) {
            // マネージ(.NET Frameworkが関与する)リソースの破棄はせず(GCでいつか家宝される)、非管理リソースの破棄を行う.
            _cryptoStream.Dispose();
        }

        /// <summary>
        /// 全ての送信文字を送信し、送信バッファをクリアする.
        /// </summary>
        public override void Flush() {
            _cryptoStream.Flush();
        }
    }
}
