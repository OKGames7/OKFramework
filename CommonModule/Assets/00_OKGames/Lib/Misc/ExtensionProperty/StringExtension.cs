using System;
using System.Linq;

namespace OKGamesLib {
    /// <summary>
    /// Stringの拡張クラス.
    /// </summary>
    public class StringExtension {

        /// <summary>
        /// 指定数繰り返した文字を返す.
        /// </summary>
        /// <param name="str"><繰り返す文字列./param>
        /// <param name="times">繰り返す回数.</param>
        /// <returns></returns>
        public static string Repeat(string str, int times) {
            if (times == 0) {
                return String.Empty;
            }

            return string.Concat(Enumerable.Repeat(str, times));
        }
    }
}
