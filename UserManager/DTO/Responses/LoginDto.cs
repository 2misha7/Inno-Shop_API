using System.IdentityModel.Tokens.Jwt;


namespace UserManager.DTO.Responses;

public class LoginDto
{
    public string accessToken { get; set; }
    public string refreshToken { get; set; } 
}