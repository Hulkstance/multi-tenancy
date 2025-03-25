namespace SharedDatabase.Domain.Entities;

public class Sale
{
    public required Guid Id { get; set; }
    public required decimal Amount { get; set; }
    public required DateTime CreatedAt { get; set; }

    public required Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
}
