using System;
using System.ComponentModel.DataAnnotations;
using ConferenceApi.Models.Database;

namespace ConferenceApi.Models.RequestModels
{
    public class TransferRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, MinimumLength = 1)]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Operation is required")]
        public Operation Operation { get; set; }

        [Required(ErrorMessage = "Account ID is required")]
        public Guid Account { get; set; }

        [Required(ErrorMessage = "Bank is required")]
        [RegularExpression(@"^(M-BANK|Z_BANK)$", ErrorMessage = "Bank must be either M-BANK or Z_BANK")]
        public string Bank { get; set; } = null!;

        [Range(10, double.MaxValue, ErrorMessage = "Amount must be at least 10")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be 3 uppercase letters")]
        public string Currency { get; set; } = null!;

        public string? Info { get; set; }

        public DateTime Date { get; set; } 
    }
}