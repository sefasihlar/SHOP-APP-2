

namespace ShopApp.WebUI.Models
{
	public class CartModel
	{
		public int CartId { get; set; }
		public List<CartItemModel> CartItems { get; set; }

		public int TotalPrice()
		{
			return CartItems.Sum(x => x.Price * x.Quantity);
		}

		public int
			BagTotal()
		{
			return CartItems.Sum(x => x.Quantity);
		}
	}
	public class CartItemModel
	{
		public int CartItemId { get; set; }
		public int ProductId { get; set; }
		public string Name { get; set; }
		public int Price { get; set; }
		public string ImageUrl { get; set; }
		public int Quantity { get; set; }

	}
}
