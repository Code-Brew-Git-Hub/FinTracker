using FinTracker.Application;
using Mapster;
using MapsterMapper;

namespace Application.UnitTests;

internal static class TestMapperFactory
{
    private static readonly Lazy<IMapper> Mapper = new(() =>
    {
        MappingConfig.Configure();
        return new Mapper(TypeAdapterConfig.GlobalSettings);
    });

    public static IMapper Create() => Mapper.Value;
}
