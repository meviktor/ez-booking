using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace BookingWebAPI.Common.Attributes
{
    /// <summary>
    /// Required attribute for value types. Please, DO NOT use this attribute on reference or nullabe types (like strings, etc.) - use the <see cref="RequiredAttribute"/> for those properties, as before.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredNotDefaultAttribute : ValidationAttribute
    {
        private static readonly ConcurrentDictionary<string, object> defaultInstancesCache = new ConcurrentDictionary<string, object>();

        public RequiredNotDefaultAttribute()
        {
        }

        /// <summary>
        /// Gets the value indicating whether or not the specified <paramref name="value" /> is valid with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value which is under investigation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If the attribute is applied on a property is null, having reference or nullable type.</exception>
        public override bool IsValid(object? value)
        {
            if(value == null)
            {
                throw new ArgumentException(nameof(value));
            }

            var type = value.GetType();
            if(!type.IsValueType || type.IsGenericType)
            {
                throw new ArgumentException(nameof(value), "This attribute cannot be placed on reference, or nullable types.");
            }
            if (!defaultInstancesCache.TryGetValue(type.FullName!, out var defaultInstance))
            {
                defaultInstance = Activator.CreateInstance(Nullable.GetUnderlyingType(type) ?? type);
                // type has to be some value type, so FullName should not return null
                // as we handle only value types via this attribute, the actual default value cannot be null either 
                defaultInstancesCache[type.FullName!] = defaultInstance!;
            }
            return !Equals(value, defaultInstance);
        }
    }
}
