using Microsoft.AspNetCore.Identity;

namespace ShopApp.Entites
{
	public class AppRole : IdentityRole<int>
	{
		public int Id { get; set; }

		public List<RoleUser> Users { get; set; }
	}
}
