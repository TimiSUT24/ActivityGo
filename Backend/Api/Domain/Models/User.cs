using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class User : IdentityUser
{
   public string Firstname {get; set;}
   public string Lastname {get; set;}
   
   // Navigation property
   public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

}
