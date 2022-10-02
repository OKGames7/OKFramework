using System;

namespace OKGamesLib {

    /// <summary>
    /// Enum型の拡張メソッド.
    /// </summary>
    public class EnumExtension {

        /// <summary>
        /// intの値からその要素のEnum値を取得する.
        /// </summary>
        /// <typeparam name="T">Enum型</typeparam>
        /// <param name="value">取得したい要素.</param>
        /// <returns></returns>
        public static T FromInt<T>(int value) where T : struct {
            return (T)Enum.ToObject(typeof(T), value);
        }


        /// <summary>
        /// stringの値からその要素のEnum値を取得する.
        /// </summary>
        /// <typeparam name="T">Enum型./typeparam>
        /// <param name="str">取得したい要素の文字列.</param>
        /// <returns></returns>
        public static T FromNumericString<T>(string str) where T : struct {
            if (str == string.Empty) {
                return EnumExtension.FromInt<T>(0);
            }

            try {
                T value = (T)Enum.ToObject(typeof(T), int.Parse(str));
                return value;
            }
            catch (Exception ex) {
                Log.Error($"[EnumExtension] Parse error: {str}");
                throw ex;
            }
        }

        /// <summary>
        /// Enumの定義名と同じ文字列からEnumの値を取得する.
        /// </summary>
        /// <typeparam name="T">Enum型.</typeparam>
        /// <param name="str">取得したい変数名.</param>
        /// <returns>Enum値.</returns>
        public static T FromString<T>(string str) where T : struct {
            T result;
            bool parsed = Enum.TryParse(str, out result) && Enum.IsDefined(typeof(T), result);
            if (!parsed) {
                Log.Error($"[EnumUtil] Parse error : {str}");
            }
            return result;
        }
    }
}
