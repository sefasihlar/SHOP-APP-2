using ShopApp.Entites;

namespace ShopApp.DataAccess.Abstract
{
	public interface IProductDal : IRepository<Product>
	{
		Product GetByIdWithCategories(int id);
		int GetCountByCategory(string category);
		IEnumerable<Product> GetPopularProduct();
		List<Product> GetProductsByCategory(string category, int page, int pageSize);
		void Update(Product entity, int[] categoryIds);
	}
}
