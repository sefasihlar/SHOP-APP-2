using Microsoft.AspNetCore.Identity;

namespace ShopApp.Entites
{
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; }
    }
}
