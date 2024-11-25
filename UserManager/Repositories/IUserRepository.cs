using UserManager.DTO.Requests;
using UserManager.DTO.Responses;

namespace UserManager.Repositories;

public interface IUserRepository
{
    Task<bool> UserExistsAsync(RegisterRequest model, CancellationToken cancellationToken);
    Task AddNewUser(RegisterRequest model, CancellationToken cancellationToken);
    UserDTO GetUser(LoginRequest loginRequest, CancellationToken cancellationToken);
    UserDTO GetUserByRefreshToken(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken);
    Task<string> UpdateTokens(UserDTO userDto, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task DeleteUser(int idUser, CancellationToken cancellationToken);
    Task<GetUserDTO> GetUserAsync(int idUser, CancellationToken cancellationToken);
    Task UpdateUserAsync(int idUser, UpdateUserDTO userDto, CancellationToken cancellationToken);
    Task ActivateUser(int idUser, CancellationToken cancellationToken);
    Task DeactivateUser(int idUser, CancellationToken cancellationToken);
}