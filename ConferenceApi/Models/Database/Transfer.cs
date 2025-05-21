using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConferenceApi.Models.Database
{
    public class Transfer
    {
        public Guid Id { get; set; }

        public double Amount { get; set; }

        public string Info { get; set; } = null!;

        public long Date { get; set; }

        public Operation Operation { get; set; }

        public Guid AccountId { get; set; }
        
        public Account Account { get; set; } = null!;
    }
}
