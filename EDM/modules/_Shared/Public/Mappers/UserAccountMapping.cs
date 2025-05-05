using System.Security.Cryptography;

namespace Public.Mappers
{
    public class UserAccountMapping
    {
        // Map từ UserDto sang UserEntity
        //CreateMap<UserDto, UserEntity>()
        //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
        //    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
        //    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == "Active"));

        //// Map từ UserEntity sang UserDto (nếu cần)
        //CreateMap<UserEntity, UserDto>()
        //    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
        //    .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FullName.Split(' ')[0]))
        //    .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.FullName.Split(' ').Last()))
        //    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive? "Active" : "Inactive"));
    }
}