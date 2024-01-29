using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.Interfaces;

namespace BookingWebAPI.Common.Models
{
    // This class is not inherited from ModelBase for a reason - this entity is not intended to be deleted at any time, even logically!
    // So, the IsDeleted property/column does not make any sense in case of this entity, it would just take up unnecessary sapce in the databae.
    public class EmailConfirmationAttempt : IEntity
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public EmailConfirmationStatus Status { get; set; }

        public EmailConfirmationFailReason? FailReason { get; set; }

        public Guid UserId { get; set; }

        public virtual BookingWebAPIUser User { get; set; } = null!;
    }
}
