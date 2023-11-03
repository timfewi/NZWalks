using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTO
{
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [MaxLength(100, ErrorMessage = "Email has to be a max of 100 characters")]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }



        [Required]
        [MaxLength(50, ErrorMessage = "Password has to be a max of  50 Characters")]
        [DataType(DataType.Password)]

        public string Password { get; set; }
    }
}

