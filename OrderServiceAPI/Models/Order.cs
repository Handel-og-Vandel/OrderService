using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models;

public class Order
{
    [Required]
    public Guid OrderId { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public DateTime OrderDate { get; set; }

    [Required]
    [MinLength(1)]
    public required List<OrderItem> Items { get; set; }

    [Required]
    public required Address ShippingAddress { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter<PaymentMethod>))]
    public PaymentMethod PaymentMethod { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public double TotalAmount { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter<OrderStatus>))]
    public OrderStatus OrderStatus { get; set; }
}

public class OrderItem
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public double Price { get; set; }
}

public class Address
{
    [Required]
    public required string Street { get; set; }

    [Required]
    public required string City { get; set; }

    public string? State { get; set; } // State is optional, so no [Required]

    [Required]
    public required string PostalCode { get; set; }

    [Required]
    public required string Country { get; set; }
}

public enum PaymentMethod
{
    credit_card,
    paypal,
    bank_transfer
}

public enum OrderStatus
{
    pending,
    processing,
    shipped,
    delivered,
    cancelled
}