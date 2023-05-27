using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.Models;

namespace BookingWebAPI.Services.Interfaces
{
    public interface ISettingService
    {
        Task<IEnumerable<BookingWebAPISetting>> GetSettingsForCategory(SettingCategory category);

        Task<T> GetValueForSetting<T>(string settingName);
    }
}
