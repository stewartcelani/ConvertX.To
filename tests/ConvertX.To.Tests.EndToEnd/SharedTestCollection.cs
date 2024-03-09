namespace ConvertX.To.Tests.EndToEnd;

[CollectionDefinition(nameof(SharedTestCollection))]
public class SharedTestCollection : ICollectionFixture<SharedTestContext>
{
}