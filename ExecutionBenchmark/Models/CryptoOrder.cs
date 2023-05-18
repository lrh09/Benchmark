using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExecutionBenchmark.Models;

[Table("CryptoOrders")]
public class CryptoOrder
{
    public Guid Id { get; set; }
    public string OrderId { get; set; }
    public string Symbol { get; set; }

    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 8)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(18, 8)")]
    public decimal Quantity { get; set; }

    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public string ClientName { get; set; }
    public OrderType Type { get; set; }

    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 8)")]
    public decimal StopLoss { get; set; }

    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 8)")]
    public decimal TakeProfit { get; set; }

    // Additional properties can be added based on your requirements

    public enum OrderStatus
    {
        Pending,
        Filled,
        Cancelled
    }

    public enum OrderType
    {
        Buy,
        Sell
    }
}