using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NightlyCode.AspNetCore.Services.Convert;
using NightlyCode.Database.Entities.Operations;
using NightlyCode.Database.Errors;
using NightlyCode.Database.Fields;
using ScriptService.Dto.Patches;

namespace ScriptService.Dto {

    /// <summary>
    /// extensions for patch operations in a database context
    /// </summary>
    public static class DatabasePatchExtensions {

        /// <summary>
        /// applies a set of patch operations to an update operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateoperation">update operation to be updated</param>
        /// <param name="operations">operations to apply</param>
        /// <returns>the update operation for fluent behavior</returns>
        public static UpdateValuesOperation<T> Patch<T>(this UpdateValuesOperation<T> updateoperation, params PatchOperation[] operations) {
            return Patch(updateoperation, (IEnumerable<PatchOperation>) operations);
        }
            /// <summary>
        /// applies a set of patch operations to an update operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateoperation">update operation to be updated</param>
        /// <param name="operations">operations to apply</param>
        /// <returns>the update operation for fluent behavior</returns>
        public static UpdateValuesOperation<T> Patch<T>(this UpdateValuesOperation<T> updateoperation, IEnumerable<PatchOperation> operations) {
            Type entitytype = typeof(T);
            if(!Attribute.IsDefined(entitytype, typeof(AllowPatchAttribute)))
                throw new NotSupportedException($"Patching of '{entitytype.Name}' is not supported");

            List<Expression<Func<T, bool>>> setters = new List<Expression<Func<T, bool>>>();
            foreach(PatchOperation patch in operations) {
                if(patch.Op != "replace")
                    throw new NotSupportedException("Only 'replace' operations are supported when updating entities");

                string propertyname = patch.Path.Substring(1).ToLower();
                PropertyInfo property = entitytype.GetProperties().FirstOrDefault(p => p.Name.ToLower() == propertyname);
                if(property == null)
                    throw new PropertyNotFoundException(propertyname);

                if(!Attribute.IsDefined(property, typeof(AllowPatchAttribute)))
                    throw new NotSupportedException($"Patching of '{entitytype.Name}::{property.Name}' is not supported");

                object targetvalue = Converter.Convert(patch.Value, property.PropertyType, true);
                setters.Add(e => Field.Property<T>(property.Name, true) == targetvalue);
            }

            if (setters.Count > 0)
                updateoperation.Set(setters.ToArray());
            return updateoperation;
        }
    }
}