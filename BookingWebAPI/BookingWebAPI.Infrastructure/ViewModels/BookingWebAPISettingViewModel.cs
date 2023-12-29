using BookingWebAPI.Common.Enums;

namespace BookingWebAPI.Infrastructure.ViewModels
{
    /// <summary>
    /// View model for fetching application settings. Do not use it for updating application settings.
    /// </summary>
    public class BookingWebAPISettingViewModel
    {
        public SettingCategory Category { get; set; }

        public string Name { get; set; } = null!;

        public SettingValueType ValueType { get; set; }

        public string RawValue { get; set; } = null!;
    }
}
