using System;
using System.Linq;
using System.Reflection;

namespace OKGamesLib {

    /// <summary>
    /// Object型の拡張メソッド.
    /// </summary>
    public class ObjectExtension {

        /// <summary>
        /// オブジェクトのフィールド内の情報を別オブジェクトのフィールドへ流し込む.
        /// </summary>
        /// <param name="from">流し込む元のObject.</param>
        /// <param name="to">流し込む先のObject.</param>
        /// <returns></returns>
        public static bool CopyFields(Object from, Object to) {
            var fromFields = from.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
            var toFields = to.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();

            if (fromFields.Count == 0) {
                Log.Error($"[ObjectExtension] FieldInfo of from-object is null.");
                return false;
            }
            if (toFields.Count == 0) {
                Log.Error($"[ObjectExtension] FieldInfo of to-object is null.");
                return false;
            }

            foreach (var toField in toFields) {
                var fromField = fromFields.Find(field => {
                    return field.Name == toField.Name
                        && field.MemberType == toField.MemberType;
                });

                if (fromField == null) {
                    Log.Error($"[ObjectExtension] Field not found : {toField.Name}");
                    return false;
                }

                object fromValue = fromField.GetValue(from);
                toField.SetValue(to, fromValue);
            }
            return true;
        }

        /// <summary>
        /// オブジェクトのプロパティ内の情報を別オブジェクトのプロパティへ流し込む.
        /// </summary>
        /// <param name="from">流し込む元のObject.</param>
        /// <param name="to">流し込む先のObject.</param>
        public static bool CoyProps(Object from, Object to) {
            var fromProps = from.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            var toProps = to.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

            if (fromProps.Count == 0) {
                Log.Error($"[ObjectUtil] PropertyInfo of from-object is null.");
                return false;
            }
            if (toProps.Count == 0) {
                Log.Error($"[ObjectUtil] PropertyInfo of to-object is null.");
                return false;
            }

            foreach (var toProp in toProps) {
                var fromProp = fromProps.Find(prop => {
                    return prop.Name == toProp.Name
                        && prop.PropertyType == toProp.PropertyType;
                });
                if (fromProp == null) {
                    Log.Error($"[ObjectUtil] Property not found : {toProp.Name}");
                    return false;
                }

                object fromValue = fromProp.GetValue(from, null);
                toProp.SetValue(to, fromValue, null);
            }
            return true;
        }
    }
}
