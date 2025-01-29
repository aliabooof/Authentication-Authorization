using System.ComponentModel.DataAnnotations;

namespace Authentication___Authorization.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }
}
