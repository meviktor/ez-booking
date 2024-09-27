using BookingWebAPI.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingWebAPI.Common.Models
{
    public class EmailConfirmationAttempt : ModelBase
    {
        public DateTimeOffset CreatedAt { get; set; }

        public EmailConfirmationStatus Status { get; set; }

        public EmailConfirmationFailReason? FailReason { get; set; }

        /// <summary>
        /// This entity is not intended to be deleted at any time, even logically! That is beacuse no database column is created for this, and the fixed return vaule.
        /// </summary>
        [NotMapped]
        public override bool IsDeleted => false;

        public Guid UserId { get; set; }

        public BookingWebAPIUser User { get; set; } = null!;
    }
}
