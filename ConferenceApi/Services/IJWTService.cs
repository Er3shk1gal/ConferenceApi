using ConferenceApi.Models;
using ConferenceApi.Models.Database;

namespace ConferenceApi.Services;

public interface IJWTService
{
    public Task<string> GenerateToken(User user);
}