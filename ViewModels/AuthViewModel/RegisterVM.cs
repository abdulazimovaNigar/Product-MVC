using System.ComponentModel.DataAnnotations;

namespace ProductMVC.ViewModels.AuthViewModel;

public class RegisterVM
{
    [Required, MaxLength(100), MinLength(3)]
    public string FirstName { get; set; }
    [Required, MaxLength(100), MinLength(3)]
    public string LastName { get; set; }
    [Required, MaxLength(100), MinLength(3)]
    public string UserName { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required, MaxLength(256), MinLength(6), DataType(DataType.Password)]
    public string Password { get; set; }
    [Required, MaxLength(256), MinLength(6), DataType(DataType.Password), Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }

}
