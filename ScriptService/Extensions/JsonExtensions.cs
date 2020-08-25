using System;
using Utf8Json;
using Utf8Json.Resolvers;

namespace ScriptService.Extensions {

    /// <summary>
    /// extensions for json data handling
    /// </summary>
    public static class JsonExtensions {

        /// <summary>
        /// deserializes a json string
        /// </summary>
        /// <remarks>
        /// this handles null values and automatically suppresses all errors
        /// </remarks>
        /// <typeparam name="T">type of data to deserialize</typeparam>
        /// <param name="data">data to deserialize</param>
        /// <returns>deserialized data (or default if any error occurs while deserializing)</returns>
        public static T Deserialize<T>(this string data) {
            if(string.IsNullOrEmpty(data))
                return default;

            try {
                return JsonSerializer.Deserialize<T>(data, StandardResolver.ExcludeNullCamelCase);
            }
            catch(Exception) {
                return default;
            }
        }

        /// <summary>
        /// serializes data to a json string
        /// </summary>
        /// <param name="data">data to serialize</param>
        /// <returns>serialized data</returns>
        public static string Serialize(this object data) {
            if (data == null)
                return null;
            return JsonSerializer.ToJsonString(data, StandardResolver.ExcludeNullCamelCase);
        }
    }
}