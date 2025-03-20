using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.Entities;

public class Person
{
    [Key]
    [Required(ErrorMessage = "Person ID is required.")]
    public required string PersonId { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Lastname is required.")]
    public required string Lastname { get; set; }

    [Required(ErrorMessage = "ID number is required.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "ID number must be exactly 11 digits.")]
    public required string IdNumber { get; set; }

    [Required(ErrorMessage = "Birth date is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid birth date format.")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public required string Email { get; set; }
    [Required(ErrorMessage = "Registration date is required")]
    public DateOnly RegistrationDate { get; set; }
    public IList<Account> Accounts { get; set; }
    public IList<Card> Cards { get; set; }
}
