using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Concrete;
using ShopApp.DataAccess.Concrete.EfCore;
using ShopApp.Entites;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
	public class ShopController : Controller
	{
		ProductManager ip = new ProductManager(new EfCoreProductDal());
		CategoryManager ic = new CategoryManager(new EfCoreCategoryDal());

		public IActionResult Index()
		{
			return View();
		}
		//AllList/category?page = 1
		public async Task<IActionResult> AllList(String category, int page = 1)
		{

			const int pageSize = 9;
			return View(new ProductModel()
			{
				PageInfo = new PageInfo()
				{
					TotalItems = ip.GetCountByCategory(category),
					CurrentPage = page,
					ItemsPerPage = pageSize,
					CurrenCategory = category,

				},
				Products = ip.GetProductsByCategory(category, page, pageSize).Where(x => x.Condition == "True").ToList()
			});
		}


		public IActionResult Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			Product product = ip.GetById((int)id);
			if (product == null)
			{
				return NotFound();
			}
			return View(product);
		}


		public PartialViewResult CategoryList()
		{


			return PartialView(new CategoryListViewModel()
			{
				kategoriler = ic.GetALl()
			});

		}

		public IActionResult iliski()
		{

			return View();
		}

	}
}
