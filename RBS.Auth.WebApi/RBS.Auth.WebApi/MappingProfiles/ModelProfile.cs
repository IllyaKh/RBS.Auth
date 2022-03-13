using AutoMapper;
using RBS.Auth.Common.Models;
using RBS.Auth.WebApi.Models;

namespace RBS.Auth.WebApi.MappingProfiles;

public class ModelProfile : Profile
{
    public ModelProfile()
    {
        CreateMap<RegisterRequest, RegisterModel>();
    }
}