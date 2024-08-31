namespace BookingWebAPI.Infrastructure.ViewModels
{
    public class ResourceCategoryViewModel
    {
        public Guid Id { get; set;  }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
