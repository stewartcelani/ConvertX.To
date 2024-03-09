using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ConvertX.To.Tests.Integration;


[CollectionDefinition(nameof(SharedTestCollection))]
public class SharedTestCollection : ICollectionFixture<SharedTestContext>
{
}