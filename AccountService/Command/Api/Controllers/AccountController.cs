using Microsoft.AspNetCore.Mvc;
using AccountService.Command.Application.Commands;
using AccountService.Command.Application.DTOs;
using Mapster;
using Infrastructure.Api.Messaging;

namespace AccountService.Command.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public AccountController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateAccountCommand command, CancellationToken ct)
    {
        var id = await _dispatcher.SendAsync<CreateAccountCommand, Guid>(command, ct);
        var response = new { AccountId = id, command.Email }.Adapt<CreateAccountResponse>();
        return Ok(response);
    }
}
