﻿using System.Text.Json.Serialization;

namespace Brain_Tumor_Classification.Dtos;

public class AuthDto
{
    public string Message { get; set; }
    public bool IsAuthenticated { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
    public string Token { get; set; }
    public bool IsConfirmed { get; set; } = false;
    //public DateTime ExpiresOn { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
}
