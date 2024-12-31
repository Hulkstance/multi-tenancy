namespace SharedDatabase.Application.MultiTenancy;

public interface ICurrentTenantService
{
    string Id { get; }
    string Identifier { get; }
}
