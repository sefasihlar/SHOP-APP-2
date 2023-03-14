using ShopApp.Entites;

namespace ShopApp.Business.Abstract
{
    public interface ICartService
    {
        void InitializeCart(string userId);

        List<Cart> GetListCartItem();

        void DeleteFromCart(string userId, int productId);

        Cart GetCartByUserId(string userId);

        void AddToCart(string userId, int productId, int quantity);

        void ClearCart(string cartId);


    }
}
