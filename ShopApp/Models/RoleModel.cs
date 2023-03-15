using System.ComponentModel.DataAnnotations;

namespace ShopApp.WebUI.Models
{
	public class RoleModel
	{
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }
	}
}
