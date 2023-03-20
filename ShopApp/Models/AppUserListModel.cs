using ShopApp.Entites;

namespace ShopApp.WebUI.Models
{
    public class AppUserListModel
    {
        public List<AppUser> Users { get; set; }
        public List<AppRole> Roles { get; set; }
    }
}
