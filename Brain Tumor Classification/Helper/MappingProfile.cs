namespace Brain_Tumor_Classification.Helper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, RegisterDto>().ReverseMap();
        CreateMap<ApplicationUser, UserDetailsDto>().ReverseMap();
        CreateMap<ApplicationUser, UpdateUserDto>().ReverseMap();
    }
}
