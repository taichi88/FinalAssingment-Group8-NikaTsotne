﻿using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Application.DTO;

public class PersonRegisterDto
{
    [Required(ErrorMessage = "Name is required.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Lastname is required.")]
    public required string Lastname { get; set; }

    [Required(ErrorMessage = "ID number is required.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "ID number must be exactly 11 characters.")]
    public required string IdNumber { get; set; }

    [Required(ErrorMessage = "Birth date is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid birth date format.")]
    public required DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    [AllowedValues("Operator", "Person", ErrorMessage = "Role must be either 'Operator' or 'Person'.")]
    public required string Role { get; set; }
}