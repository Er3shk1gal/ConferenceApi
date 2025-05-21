using ConferenceApi.Models.Database;

namespace ConferenceApi.Models.DTOs;

public class HistoryDto
{
    public double Amount { get; set; }
    public required string Info { get; set; }
    public long Date { get; set; }
    public Operation Operation { get; set; }
    public required string InitiatorName { get; set; }
    public required string Reciever { get; set; }
}