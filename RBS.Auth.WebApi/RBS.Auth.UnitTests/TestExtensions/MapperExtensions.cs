using AutoMapper;
using RBS.Auth.WebApi.MappingProfiles;

namespace RBS.Auth.UnitTests.TestExtensions;

public static class MapperExtensions
{
    private static readonly ModelProfile Profile = new();

    private static readonly MapperConfiguration Configuration = 
        new(cfg => cfg.AddProfile(Profile));

    public static readonly Mapper Mapper = new(Configuration);
}