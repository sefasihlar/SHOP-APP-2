using ShopApp.Entites;

namespace ShopApp.Business.Abstract
{
    public interface IOrderService
    {
        void Create(Order entity);
        List<Order> GetWithOrderId(int orderId);
        Order GetById(int id);

        List<Order> GetAllOrders();
        List<Order> GetOrders(string? userId);
    }
}
