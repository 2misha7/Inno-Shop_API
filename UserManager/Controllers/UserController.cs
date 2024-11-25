using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserManager.Context;
using UserManager.DTO.Requests;
using UserManager.DTO.Responses;
using UserManager.Helpers;
using UserManager.Services;
using LoginRequest = UserManager.DTO.Requests.LoginRequest;
using RegisterRequest = UserManager.DTO.Requests.RegisterRequest;

namespace UserManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    
    private int GetAuthorizedUserId()
    {
        var userIdClaim = User.FindFirst("IdUser");
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User ID not found in the token.");

        return int.Parse(userIdClaim.Value);
    }
    
    private readonly UserManagerContext _context;
    private readonly IUserService _userService;

    public UserController(UserManagerContext context, IUserService userService)
    {
        _userService = userService;
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterStudent(RegisterRequest model, CancellationToken cancellationToken)
    {
        try
        {
            if (model.Role != "admin" && model.Role != "user")
            {
                return BadRequest("Invalid Role");
            }
            await _userService.RegisterStudentAsync(model, cancellationToken);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async  Task<IActionResult> Login(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        try
        {
           var toReturn = await _userService.Login(loginRequest, cancellationToken);
           return Ok(toReturn);
        }
        catch (Exception e)
        {
            if (e.Message == "Unauthorized")
            {
                return Unauthorized();
            }
            return BadRequest(e.Message);
        }
        
    }
    
    [Authorize]
    [HttpDelete("/{idUser}")]
    public async Task<IActionResult> DeleteUser(int idUser, CancellationToken cancellationToken)
    {
        try
        {
            var id = GetAuthorizedUserId();
            if (idUser != id)
            {
                return Unauthorized("Users are able to delete only their accounts");
            }
            await _userService.DeleteUserAsync(idUser, cancellationToken);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("/activate/{idUser}")]
    public async Task<IActionResult> ActivateUser(int idUser, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.ActivateUser(idUser, cancellationToken);
            return Ok("User is active now");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpPut("/deactivate/{idUser}")]
    public async Task<IActionResult> DeactivateUser(int idUser, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.DeactivateUser(idUser, cancellationToken);
            return Ok("User is not active now");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
    [Authorize]
    [HttpPut("/{idUser}")]
    public async Task<IActionResult> UpdateUser(int idUser, [FromBody] UpdateUserDTO updateDto, CancellationToken cancellationToken)
    {
        try
        {
            var id = GetAuthorizedUserId();
            if (idUser != id)
            {
                return Unauthorized("Users are able to update only their own info");
            }
            await _userService.UpdateUserAsync(idUser, updateDto, cancellationToken);
            return Ok();
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("/{idUser}")]
    public async Task<IActionResult> GetUser(int idUser, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.GetUserAsync(idUser, cancellationToken);
            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    
     [Authorize(AuthenticationSchemes = "IgnoreTokenExpirationScheme")]
     [HttpPost("refresh")]
     public async Task<IActionResult> Refresh(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
     {
         try
         {
             var result = await _userService.RefreshToken(refreshTokenRequest, cancellationToken);
             return Ok(result);
         }
         catch (Exception e)
         {
             if (e.Message is "No user found with the provided refresh token." or "Refresh token expired")
             {
                 return Unauthorized();
             }
             return BadRequest(e.Message);
         }
     }
}