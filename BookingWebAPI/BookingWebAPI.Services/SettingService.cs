using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.Services.Interfaces;

namespace BookingWebAPI.Services
{
    internal class SettingService : ISettingService
    {
        private ISettingRepository _settingRepository;

        public SettingService(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public async Task<IEnumerable<BookingWebAPISetting>> GetSettingsForCategoryAsync(SettingCategory category) => await _settingRepository.GetSettingsForCategoryAsync(category);

        public async Task<T> GetValueBySettingNameAsync<T>(string settingName)
        {
            var setting = await _settingRepository.GetSettingByNameAsync(settingName);
            if(setting == null)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.EntityNotFound);
            }

            return ExtractValueFromSetting<T>(setting);
        }

        public T ExtractValueFromSetting<T>(BookingWebAPISetting setting)
        {
            if(setting == null)
            {
                throw new ArgumentNullException(nameof(setting));
            }

            switch (setting.ValueType)
            {
                case SettingValueType.Boolean:
                    if (typeof(T).Equals(typeof(bool)) && bool.TryParse(setting.RawValue, out bool boolResult))
                    {
                        return (T)Convert.ChangeType(boolResult, typeof(T));
                    }
                    break;
                case SettingValueType.Integer:
                    if (typeof(T).Equals(typeof(int)) && int.TryParse(setting.RawValue, out int intResult))
                    {
                        return (T)Convert.ChangeType(intResult, typeof(T));
                    }
                    break;
                case SettingValueType.Float:
                    if (typeof(T).Equals(typeof(float)) && float.TryParse(setting.RawValue, out float floatResult))
                    {
                        return (T)Convert.ChangeType(floatResult, typeof(T));
                    }
                    break;
                case SettingValueType.String:
                    return (T)Convert.ChangeType(setting.RawValue, typeof(T));
                default:
                    throw new BookingWebAPIException(ApplicationErrorCodes.SettingNotKnownSettingType);
            }

            throw new BookingWebAPIException(ApplicationErrorCodes.SettingWrongSettingType);
        }
    }
}
