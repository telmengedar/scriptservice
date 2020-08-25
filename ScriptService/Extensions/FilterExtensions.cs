using System;
using System.Text;
using NightlyCode.AspNetCore.Services.Data;
using NightlyCode.Database.Entities.Operations;
using NightlyCode.Database.Fields;

namespace ScriptService.Extensions {

    /// <summary>
    /// extensions for filters
    /// </summary>
    public static class FilterExtensions {

        /// <summary>
        /// applies a standard list filter to a load operation
        /// </summary>
        /// <typeparam name="T">type of entities to load</typeparam>
        /// <param name="operation">operation to modify</param>
        /// <param name="filter">filter to apply</param>
        /// <returns>load operation for fluent behavior</returns>
        public static LoadEntitiesOperation<T> ApplyFilter<T>(this LoadEntitiesOperation<T> operation, ListFilter filter) {
            if(!filter.Count.HasValue || filter.Count > 500)
                filter.Count = 500;
            if(filter.Count <= 0)
                throw new ArgumentException($"A count of '{filter.Count}' makes no sense", nameof(filter.Count));

            operation.Limit(filter.Count.Value);
            if(filter.Continue.HasValue)
                operation.Offset(filter.Continue.Value);
            if(!string.IsNullOrEmpty(filter.Sort))
                operation.OrderBy(new OrderByCriteria(Field.Property<T>(filter.Sort, true), !filter.Descending));
            return operation;
        }

        /// <summary>
        /// applies a standard list filter to a load operation
        /// </summary>
        /// <typeparam name="T">type of entities to load</typeparam>
        /// <param name="operation">operation to modify</param>
        /// <param name="filter">filter to apply</param>
        /// <returns>load operation for fluent behavior</returns>
        public static LoadValuesOperation<T> ApplyFilter<T>(this LoadValuesOperation<T> operation, ListFilter filter) {
            if(!filter.Count.HasValue || filter.Count > 500)
                filter.Count = 500;
            if(filter.Count <= 0)
                throw new ArgumentException($"A count of '{filter.Count}' makes no sense", nameof(filter.Count));

            operation.Limit(filter.Count.Value);
            if(filter.Continue.HasValue)
                operation.Offset(filter.Continue.Value);
            if(!string.IsNullOrEmpty(filter.Sort))
                operation.OrderBy(new OrderByCriteria(Field.Property<T>(filter.Sort, true), !filter.Descending));
            return operation;
        }

        /// <summary>
        /// translates query wildcards
        /// </summary>
        /// <param name="data">data to translate</param>
        /// <returns>translated string</returns>
        public static string TranslateWildcards(this string data) {
            if(!data.Contains("*") && !data.Contains("?")) {
                return $"%{data}%";
            }

            StringBuilder result = new StringBuilder();
            foreach(char character in data) {
                switch(character) {
                case '*':
                    result.Append('%');
                    break;
                case '?':
                    result.Append('_');
                    break;
                default:
                    result.Append(character);
                    break;
                }
            }

            return result.ToString();
        }
    }
}