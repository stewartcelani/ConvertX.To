using ConvertX.To.Application.Interfaces;
using ConvertX.To.Application.Interfaces.Repositories;
using ConvertX.To.Infrastructure.Shared.Services;
using NSubstitute;

namespace ConvertX.To.Infrastructure.Shared.Tests.Unit;

public class ConversionServiceTests
{
    private readonly IConversionService _sut;
    private readonly IConversionRepository _conversionRepository = Substitute.For<IConversionRepository>();

    public ConversionServiceTests()
    {
        _sut = new ConversionService(_conversionRepository);
    }
    
    
}