using System;
using System.Linq;

namespace ScriptService.Extensions {

    /// <summary>
    /// extensions for type structures
    /// </summary>
    public static class TypeExtensions {

        /// <summary>
        /// get plain type name from an assembly named full type name
        /// </summary>
        /// <param name="typename">name of type to analyse</param>
        /// <returns>plain type name</returns>
        public static string GetPlainTypeName(this string typename) {
            return typename.Split(',')[0].Split('.').LastOrDefault();
        }

        /// <summary>
        /// get the type name including assembly name but not version information
        /// </summary>
        /// <param name="type">type for which to generate type name</param>
        /// <returns>name of type containing all info necessary to get type info from it</returns>
        public static string GetTypeName(this Type type) {
            if(type == null)
                return null;
            return $"{type.FullName},{type.Assembly.GetName().Name}";
        }

    }
}