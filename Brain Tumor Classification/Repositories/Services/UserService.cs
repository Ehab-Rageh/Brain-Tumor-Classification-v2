using System.Collections;

namespace Brain_Tumor_Classification.Repositories.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
    {
        var users = _context.Users.ToList();

        return users;
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _context.ApplicationUsers.FindAsync(id);
    }

    public async Task<ApplicationUser> UpdateAsync(ApplicationUser user)
    {
        _context.ApplicationUsers.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<ApplicationUser?> Delete(ApplicationUser user)
    {
        _context.ApplicationUsers.Remove(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<IEnumerable<ApplicationUser?>> DeleteAll()
    {
        var Users = _context.Users.ToList();
        _context.Users.RemoveRange(Users);

        await _context.SaveChangesAsync();

        return Users;
    }
}
