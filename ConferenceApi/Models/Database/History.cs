namespace ConferenceApi.Models.Database
{
    public class History
    {
        public Guid Id { get; set; }

        public double Amount { get; set; }
        
        public string Info { get; set; } = null!;

        public long Date { get; set; }
        
        public Operation Operation { get; set; }

        public Guid InitiatorId { get; set; }
        
        public Account Initiator { get; set; } = null!;

        public string Receiver { get; set; } = null!;
    }
}
