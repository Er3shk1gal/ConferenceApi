namespace ConferenceApi.Models.Database
{
    public class Account
    {

        public Guid Id { get; set; }
        
        public required string Name { get; set; }
        
        public double Balance { get; set; }
        
        public required string Currency { get; set; }
        
        public required User User { get; set; }
    }
}