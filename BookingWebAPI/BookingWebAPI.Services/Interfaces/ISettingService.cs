using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.Models;

namespace BookingWebAPI.Services.Interfaces
{
    public interface ISettingService
    {
        Task<IEnumerable<BookingWebAPISetting>> GetSettingsForCategoryAsync(SettingCategory category);

        Task<T> GetValueBySettingNameAsync<T>(string settingName);

        T ExtractValueFromSetting<T>(BookingWebAPISetting setting);
    }
}
