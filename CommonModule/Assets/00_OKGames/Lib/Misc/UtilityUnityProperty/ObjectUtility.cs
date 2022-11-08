using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OKGamesLib {

    /// <summary>
    /// オブジェクトに関する便利クラス.
    /// </summary>
    public class ObjectUtility {

        /// <summary>
        /// シーンのRootオブジェクトの内、指定した型のコンポーネントを取得する.
        /// </summary>
        /// <typeparam name="T">取得したいコンポーネントの型.</typeparam>
        /// <returns>コンポーネント.</returns>
        public static T GetComponentInSceneRootObjects<T>() {
            // ActiveなSceneのRootにあるGameObject[]を取得する
            var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            T component = default(T);
            foreach (var obj in rootGameObjects) {
                // includeInactive = true を指定するとGameObjectが非活性なものからも取得する
                component = obj.GetComponent<T>();
                if (component != null) {
                    break;
                }
            }
            return component;
        }
    }
}
