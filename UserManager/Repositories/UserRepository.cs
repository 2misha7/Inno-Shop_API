using Microsoft.EntityFrameworkCore;
using UserManager.Context;
using UserManager.DTO.Requests;
using UserManager.DTO.Responses;
using UserManager.Entities;
using UserManager.Helpers;


namespace UserManager.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManagerContext _dbContext;

    public UserRepository(UserManagerContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> UserExistsAsync(RegisterRequest model, CancellationToken cancellationToken)
    {
        if (_dbContext.Users.Any(u => u.Login.Equals(model.Login)))
        {
            return true;
        }

        return false;
    }

    public async Task AddNewUser(RegisterRequest model, CancellationToken cancellationToken)
    {
        var hashedPasswordAndSalt = SecurityHelpers.GetHashedPasswordAndSalt(model.Password);
        var user = new AppUser()
        {
            Email = model.Email,
            Login = model.Login,
            Role = model.Role,
            Password = hashedPasswordAndSalt.Item1,
            Salt = hashedPasswordAndSalt.Item2,
            RefreshToken = SecurityHelpers.GenerateRefreshToken(),
            RefreshTokenExp = DateTime.Now.AddDays(1),
            IsActive = true
        };
        await _dbContext.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public UserDTO GetUser(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var user =  _dbContext.Users.FirstOrDefault(u => u.Login == loginRequest.Login)!;
        if (!user.IsActive)
        {
            throw new Exception("User is not active");
        }
        return new UserDTO
        {
            Email = user.Email,
            IdUser = user.IdUser,
            Login = user.Login,
            Password = user.Password,
            RefreshToken = user.RefreshToken,
            RefreshTokenExp = user.RefreshTokenExp,
            Role = user.Role,
            Salt = user.Salt
        };
    }

    public UserDTO GetUserByRefreshToken(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
    {
        var user =  _dbContext.Users.FirstOrDefault(u => u.RefreshToken == refreshTokenRequest.RefreshToken)!;
        if (!user.IsActive)
        {
            throw new Exception("User is not active");
        }
        if (user == null)
        {
            throw new Exception("No user found with the provided refresh token.");
        }

        return new UserDTO
        {
            Email = user.Email,
            IdUser = user.IdUser,
            Login = user.Login,
            Password = user.Password,
            RefreshToken = user.RefreshToken,
            RefreshTokenExp = user.RefreshTokenExp,
            Role = user.Role,
            Salt = user.Salt
        };
    }

    public async Task<string> UpdateTokens(UserDTO userDto, CancellationToken cancellationToken)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Login == userDto.Login)!;
        if (!user.IsActive)
        {
            throw new Exception("User is not active");
        }
        user.RefreshToken = SecurityHelpers.GenerateRefreshToken();
        user.RefreshTokenExp = DateTime.Now.AddDays(1);
        await _dbContext.AddAsync(user, cancellationToken);
        return user.RefreshToken;
    }
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    

    public async Task DeleteUser(int idUser, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdUser == idUser, cancellationToken);
        if (user == null)
        {
            throw new Exception("This user does not exist");
        }
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<GetUserDTO> GetUserAsync(int idUser, CancellationToken cancellationToken)
    {
        var userDTO =  await _dbContext.Users.FirstOrDefaultAsync(u => u.IdUser == idUser);
        if (!userDTO.IsActive)
        {
            throw new Exception("User is not active");
        }
        var getUserDTO = new GetUserDTO
        {
            Login = userDTO.Login,
            Email = userDTO.Email,
            IdUser = userDTO.IdUser,
            Role = userDTO.Role
        };
        return getUserDTO;
    }

    public async Task UpdateUserAsync(int idUser,UpdateUserDTO updateUserDto, CancellationToken cancellationToken)
    {
        var user =  await _dbContext.Users.FirstOrDefaultAsync(u => u.IdUser == idUser);
        if (!user.IsActive)
        {
            throw new Exception("User is not active");
        }
        var hashedPasswordAndSalt = SecurityHelpers.GetHashedPasswordAndSalt(updateUserDto.Password);
        user.Email = updateUserDto.Email;
        user.Login = updateUserDto.Login;
        user.Password = hashedPasswordAndSalt.Item1;
        user.Salt = hashedPasswordAndSalt.Item2;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateUser(int idUser, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.IdUser == idUser);
        user.IsActive = true;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateUser(int idUser, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.IdUser == idUser);
        user.IsActive = false;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}