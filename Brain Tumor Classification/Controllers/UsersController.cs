using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Brain_Tumor_Classification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var users = await _userService.GetAllAsync();

        //    return Ok(users);
        //}

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirst("uid")?.Value;

            if (userId is null) return Unauthorized();

            var user = await _userService.GetByIdAsync(userId);

            return Ok(_mapper.Map<UserDetailsDto>(user));
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst("uid")?.Value;

            if(userId is null) return Unauthorized();

            var user = await _userService.GetByIdAsync(userId);
            if (user is null)
                return NotFound($"There is no user with Id: {userId}");

            user.Name = dto.Name;
            user.BirthDate = dto.BirthDate;
            user.Gender = dto.Gender;

            await _userService.UpdateAsync(user);

            return Ok(_mapper.Map<UpdateUserDto>(user));
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var userId = User.FindFirst("uid")?.Value;

            if (userId is null) return Unauthorized();

            var user = await _userService.GetByIdAsync(userId);

            if (user is null)
                return NotFound($"There is no user with Id: {userId}");

            await _userService.Delete(user);

            return Ok(_mapper.Map<UserDetailsDto>(user));
        }

        //[HttpDelete("delete-all")]
        //public async Task<IActionResult> DeleteAllUsers()
        //{
        //    await _userService.DeleteAll();
        //    return Ok(new { message = "All users deleted successfully" });
        //}
    }
}
