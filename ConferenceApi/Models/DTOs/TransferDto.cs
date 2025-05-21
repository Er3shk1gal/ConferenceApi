using System.ComponentModel.DataAnnotations;

namespace ConferenceApi.Models.DTOs
{
    public class TransferDto
    {
        public string Initiator { get; set; } = null!;
        
        public string Receiver { get; set; } = null!;
        
        [Range(10, double.MaxValue, ErrorMessage = "Amount must be at least 10")]
        public double Amount { get; set; }
        
        public string Currency { get; set; } = null!;
    }
}