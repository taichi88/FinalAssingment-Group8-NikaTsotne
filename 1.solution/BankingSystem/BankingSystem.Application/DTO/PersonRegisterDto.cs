using System.ComponentModel.DataAnnotations;
using BankingSystem.Domain.CustomValidationAttributes;
using BankingSystem.Domain.Enums;

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
    [PasswordValidation]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    [EnumDataType(typeof(RoleType), ErrorMessage = "Invalid role type. Use 'Operator', 'Person' or 'Manager'.")]
    public required RoleType Role { get; set; }
}