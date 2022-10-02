using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// ランダム関数のユーティリティ.
    /// </summary>
    public class RandUtility {

        /// <summary>
        /// min ~ maxの間のint値を乱数で返す(minとmaxの値も含む).
        /// </summary>
        /// <param name="min">最小値.</param>
        /// <param name="max">最大値.</param>
        /// <returns>min ~ maxの乱数.</returns>
        public static int Range(int min, int max) {
            return Random.Range(min, max + 1);
        }
    }
}
