namespace Brain_Tumor_Classification.Repositories.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto);
    Task<AuthDto> LoginAsync(LoginDto dto);
    Task<string> ConfirmEmailAsync(ConfirmEmailRequestDto dto);
    Task<string> ForgetPasswordAsync(ForgetPasswordDto dto);
    Task<string> ResetPasswordAsync(ResetPasswordDto dto);
    Task<string> AddRoleAsync(AddRoleDto dto);
    Task<AuthDto> RefreshTokenAsync(string token);
    Task<bool> RevokeTokenAsync(string token);

}
