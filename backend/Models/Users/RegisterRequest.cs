using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Users
{
    public class RegisterRequest
    {

        [Required]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }


        public int id { get; set; }
    }
}
