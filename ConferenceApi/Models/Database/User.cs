using Microsoft.AspNetCore.Identity;

namespace ConferenceApi.Models.Database;

public class User : IdentityUser
{
    public string Fullname { get; set; } = string.Empty;
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}