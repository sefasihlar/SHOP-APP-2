using ShopApp.Entites;

namespace ShopApp.DataAccess.Abstract
{
	public interface ICategoryDal : IRepository<Category>
	{
		void DeleteFromCategory(int id, int categoryid);
		Category GetByIdWithProducuts(int id);
	}
}
