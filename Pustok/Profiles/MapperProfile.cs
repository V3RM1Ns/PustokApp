using AutoMapper;

namespace Pustok.Profiles;

public class MapperProfile:Profile
{
    public MapperProfile()
    {
        CreateMap<Models.Book, ViewModels.BookTestVm>()
            .ForMember(dest => dest.BookTagNames,
                opt => opt.MapFrom(src => src.BookTags.Select(bt => bt.Tag.Name).ToList()))
            .ForMember(dest => dest.BookmageUrls,
                opt => opt.MapFrom(src => src.BookImages.Select(bi => bi.Image).ToList()));
    }
}