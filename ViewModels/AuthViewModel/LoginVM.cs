using System.ComponentModel.DataAnnotations;

namespace ProductMVC.ViewModels.AuthViewModel;

public class LoginVM
{
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required, MaxLength(256), MinLength(6), DataType(DataType.Password), Compare(nameof(Password))]
    public string Password { get; set; }
    public bool IsRemember { get; set; }
}

