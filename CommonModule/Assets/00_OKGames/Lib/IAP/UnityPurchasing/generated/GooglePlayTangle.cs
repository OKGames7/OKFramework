// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle {
        private static byte[] data = System.Convert.FromBase64String("WOl7yUjmYtK4i2NTKeyJb08Eu4xIlRDM6JW+JnwCNaBCW3+thB4hRMndWlvMmD50LT4ILfIIf9A3gZ1xV9Ta1eVX1N/XV9TU1WPdhvr5YmNdJP8Bn3k9ncDjyp+MuyNt7dH4Hmr/UPFBlUvfx5KjYdLA/GeWPRpfiA9byJTORXFW/GZLryStL7QAjWHO+9F74t/LHTjRhjqVYfxl7f0FEeVX1Pfl2NPc/1OdUyLY1NTU0NXWUuDnJ6Zm8pey/tCP4AYV8RPI5UnxBnvQ/HhWSh/kwlSp36+m0BcntUhXtpwjuf2pZMTzpDYeG3m9Bn0Kzm9NLkyusa7oyu7dJxIWTLIi1eyYWuMgh6ex7MXLaMGecUq42nwTCAlIgxvrX9t05tfW1NXU");
        private static int[] order = new int[] { 10, 4, 9, 4, 12, 12, 9, 9, 10, 9, 13, 11, 13, 13, 14 };
        private static int key = 213;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
            if (IsPopulated == false)
                return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
