using System.ComponentModel.DataAnnotations;


namespace Shared
{
    public sealed class UserForCreationDto
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        [Required, MaxLength(20)]
        public string UserName { get; set; }
        [Required, MaxLength(50), EmailAddress]
        public string Email { get; set; }
    }
}
