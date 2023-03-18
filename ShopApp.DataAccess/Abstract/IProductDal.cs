using ShopApp.Entites;

namespace ShopApp.DataAccess.Abstract
{
    public interface IProductDal : IRepository<Product>
    {
        void Create(Product entity, int[] categoryIds);
        Product GetByIdWithCategories(int id);
        int GetCountByCategory(string category);
        IEnumerable<Product> GetPopularProduct();
        IEnumerable<Product> GetLastAddProduct();
        List<Product> GetProductsByCategory(string category, int page, int pageSize);
        void Update(Product entity, int[] categoryIds);
    }
}
