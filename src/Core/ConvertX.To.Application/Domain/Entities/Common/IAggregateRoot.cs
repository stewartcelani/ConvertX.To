namespace ConvertX.To.Application.Domain.Entities.Common;

/// <summary>
/// Marker interface, repositories will only work with aggregate roots
/// https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/net-core-microservice-domain-model
/// </summary>
public interface IAggregateRoot
{
}