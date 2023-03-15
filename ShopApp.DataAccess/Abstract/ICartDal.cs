using ShopApp.Entites;

namespace ShopApp.DataAccess.Abstract
{
	public interface ICartDal : IRepository<Cart>
	{
		void ClearCart(string cartId);
		void DeleteFromCart(int cartId, int productId);
		Cart GetByUserId(string userId);
		List<Cart> GetListCartItem();
	}
}
