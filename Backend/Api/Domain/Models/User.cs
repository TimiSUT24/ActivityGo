namespace Domain.Models;

public class User : IdentityUser
{
   public string Firstname {get; set;}
   public string Lastname {get; set;}
}
