using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.Models;

namespace BookingWebAPI.DAL.Interfaces
{
    public interface ISettingRepository
    {
        Task<BookingWebAPISetting?> GetSettingByNameAsync(string settingName);

        Task<IEnumerable<BookingWebAPISetting>> GetSettingsForCategoryAsync(SettingCategory category);
    }
}
