using Microsoft.AspNetCore.Authorization;

namespace Brain_Tumor_Classification.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TumorsController : ControllerBase
    {
    }
}
