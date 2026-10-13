using Microsoft.AspNetCore.Identity;

namespace TradeJournal.Api.Entities;

public class Strategy
{
    public int Id { get; set; }

    public String UserId { get; set; } = null!;
    public IdentityUser User { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<Trade> Trades { get; set; } = new();
}