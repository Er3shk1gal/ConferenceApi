using ConferenceApi.Models.DTOs;
using ConferenceApi.Models.RequestModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConferenceApi.Services
{
    public interface IBankService
    {
        Task<bool> TransferAsync(ClaimsPrincipal principal, TransferRequest request);
        Task<PagedModel<HistoryDto>> GetHistoryAsync(int page, int size, Guid? accountId, DateFilter filter);
    }
}
