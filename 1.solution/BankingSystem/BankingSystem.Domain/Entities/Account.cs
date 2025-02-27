﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace BankingSystem.Domain.Entities;

public class Account
{
    public int AccountId { get; set; }

    [Required(ErrorMessage = "IBAN is required.")]
    [StringLength(34, MinimumLength = 15, ErrorMessage = "IBAN must be between 15 and 34 characters.")]
    public required string IBAN { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Required(ErrorMessage = "Balance is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Balance cannot be negative.")]
    public decimal Balance { get; set; }

    [Required(ErrorMessage = "Currency is required.")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter ISO code.")]
    public required string Currency { get; set; }

    [Required(ErrorMessage = "Person ID is required.")]
    public required string PersonId { get; set; }
}