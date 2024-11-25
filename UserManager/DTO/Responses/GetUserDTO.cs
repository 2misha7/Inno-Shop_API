namespace UserManager.DTO.Responses;

public class GetUserDTO
{
    public int IdUser { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}