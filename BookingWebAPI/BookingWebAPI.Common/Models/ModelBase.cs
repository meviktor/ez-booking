﻿using BookingWebAPI.Common.Interfaces;

namespace BookingWebAPI.Common.Models
{
    public class ModelBase : IEntity
    {
        public Guid Id { get; set; }

        public virtual bool IsDeleted { get; set; }
    }
}
