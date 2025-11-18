using MediatR;
using Microsoft.AspNetCore.Mvc;
using AccountService.Command.Application.Commands;

namespace AccountService.Command.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateAccountCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { AccountId = id, command.Email });
    }
}
