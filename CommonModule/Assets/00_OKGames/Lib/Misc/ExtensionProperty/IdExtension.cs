using System;

namespace OKGamesLib {

    /// <summary>
    /// IDに関する拡張メソッド.
    /// </summary>
    public class IdExtension {

        /// <summary>
        /// GUIDを16進数32桁の文字列(ハイフンなし)で生成し取得する.
        /// </summary>
        /// <returns></returns>
        public static string CreateGuid() {
            return System.Guid.NewGuid().ToString();
        }
    }
}
