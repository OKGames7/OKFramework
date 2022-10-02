using UnityEngine;
using UnityEditor;

namespace OKGamesLib {

    /// <summary>
    /// Finderに関する機能を助長するクラス.
    /// </summary>
    public class FinderHelper {

        /// <summary>
        /// PCのOSに合わせてpersistentDataPathのFinder/エクスプローラーを開く.
        /// </summary>
        [MenuItem("o.k.games/99.UnityEditor/Open Persistent Data Path")]
        private static void OpenPersistentDataPath() {
            if (Application.platform == RuntimePlatform.OSXEditor) {
                System.Diagnostics.Process.Start(Application.persistentDataPath);
            } else if (Application.platform == RuntimePlatform.WindowsEditor) {
                EditorUtility.RevealInFinder(Application.persistentDataPath);
            }
        }
    }
}




