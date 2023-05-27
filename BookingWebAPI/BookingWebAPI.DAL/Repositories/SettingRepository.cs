using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories
{
    internal class SettingRepository : RepositoryBase<BookingWebAPISetting>, ISettingRepository
    {
        public SettingRepository(BookingWebAPIDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<BookingWebAPISetting?> GetSettingByName(string settingName) => await Set.SingleOrDefaultAsync(s => !s.IsDeleted && s.Name == settingName);

        public async Task<IEnumerable<BookingWebAPISetting>> GetSettingsForCategory(SettingCategory category) => await Set.Where(s => !s.IsDeleted && s.Category == category).ToListAsync();
    }
}
