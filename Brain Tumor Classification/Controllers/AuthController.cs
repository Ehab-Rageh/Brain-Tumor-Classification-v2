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

        return Ok(new
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken
        });
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

    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmationEmailAsync([FromBody] ResendConfirmationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.ResendConfirmationEmailAsync(dto);

        if (!result.IsNullOrEmpty())
            return BadRequest(result);

        return Ok("a new confirmation email has been sent.");
    }

    [Route("forgetPassword")]
    [HttpPost]
    public async Task<IActionResult> ForgetPasswordAsync(ForgetPasswordDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var  res = await _authService.ForgetPasswordAsync(request);

        if (res == "User not found") 
            return BadRequest("Something Went Wrong. Please Try again later");

        return Ok(new { message = "Reset password code has been sent to your email" });
    }

    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var res = await _authService.ResetPasswordAsync(model);

        if (res.IsNullOrEmpty())
            return Ok("password changed successfully");

        return BadRequest(await _authService.ResetPasswordAsync(model));
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
    public async Task<IActionResult> RefreshTokenAsync([FromBody] TokenDto dto)
    {
        var token = dto.Token;

        if (token == null)
            return BadRequest("refreshToken is null");

        var result = await _authService.RefreshTokenAsync(token);

        if (!result.IsAuthenticated)
            return BadRequest(result.Message);

        return Ok(new
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken
        });
    }

    [HttpPost("revokeToken")]
    public async Task<IActionResult> RevokeTokenAsync([FromBody] TokenDto dto)
    {
        var token = dto.Token;

        if (token is null)
            return BadRequest("Token is required!");
            
        var result = await _authService.RevokeTokenAsync(token);

        if (!result)
            return BadRequest("Token is invalid!");

        return Ok("Token has been revoked successfully!");
    }

    //private void SetRefreshTokenInCookie(string refreshToken, DateTime Expires)
    //{
    //    var cookieOptions = new CookieOptions
    //    {
    //        HttpOnly = true,
    //        Expires = Expires.ToLocalTime(),
    //        Secure = true,
    //        IsEssential = true,
    //        SameSite = SameSiteMode.None,
    //        Domain = "brain-tumor-classification.runasp.net",
            
    //    };
    //    Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    //}
}
