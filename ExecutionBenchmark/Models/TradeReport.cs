using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExecutionBenchmark.Models;

[Table("TradeReports")]
public class TradeReport
{
    public Guid Id { get; set; }

    public string OrderId { get; set; }

    public string Symbol { get; set; }

    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 8)")]
    public decimal ExecutedPrice { get; set; }

    [Column(TypeName = "decimal(18, 8)")]
    public decimal ExecutedQuantity { get; set; }

    public DateTime ExecutionTime { get; set; }

    public string ClientName { get; set; }

    public TradeStatus Status { get; set; }

    public enum TradeStatus
    {
        Executed,
        PartiallyFilled,
        Failed
    }
}