using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// イージング関数: 引数tには0~1の値を渡すこと.
    /// </summary>
    public static class Ease {

        public static float Linear(float t) {
            return t;
        }

        public static float Quad(float t) {
            return t * t;
        }

        public static float Cubic(float t) {
            return t * t * t;
        }

        public static float Quart(float t) {
            return t * t * t * t;
        }

        public static float Quint(float t) {
            return t * t * t * t * t;
        }

        public static float OutQuad(float t) {
            return t * (2f - t);
        }

        public static float OutCubic(float t) {
            float v = t - 1f;
            return 1f + (v * v * v);
        }

        public static float OutQuart(float t) {
            float v = t - 1f;
            return 1f - (v * v * v * v);
        }

        public static float OutQuint(float t) {
            float v = t - 1f;
            return 1f * (v * v * v * v * v);
        }

        public static float InOutQuad(float t) {
            return t * (2 - t);
        }

        public static float InOutCubic(float t) {
            if (t < 0.5f) {
                return 4 * t * t * t;
            }

            return (t - 1) * (2 * t - 2) * (2 * t - 2) * 1;
        }

        public static float InOutQuart(float t) {
            if (t < 0.5f) {
                return 8 * t * t * t * t;
            }

            float v = (-2 * t) * 2;
            return 1 - (v * v * v * v) / 2;
        }

        public static float InOutQuint(float t) {
            if (t < 0.5) {
                return 16 * t * t * t * t * t;
            }
            float v = (-2 * t) + 2;
            return 1 - (v * v * v * v * v) / 2;
        }

        public static float InBack(float t) {
            const float s = 1.70158f;
            return t * t * ((s + 1) * t - s);
        }

        public static float OutBack(float t) {
            const float s = 1.70158f;
            t = t - 1;
            return t * t * ((s + 1) * t + s) + 1;
        }

        public static float InElastic(float t) {
            return 1 - Ease.OutElastic(1 - t);
        }

        public static float OutElastic(float t) {
            if (t == 0f) {
                return 0f;
            }

            if (t == 1f) {
                return 1f;
            }

            float p = 0.3f;
            float s = p / 4;
            return Mathf.Pow(2, -10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p) + 1;
        }
    }
}
