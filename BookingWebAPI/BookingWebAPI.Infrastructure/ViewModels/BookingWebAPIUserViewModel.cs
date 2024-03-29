﻿namespace BookingWebAPI.Common.ViewModels
{
    public class BookingWebAPIUserViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public bool EmailConfirmed { get; set; }
    }
}
