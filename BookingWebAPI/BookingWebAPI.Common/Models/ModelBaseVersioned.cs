using System.ComponentModel.DataAnnotations;

namespace BookingWebAPI.Common.Models
{
    public class ModelBaseVersioned : ModelBase
    {
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}
