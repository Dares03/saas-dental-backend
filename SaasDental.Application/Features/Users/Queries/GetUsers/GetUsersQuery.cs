using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Users.Queries.GetUsers;

public record UserDto(Guid Id, string FirstName, string LastName, string Email, string Role, bool IsActive, Guid? DefaultBranchId);

public record GetUsersQuery : IRequest<List<UserDto>>;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        return users.Select(u => new UserDto(
            u.Id,
            u.FirstName,
            u.LastName,
            u.Email,
            u.Role,
            u.IsActive,
            u.DefaultBranchId
        )).ToList();
    }
}
