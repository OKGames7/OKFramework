using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// Consoleへ出力するログ.
    /// UnityEngine.Debug.Logのラッパー.
    /// </summary>
    public class Log {

        public const string COLOR_NOTICE = "ffffff";
        public const string COLOR_SUCCESS = "33ee00";
        public const string COLOR_WARN = "ff9900";
        public const string COLOR_ERROR = "ff3322";

        [Conditional("DEVELOPMENT")]
        public static void Notice(object message, Object context = null, string color = COLOR_NOTICE) {
            UnityEngine.Debug.Log($"<color=#{color}>{message}</color>", context);
        }

        [Conditional("DEVELOPMENT")]
        public static void Success(object message, Object context = null, string color = COLOR_SUCCESS) {
            UnityEngine.Debug.Log($"<color=#{color}>{message}</color>", context);
        }

        [Conditional("DEVELOPMENT")]
        public static void Warning(object message, Object context = null, string color = COLOR_WARN) {
            UnityEngine.Debug.LogWarning($"<color=#{color}>{message}</color>", context);
        }

        [Conditional("DEVELOPMENT")]
        public static void Error(object message, Object context = null, string color = COLOR_ERROR) {
            UnityEngine.Debug.LogError($"<color=#{color}>{message}</color>\n", context);
        }

        //----------------------------------------------------------------------
        // Utilities
        //----------------------------------------------------------------------

        [Conditional("DEVELOPMENT")]
        public static void Clear() {
#if UNITY_EDITOR
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
#endif
        }

        [Conditional("DEVELOPMENT")]
        public static void DumpList<T>(IEnumerable<T> list, Object context = null, string color = COLOR_NOTICE) {
            string output = "";
            int index = 0;
            foreach (var item in list) {
                output += $"[{index}] : {item.ToString()}　";
                ++index;
            }
            UnityEngine.Debug.Log($"<color=#{color}>{output}</color>", context);
        }

        [Conditional("DEVELOPMENT")]
        public static void DumpDictionary<T1, T2>(Dictionary<T1, T2> dictionary, Object context = null, string color = COLOR_NOTICE) {
            string output = "";
            foreach (KeyValuePair<T1, T2> kv in dictionary) {
                output += $"{kv.Key} : {kv.Value}　";
            }
            UnityEngine.Debug.Log($"<color=#{color}>{output}</color>", context);
        }
    }
}

