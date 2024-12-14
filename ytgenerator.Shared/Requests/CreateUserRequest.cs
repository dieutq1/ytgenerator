using System.ComponentModel.DataAnnotations;
using ytgenerator.Shared;

namespace ytgenerator.Shared.Requests;
public class CreateUserRequest
{
    [Required]
    public string Email { get; set; }
    public SystemRole Role { get; set; }

    [Required]
    public string Name { get; set; }
    public string Phone { get; set; }
    [Required]
    public string Password { get; set; }

}
