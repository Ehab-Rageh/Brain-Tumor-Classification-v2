
using System.Security.Cryptography;

namespace Brain_Tumor_Classification.Repositories.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailSender _emailSender;
    private readonly JWT _jwt;
     
    public AuthService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _roleManager = roleManager;
        _jwt = jwt.Value;
    }
    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
            return "Email is already exists!";

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            Name = dto.Name,
            Gender = dto.Gender,
            BirthDate = dto.BirthDate,
         
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            foreach (var role in dto.Roles)
            {
                result = await _userManager.AddToRoleAsync(user, role);
            }
        }

        if (!result.Succeeded)
        {
            var errors = string.Empty;
            foreach (var error in result.Errors)
            {
                errors += $"{error.Description}\n";
            }

            return errors;
        }

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        await _emailSender.SendEmailAsync(user.Email, "Email Confirmation", $"Your confirmation code: {code}");

        await _userManager.AddToRoleAsync(user, "User");

        return string.Empty;
    }

    public async Task<AuthDto> LoginAsync(LoginDto dto)
    {
        var authDto = new AuthDto();

        var user = await _userManager.FindByEmailAsync(dto.Email);

        //Todo
        //if (user.EmailConfirmed)
        //{
        //    authDto.IsConfirmed = true;
        //}

        authDto.IsConfirmed = true;

        if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return new AuthDto { Message = "Email or password is incorrect!" };

        var jwtSecurityToken = await CreateJwtTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        authDto.Email = user.Email;
        authDto.IsAuthenticated = true;
        authDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        authDto.Roles = roles.ToList();
        //authDto.ExpiresOn = jwtSecurityToken.ValidTo;

        if (user.RefreshTokens.Any(t => t.IsActive))
        {
            var activeRefreshToken = user.RefreshTokens.SingleOrDefault(t => t.IsActive);
            authDto.RefreshToken = activeRefreshToken.Token;
            authDto.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
        }
        else
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
            authDto.RefreshToken = refreshToken.Token;
            authDto.RefreshTokenExpiration = refreshToken.ExpiresOn;
        }

        return authDto;
    }

    public async Task<string> ConfirmEmailAsync(ConfirmEmailRequestDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);

        if (user == null)
            return "User not found.";

        var result = await _userManager.ConfirmEmailAsync(user, dto.Code);

        if (!result.Succeeded)
            return "T";

        return "Invalid or expired confirmation code.";
    }

    public async Task<string> ForgetPasswordAsync(ForgetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            return "User not found";

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // Store the code temporarily
        await _userManager.SetAuthenticationTokenAsync(user, "PasswordReset", "Code", token);

        await _emailSender.SendEmailAsync(user.Email, "Password Reset Code", $"Your password reset code: {token}");

        return string.Empty;
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            return "Invalid email.";

        // Retrieve stored reset code
        var storedCode = await _userManager.GetAuthenticationTokenAsync(user, "PasswordReset", "Code");
        if (storedCode != dto.Code)
            return "Invalid or expired reset code.";

        // Reset password
        var result = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), dto.NewPassword);
        
        if (!result.Succeeded)
        {
            var errors = string.Empty;
            foreach (var error in result.Errors)
            {
                errors += $"{error.Description}\n";
            }
            return errors;
        }

        // Remove the stored reset code
        await _userManager.RemoveAuthenticationTokenAsync(user, "PasswordReset", "Code");

        return string.Empty;
    }

    public async Task<JwtSecurityToken> CreateJwtTokenAsync(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();

        foreach (var role in roles)
            roleClaims.Add(new Claim(ClaimTypes.Role, role));

        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials
        );

        return jwtSecurityToken;
    }

    public async Task<string> AddRoleAsync(AddRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);

        if(user is null || !await _roleManager.RoleExistsAsync(dto.Role))
            return "Invaild User ID or Role!";

        if (await _userManager.IsInRoleAsync(user, dto.Role))
            return "User already has this role!";

        var result = await _userManager.AddToRoleAsync(user, dto.Role);

        return result.Succeeded ? string.Empty : "Something went wrong!";
    }

    public async Task<AuthDto> RefreshTokenAsync(string token)
    {
        var authDto = new AuthDto();

        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        if(user is null)
        {
            authDto.Message = "Invalid token!";
            return authDto;
        }

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
        {
            authDto.Message = "Invalid token!";
            return authDto;
        }

        refreshToken.RevokedOn = DateTime.UtcNow;

        var newRefreshToken = GenerateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);

        var jwtSecurityToken = await CreateJwtTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        authDto.IsAuthenticated = true;
        authDto.Email = user.Email;
        authDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        authDto.Roles = roles.ToList();
        authDto.RefreshToken = newRefreshToken.Token;
        authDto.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

        return authDto;
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user is null)
            return false;

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
            return false;

        refreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return true;
    }

    private RefreshToken GenerateRefreshToken()
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            ExpiresOn = DateTime.UtcNow.AddDays(3),
            CreatedOn = DateTime.UtcNow
        };
    }

}
