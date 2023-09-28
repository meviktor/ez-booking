using BookingWebAPI.Common.Utils;
using System.ComponentModel.DataAnnotations;

namespace BookingWebAPI.Common.Attributes
{
    /// <summary>
    /// Required attribute for value types. Please, DO NOT use this attribute on reference or nullabe types (like strings, etc.) - use the <see cref="RequiredAttribute"/> for those properties, as before.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BookingWebAPIEmailAddressAttribute : ValidationAttribute
    {
        public BookingWebAPIEmailAddressAttribute()
        {
        }

        /// <summary>
        /// Validates if the value can be interpreted as a valid e-mail address. This property can be applied 
        /// </summary>
        /// <param name="value">The str.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If the attribute is applied on a property has a type different from <see cref="string"/>.</exception>
        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                throw new ArgumentException(nameof(value));
            }

            var type = value.GetType();
            if (type != typeof(string))
            {
                throw new ArgumentException(nameof(value), "This attribute can be applied only on properties whose type is string.");
            }
            return Utilities.IsValidEmail($"{value}");
        }
    }
}
