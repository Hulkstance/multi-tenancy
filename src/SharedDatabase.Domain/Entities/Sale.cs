namespace SharedDatabase.Domain.Entities;

public class Sale
{
    public required int Id { get; set; }
    public required decimal Amount { get; set; }
    public required DateTime CreatedAt { get; set; }

    public required int CompanyId { get; set; }
    public required Company Company { get; set; }
}
