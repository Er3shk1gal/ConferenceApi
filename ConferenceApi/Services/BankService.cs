using ConferenceApi.Database.Repositories;
using ConferenceApi.Models.Database;
using ConferenceApi.Models.DTOs;
using ConferenceApi.Models.RequestModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ConferenceApi.Services
{
    public class BankService : IBankService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<BankService> _logger;

        public BankService(UnitOfWork unitOfWork, ILogger<BankService> logger, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<bool> TransferAsync(ClaimsPrincipal principal, TransferRequest request)
        {
            _logger.LogInformation("Transfer from {FromUser} to {ToUser}", principal.Identity?.Name, request.Username);

            await _unitOfWork.BeginTransactionAsync();
            
            try
            {
                var account = await _unitOfWork.AccountRepository.FindByIdAsync(request.Account);
                if (account == null)
                    throw new Exception("Account not found");

                // Z-BANK transfer
                if (request.Bank.Equals("Z-BANK", StringComparison.OrdinalIgnoreCase))
                {
                    // Implement Z-BANK API call here
                    var result = true; // Placeholder for Z-BANK API call
                    
                    if (result)
                        await SaveHistoryAsync(account, request);
                    
                    await transaction.CommitAsync();
                    return result;
                }

                // Check if user is admin
                var isAdmin = (await _userService.GetUserByUsernameAsync(principal.Identity?.Name))
                    .Roles.Any(r => r.Name == "ROLE_ADMIN");

                if (!isAdmin)
                {
                    if (account.Balance < request.Amount)
                        throw new Exception("Insufficient funds");

                    account.Balance -= request.Amount;
                    await _unitOfWork.Accounts.UpdateAsync(account);
                }

                var recipient = await _userService.GetUserByUsernameAsync(request.Username);
                var recipientAccount = recipient.Accounts.First();
                recipientAccount.Balance += request.Amount;
                await _unitOfWork.Accounts.UpdateAsync(recipientAccount);

                await SaveHistoryAsync(account, request);
                await _unitOfWork.CommitAsync();
                
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task SaveHistoryAsync(Account initiator, TransferRequest request)
        {
            var history = new History
            {
                Amount = request.Amount,
                Date = request.Date.Ticks,
                Info = request.Info,
                Operation = request.Operation,
                Initiator = initiator,
                Receiver = request.Username
            };

            await _unitOfWork.Histories.AddAsync(history);
        }

        public async Task<PagedModel<HistoryDto>> GetHistoryAsync(int page, int size, Guid? accountId, DateFilter filter)
        {
            throw new NotImplementedException();
        }
    }
}
