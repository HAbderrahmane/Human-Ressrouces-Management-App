using MediatR;

namespace AccountService.Command.Application.Commands;

public record CreateAccountCommand(string Email, string Password) : IRequest<Guid>;
