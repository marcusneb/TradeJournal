using Microsoft.AspNetCore.Identity;

namespace TradeJournal.Api.Entities;

public class Trade
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public IdentityUser User { get; set; } = null!;

    public int? StrategyId { get; set; }
    public Strategy? Strategy { get; set; }

    public string Symbol { get; set; } = null!;
    public Entities.AssetType AssetType { get; set; }
    public TradeDirection Direction { get; set; }
    public decimal Quantity { get; set; }

    public decimal EntryPrice { get; set; }
    public DateTime EntryDate { get; set; }

    public decimal? ExitPrice { get; set; }   // null = trade still open
    public DateTime? ExitDate { get; set; }

    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
    public decimal Fees { get; set; }

    public string? Notes { get; set; }
    public int? Rating { get; set; }          // 1-5
    public bool? FollowedPlan { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}