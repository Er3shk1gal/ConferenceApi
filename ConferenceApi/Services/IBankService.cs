using ConferenceApi.Models.DTOs;
using ConferenceApi.Models.RequestModels;
using System.Security.Claims;
using System.Threading.Tasks;
using ConferenceApi.Models.Database;

namespace ConferenceApi.Services
{
    public interface IBankService
    {
        Task<Account> CreateAccountAsync(User user);
        Task<bool> TransferAsync(User user, TransferRequest request);
        Task<(List<HistoryDto> Histories, int TotalCount)>  GetHistoryAsync(int page, int size, Guid? accountId, DateTime from, DateTime to);
    }
}
