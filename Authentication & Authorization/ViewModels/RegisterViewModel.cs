using System.ComponentModel.DataAnnotations;

namespace Authentication___Authorization.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = default!;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = default!;

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;


        [Required]
        [DataType(DataType.Password)]
        [MinLength(8,ErrorMessage ="Password must have a minimum length of '8'")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).+$",
            ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one special character.")]
        public string Password { get; set; }= default!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

    }
}
