using ShopApp.Entites;

namespace ShopApp.DataAccess.Abstract
{
    public interface IOrderDal : IRepository<Order>
    {
        List<Order> GetAllOrders();

        List<Order> GetWithOrderId(int orderId);
        List<Order> GetOrders(string? userId);
    }
}
