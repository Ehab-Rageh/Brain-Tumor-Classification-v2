namespace Brain_Tumor_Classification.Repositories.Interfaces;

public interface IUserService
{
    Task<IEnumerable<ApplicationUser>> GetAllAsync();
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<ApplicationUser> UpdateAsync(ApplicationUser user);
    Task<ApplicationUser?> Delete(ApplicationUser user);
    Task<IEnumerable <ApplicationUser?>> DeleteAll();

}
