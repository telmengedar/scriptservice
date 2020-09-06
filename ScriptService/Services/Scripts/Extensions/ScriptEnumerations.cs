using System.Collections;
using System.Linq;
using NightlyCode.Scripting.Providers;
using ScriptService.Extensions;

namespace ScriptService.Services.Scripts.Extensions {

    /// <summary>
    /// extensions for enumerations in script
    /// </summary>
    public class ScriptEnumerations {

        /// <summary>
        /// determines whether any item in the collection matches the specified predicate
        /// </summary>
        /// <param name="collection">collection to analyse</param>
        /// <param name="predicate">predicate to execute</param>
        /// <returns>true if any item matches the predicate, false otherwise</returns>
        public static bool Any(IEnumerable collection, LambdaMethod predicate) {
            return collection.Cast<object>().Any(i => predicate.Invoke(i).ToBoolean());
        }

        /// <summary>
        /// determines whether any item in the collection matches the specified predicate
        /// </summary>
        /// <param name="collection">collection to analyse</param>
        /// <param name="predicate">predicate to execute</param>
        /// <returns>true if any item matches the predicate, false otherwise</returns>
        public static bool All(IEnumerable collection, LambdaMethod predicate) {
            return collection.Cast<object>().All(i => predicate.Invoke(i).ToBoolean());
        }

        /// <summary>
        /// transforms an enumeration using a selector
        /// </summary>
        /// <param name="enumeration">enumeration to transform</param>
        /// <param name="selector">selector to use to transform selection</param>
        /// <returns>resulting enumeration</returns>
        public static IEnumerable Select(IEnumerable enumeration, LambdaMethod selector) {
            return enumeration.Cast<object>().Select(i => selector.Invoke(i));
        }

        /// <summary>
        /// transforms an enumeration using a selector
        /// </summary>
        /// <param name="enumeration">enumeration to transform</param>
        /// <param name="selector">selector to use to transform selection</param>
        /// <returns>resulting enumeration</returns>
        public static IEnumerable SelectMany(IEnumerable enumeration, LambdaMethod selector) {
            return enumeration.Cast<object>().SelectMany(i => {
                object value = selector.Invoke(i);
                if (value is IEnumerable selection)
                    return selection.Cast<object>();
                return new[]{value};
            });
        }

        /// <summary>
        /// filters an enumeration using a predicate
        /// </summary>
        /// <param name="enumeration">enumeration to filter</param>
        /// <param name="predicate">filter predicate</param>
        /// <returns>filtered enumeration</returns>
        public static IEnumerable Where(IEnumerable enumeration, LambdaMethod predicate) {
            return enumeration.Cast<object>().Where(i => predicate.Invoke(i).ToBoolean());
        }

        /// <summary>
        /// get first item of enumeration or null if enumeration contains no elements
        /// </summary>
        /// <param name="enumeration">enumeration of which to return first element</param>
        /// <returns>first element of enumeration or null if no element is in enumeration</returns>
        public static object FirstOrDefault(IEnumerable enumeration) {
            return enumeration.Cast<object>().FirstOrDefault();
        }

        /// <summary>
        /// get first item of enumeration which matches the predicate or null if no element matches the predicate
        /// </summary>
        /// <param name="enumeration">enumeration of which to return first element</param>
        /// <param name="predicate">predicate for element to match</param>
        /// <returns>first element of enumeration which matches the predicate or null if no element matches</returns>
        public static object FirstOrDefault(IEnumerable enumeration, LambdaMethod predicate) {
            return enumeration.Cast<object>().FirstOrDefault(i=>predicate.Invoke(i).ToBoolean());
        }

        /// <summary>
        /// skips a number of items in an enumeration and returns the remaining items
        /// </summary>
        /// <param name="enumeration">enumeration to process</param>
        /// <param name="count">number of items to skip</param>
        /// <returns>enumeration of remaining items</returns>
        public static IEnumerable Skip(IEnumerable enumeration, int count) {
            return enumeration.Cast<object>().Skip(count);
        }

        /// <summary>
        /// returns a number of items from an enumeration
        /// </summary>
        /// <param name="enumeration">enumeration to process</param>
        /// <param name="count">number of items to return</param>
        /// <returns>enumeration of items taken</returns>
        public static IEnumerable Take(IEnumerable enumeration, int count) {
            return enumeration.Cast<object>().Take(count);
        }
    }
}