namespace Brain_Tumor_Classification.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> userManager;

    public AuthController
    (
        IAuthService authService,
        UserManager<ApplicationUser> userManager
    )
    {
        _authService = authService;
        this.userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(dto);

        if (!result.IsNullOrEmpty())
            return BadRequest(result);


        return Ok("User registered successfully!\n" +
                  "Check your Email to validation!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Loginasync([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(dto);

        if (!result.IsAuthenticated)
            return BadRequest(result.Message);

        if (!result.IsConfirmed)
            return BadRequest("Please confirm your email first");

        if (!string.IsNullOrEmpty(result.RefreshToken))
            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

        return Ok(new
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken
        });
    }

    [HttpGet("Me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst("uid")?.Value;
        
        if (userId == null)
            return Unauthorized();

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found");

        var roles = await userManager.GetRolesAsync(user);

        var result = new GetCurrentUserResponseDto
        {
            Name = user.Name,
            Email = user.Email,
            Gender = user.Gender,
            BirthDate = user.BirthDate,
        };

        return Ok(result);
    }

    [HttpPost("confirmEmail")]
    public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailRequestDto confirmEmailRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var res = await _authService.ConfirmEmailAsync(confirmEmailRequestDto);

        if (res == "T")
            return Ok("Email confirmed successfully.");

        return BadRequest(await _authService.ConfirmEmailAsync(confirmEmailRequestDto));
    }

    [Route("forgetPassword")]
    [HttpPost]
    public async Task<IActionResult> ForgetPasswordAsync(ForgetPasswordDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _authService.ForgetPasswordAsync(request) is not null)
            return BadRequest(await _authService.ForgetPasswordAsync(request));

        return Ok(new { message = "Reset password code has been sent to your email" });
    }

    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _authService.ResetPasswordAsync(model) is not null)
            return BadRequest(await _authService.ResetPasswordAsync(model));

        return Ok("Password has been reset successfully.");
    }

    [HttpPost("addRole")]
    public async Task<IActionResult> AddRoleasync([FromBody] AddRoleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.AddRoleAsync(dto);

        if (!string.IsNullOrEmpty(result))
            return BadRequest(result);

        return Ok(dto);
    }

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        var result = await _authService.RefreshTokenAsync(refreshToken);

        if (!result.IsAuthenticated)
            return BadRequest(result.Message);

        SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

        return Ok(result);
    }

    [HttpPost("revokeToken")]
    public async Task<IActionResult> RevokeTokenAsync([FromBody] RevokeTokenDto dto)
    {
        var token = dto.Token ?? Request.Cookies["refreshToken"];

        if (token is null)
            return BadRequest("Token is required!");

        var result = await _authService.RevokeTokenAsync(token);

        if (!result)
            return BadRequest("Token is invalid!");

        return Ok("Token has been revoked successfully!");
    }

    private void SetRefreshTokenInCookie(string refreshToken, DateTime Expires)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = Expires.ToLocalTime(),
            Secure = true,
            IsEssential = true,
            SameSite = SameSiteMode.None,
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
