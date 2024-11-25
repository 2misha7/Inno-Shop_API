using UserManager.DTO.Requests;
using UserManager.DTO.Responses;

namespace UserManager.Services;

public interface IUserService
{
    Task<UserDTO> CreateUserAsync(AddUpdateUserDTO userDto, CancellationToken cancellationToken);
    Task<GetUserDTO> GetUserAsync(int idUser, CancellationToken cancellationToken);
    Task UpdateUserAsync(int idUser, UpdateUserDTO userDto, CancellationToken cancellationToken);
    Task DeleteUserAsync(int idUser, CancellationToken cancellationToken);
    Task RegisterStudentAsync(RegisterRequest model, CancellationToken cancellationToken);
    Task<LoginDto> Login(LoginRequest loginRequest, CancellationToken cancellationToken);
    Task<LoginDto> RefreshToken(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken);
    Task ActivateUser(int idUser, CancellationToken cancellationToken);
    Task DeactivateUser(int idUser, CancellationToken cancellationToken);
}