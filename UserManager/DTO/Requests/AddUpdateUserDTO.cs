namespace UserManager.DTO.Requests;

public class AddUpdateUserDTO
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public string RefreshToken { get; set; }
    public string Role { get; set; }
    public DateTime? RefreshTokenExp { get; set; }
    public bool IsDeleted { get; set; }
}