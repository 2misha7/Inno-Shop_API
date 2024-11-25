﻿using System.ComponentModel.DataAnnotations;

namespace UserManager.Entities;

public class AppUser
{
    [Key]
    public int IdUser { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public string RefreshToken { get; set; }
    public string Role { get; set; }
    public DateTime? RefreshTokenExp { get; set; }
    public bool IsActive { get; set; }
}