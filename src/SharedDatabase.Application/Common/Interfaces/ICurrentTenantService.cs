namespace SharedDatabase.Application.Common.Interfaces;

public interface ICurrentTenantService
{
    string Id { get; }

    string Identifier { get; }
}
