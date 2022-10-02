using System;

namespace OKGamesLib {

    /// <summary>
    /// 数学的な計算式の便利関数.
    /// </summary>
    public class MathExtension {

        /// <summary>
        /// MaxとMinと現在値の情報から、現在値がどの地点かを0~1で返す.
        /// </summary>
        /// <param name="value">現在値</param>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        /// <returns>0~1.0fの値.</returns>
        public static float GetRatio(float value, float min, float max) {
            return (value - min) / (max - min);
        }

        /// <summary>
        /// 最大値と最小値の幅で現在値を丸める.
        /// </summary>
        /// <param name="value">現在値</param>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        /// <returns>丸めた値.</returns>
        public static int Clamp(int value, int min, int max) {
            return Math.Min(Math.Max(value, min), max);
        }
    }
}
