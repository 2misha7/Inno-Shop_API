using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProductManager.Repositories;
using UserManager.DTO.Requests;
using UserManager.DTO.Responses;
using UserManager.Helpers;

using UserManager.Repositories;

namespace UserManager.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
   // private readonly IProductRepository _productRepository;

    public UserService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        //_productRepository = productRepository;
    }

    public Task<UserDTO> CreateUserAsync(AddUpdateUserDTO userDto, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<GetUserDTO> GetUserAsync(int idUser, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _userRepository.GetUserAsync(idUser, cancellationToken);
            return result;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        } 
    }

    public async Task UpdateUserAsync(int idUser, UpdateUserDTO userDto, CancellationToken cancellationToken)
    {
        try
        {
            await _userRepository.UpdateUserAsync(idUser, userDto, cancellationToken);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task DeleteUserAsync(int idUser, CancellationToken cancellationToken)
    {
        try
        {
            await _userRepository.DeleteUser(idUser, cancellationToken);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task RegisterStudentAsync(RegisterRequest model, CancellationToken cancellationToken)
    {
        var exists = await _userRepository.UserExistsAsync(model, cancellationToken);
        if (exists)
        {
            throw new Exception("User with this login already exists");
        }
        
        await _userRepository.AddNewUser(model, cancellationToken);
        
    }

    public async Task<LoginDto> Login(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var userDto = _userRepository.GetUser(loginRequest, cancellationToken);

        string passwordHashFromDb = userDto.Password;
        string curHashedPassword = SecurityHelpers.GetHashedPasswordWithSalt(loginRequest.Password, userDto.Salt);

        if (passwordHashFromDb != curHashedPassword && userDto.Role == loginRequest.Role)
        {
            throw new Exception("Unauthorized");
        }
        Claim[] userclaim = new[]
        {
            new Claim(ClaimTypes.Role, userDto.Role),
            new Claim("IdUser", userDto.IdUser.ToString()),
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));

        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: userclaim,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: creds
        );

        var rToken = await _userRepository.UpdateTokens(userDto, cancellationToken);

        return new LoginDto
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken = userDto.RefreshToken
        };
    }

    public async Task<LoginDto> RefreshToken(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
    {
        UserDTO userDTO;
        try
        {
            userDTO = _userRepository.GetUserByRefreshToken(refreshTokenRequest, cancellationToken);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        if (userDTO.RefreshTokenExp < DateTime.Now)
        {
            throw new Exception("Refresh token expired");
        }
        var userClaims = new[]
        {
            new Claim(ClaimTypes.Role, userDTO.Role),
            new Claim("IdUser", userDTO.IdUser.ToString()),
        };
        
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));

        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var jwtToken = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: userClaims,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: creds
        );
        
        var refreshToken   = await _userRepository.UpdateTokens(userDTO, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
        
        
        return new LoginDto
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            refreshToken = refreshToken
        };
    }

    public async Task ActivateUser(int idUser, CancellationToken cancellationToken)
    {
        try
        {
            await _userRepository.ActivateUser(idUser, cancellationToken);
            //await _productRepository.RestoreProductsAsync(idUser, cancellationToken);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task DeactivateUser(int idUser, CancellationToken cancellationToken)
    {
        try
        {
            await _userRepository.DeactivateUser(idUser, cancellationToken);
            //await _productRepository.SoftDeleteProductsAsync(idUser, cancellationToken);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}