using Microsoft.AspNetCore.Identity;

namespace ShopApp.WebUI.Models
{
    public class AppUserModel : IdentityUser<int>
    {
        public string FullName { get; set; }
        public string? SellerNumber { get; set; }
    }
}
