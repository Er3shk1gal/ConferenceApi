using ConferenceApi.Models.Database;
using ConferenceApi.Models.RequestModels;
using ConferenceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceApi.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class BankController : ControllerBase
{
    private readonly IBankService _bankService; 
    private readonly ILogger<BankController> _logger;
    private readonly UserManager<User> _userManager;
    
    public BankController(IBankService bankService, UserManager<User> userManager, ILogger<BankController> logger)
    {
        _bankService = bankService;
        _userManager = userManager;
        _logger = logger;
    }
    
    [HttpGet("balance")]
    [Authorize(Policy = "ReadPolicy")]
    public async Task<IActionResult> GetBalanceAsync([FromQuery] Guid accountId)
    {
        var user = await _userManager.GetUserAsync(User);
        return Ok(user.Accounts.FirstOrDefault(x=> x.Id == accountId).Balance);
    }
    
    [HttpPost("transfer")]
    [Authorize(Policy = "ModifyPolicy")]
    public async Task<IActionResult> TransferAsync([FromBody] TransferRequest request)
    {
        return Ok(await _bankService.TransferAsync(await _userManager.GetUserAsync(User), request));
    }
    
    [HttpGet("history")]
    [Authorize(Policy = "HistoryPolicy")]
    public async Task<IActionResult> GetHistoryAsync([FromQuery]int page, [FromQuery] int size,[FromQuery] Guid accountId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var user = await _userManager.GetUserAsync(User);
        return Ok(_bankService.GetHistoryAsync(page, size, accountId, from, to));
    }
}