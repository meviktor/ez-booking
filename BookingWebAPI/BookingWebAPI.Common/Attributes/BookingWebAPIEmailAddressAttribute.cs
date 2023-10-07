using BookingWebAPI.Common.Utils;
using System.ComponentModel.DataAnnotations;

namespace BookingWebAPI.Common.Attributes
{
    /// <summary>
    /// Validates if the value is a valid e-mail address. This property can be used with string properties only. Not doing so will cause an <see cref="ArgumentException"/> at aruntime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BookingWebAPIEmailAddressAttribute : ValidationAttribute
    {
        public BookingWebAPIEmailAddressAttribute()
        {
        }

        /// <summary>
        /// Validates if the value can be interpreted as a valid e-mail address. 
        /// </summary>
        /// <param name="value">The string to be validated.</param>
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
