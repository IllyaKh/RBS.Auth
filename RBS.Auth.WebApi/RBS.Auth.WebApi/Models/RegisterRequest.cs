using System.ComponentModel.DataAnnotations;

namespace RBS.Auth.WebApi.Models;

public class RegisterRequest
{
    [Required] 
    public string FirstName { get; set; }

    [Required] 
    public string LastName { get; set; }

    [Required] 
    [EmailAddress] 
    public string Email { get; set; }

    [Required] 
    public string Password { get; set; }
}