using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.Models;

namespace BookingWebAPI.DAL.Interfaces
{
    public interface ISettingRepository
    {
        Task<BookingWebAPISetting?> GetSettingByName(string settingName);

        Task<IEnumerable<BookingWebAPISetting>> GetSettingsForCategory(SettingCategory category);
    }
}
