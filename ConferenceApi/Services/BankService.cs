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
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

        public async Task<Account> CreateAccountAsync(User user)
        {
            try
            {
                Account account = new Account
                {
                    Balance = 1000,
                    User = user,
                    Currency = "USD",
                    Name = "Main",
                    
                };
                EntityEntry<Account> entry = await _unitOfWork.AccountRepository.InsertAsync(account);
                if (await _unitOfWork.SaveAsync())
                {
                    return entry.Entity;
                }
                throw new ArgumentException("Account not created");
                
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<bool> TransferAsync(User user, TransferRequest request)
        {
            _logger.LogInformation("Transfer from {FromUser} to {ToUser}", user.UserName, request.Username);

            await _unitOfWork.BeginTransactionAsync();
            
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(request.Account);
                if (account == null)
                {
                    throw new KeyNotFoundException("Account not found");
                }

               

                if ( !_userManager.GetRolesAsync(user).Result.Contains("Admin"))
                {
                    if (account.Balance < request.Amount)
                    {
                        throw new Exception("Insufficient funds");
                    }
                    account.Balance -= request.Amount;
                    _unitOfWork.AccountRepository.Update(account);
                }
        
                
                var recipientAccount = user.Accounts.First();
                recipientAccount.Balance += request.Amount;
                _unitOfWork.AccountRepository.Update(recipientAccount);

              
                await _unitOfWork.HistoryRepository.InsertAsync(new History
                {
                    Amount = request.Amount,
                    Date = request.Date.Ticks,
                    Info = request.Info,
                    Operation = request.Operation,
                    Initiator = account,
                    Receiver = request.Username
                });
                
                if (await _unitOfWork.SaveAsync())
                {
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                await _unitOfWork.RollbackTransactionAsync();
                return false;
            }
            catch(Exception e)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(e, e.Message);
                throw;
            }
        }


        public async Task<(List<HistoryDto> Histories, int TotalCount)> GetHistoryAsync(int page, int size, Guid? accountId, DateTime from, DateTime to)
        {
            try
            {
                var query = _unitOfWork.HistoryRepository.Get();
    
                var totalItems = await query.CountAsync();
    
                var pagedHistories = await query
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToListAsync();
       
                List<HistoryDto> result = pagedHistories.Select<History,HistoryDto>(history => new HistoryDto()
                {
                    Info = history.Info,
                    Amount = history.Amount,
                    Date = history.Date,
                    Operation = history.Operation,
                    InitiatorName = history.Initiator.Name,
                    Reciever = history.Receiver
                }).ToList();
        
                return (result, totalItems);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
