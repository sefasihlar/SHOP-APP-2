using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Concrete;
using ShopApp.DataAccess.Concrete.EfCore;
using ShopApp.Entites;
using ShopApp.WebUI.Extensions;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{

	public class AdminController : Controller
	{
		ProductManager ip = new ProductManager(new EfCoreProductDal());
		CategoryManager _category = new CategoryManager(new EfCoreCategoryDal());
		OrderManager _orderManager = new OrderManager(new EfCoreOrderDal());

		public IActionResult AdminHomePage()
		{
			return View(new ProductModel()
			{
				Products = ip.GetALl()
			});
		}


		public IActionResult OrderList()
		{
			var values = _orderManager.GetAllOrders();
			return View(values);
		}

		public IActionResult OrderDetails(int id)
		{
			var values = _orderManager.GetWithOrderId(id);
			return View(values);
		}

		


		public IActionResult Index()
		{
			return View(new ProductModel()
			{
				Products = ip.GetALl()
			});
		}
		[HttpGet]
		public IActionResult CreateProduct()
		{
			return View(new AddProductModel()
			{

			});
		}

		[HttpPost]
		public async Task<IActionResult> CreateProduct(AddProductModel model, IFormFile? file, int[]? categoryIds)
		{
			if (ModelState.IsValid)
			{
				var values = new Product()
				{
					Name = model.Name,
					Price = model.Price,
					Gender = model.Gender,
					ImageUrl = model.ImageUrl,
				};

				if (file != null)
				{
					var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Template\\img\\product", file.FileName);
					using (var stream = new FileStream(path, FileMode.Create))
					{
						await file.CopyToAsync(stream);
					}
				}

				ip.Create(values);
				TempData.Put("Message", new ResultMessage()
				{
					Title = "Başarılı",
					Message = "Ürün Ekleme işlemi başarılı",
					Css = "success"
				});
				return RedirectToAction("Index", "Admin");
			}
			TempData.Put("Message", new ResultMessage()
			{
				Title = "Hata",
				Message = "Ürün Eklenemedi lütfen bilgileri gözden geçiriniz",
				Css = "error"
			});
			return View(model);


		}
		[HttpPost]
		public IActionResult ProductDelete(int id)
		{
			var values = ip.GetById(id);
			ip.Delete(values);
			TempData.Put("Message", new ResultMessage()
			{
				Title = "Başarılı",
				Message = "Ürün silme işlemi başarılı",
				Css = "success"
			});
			return RedirectToAction("Index", "Admin");
		}

		[HttpGet]
		public IActionResult EditProduct(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var values = ip.GetByIdWithCategories((int)id);

			if (values == null)
			{
				TempData.Put("Message", new ResultMessage()
				{
					Title = "Hata",
					Message = "Ürün bulunamadı lütfen daha sonra tekrar deneyiniz",
					Css = "error"
				});
			}

			var model = new AddProductModel()
			{
				Id = values.Id,
				Name = values.Name,
				Price = values.Price,
				ImageUrl = values.ImageUrl,
				Gender = values.Gender,
				Condition = values.Condition,
				SelectedCategories = values.ProductCategories.Select(x => x.Category).ToList()

			};

			ViewBag.Categories = _category.GetALl();

			return View(model);


		}

		[HttpPost]
		public async Task<IActionResult> EditProduct(AddProductModel model, int[] categoryIds, IFormFile file)
		{

			var entity = ip.GetById(model.Id);

			if (entity == null)
			{
				return NotFound();
			}

			entity.Name = model.Name;
			entity.Price = model.Price;
			entity.Gender = model.Gender;
			entity.Condition = model.Condition;
			if (file != null)
			{
				entity.ImageUrl = file.FileName;

				var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Theme\\img\\product", file.FileName);
				using (var stream = new FileStream(path, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				};

			}


			ip.Update(entity, categoryIds);
			TempData.Put("Message", new ResultMessage()
			{
				Title = "Başarılı",
				Message = "Ürün güncelleme işlemi başarılı",
				Css = "success"
			});
			return RedirectToAction("Index", "Admin");



			ViewBag.Categories = _category.GetALl();

			return View(model);

		}


        public IActionResult ProductList()
        {
            return View(new ProductModel()
            {
                Products = ip.GetALl().ToList(),
            });

        }


        public IActionResult CategoryList()
		{
			return View(new CategoryListModel()
			{
				Categories = _category.GetALl()
			});

		}

		[HttpGet]
		public IActionResult CreateCategory()
		{
			return View();
		}

		[HttpPost]
		public IActionResult CreateCategory(CategoryModel model)
		{
			var values = new Category()
			{
				Name = model.Name
			};
			_category.Create(values);
			TempData.Put("Message", new ResultMessage()
			{
				Title = "Başarılı",
				Message = "Kategoriy Ekleme işlemi başarılı",
				Css = "success"
			});

			return RedirectToAction("CategoryList", "Admin");
		}
		[HttpGet]
		public IActionResult EditCategory(int id)
		{
			var values = _category.GetByIdWithProducuts(id);
			return View(new CategoryModel()
			{
				Id = values.Id,
				Name = values.Name,
				Products = values.ProductCategories.Select(p => p.Product).ToList()
			});
		}

		[HttpPost]
		public IActionResult EditCategory(CategoryModel model)
		{
			var values = _category.GetById(model.Id);

			if (values == null)
			{
				TempData.Put("Message", new ResultMessage()
				{
					Title = "Opps!!! Hata",
					Message = "Birşetler ters gitti lütfen daha sonra tekrar deneyiniz",
					Css = "error"
				});
			}

			values.Name = model.Name;
			_category.Update(values);
			TempData.Put("Message", new ResultMessage()
			{
				Title = "Başarılı",
				Message = "Kategoriy güncelleme işlemi başarılı",
				Css = "success"
			});

			return RedirectToAction("CategoryList", "Admin");
		}

		public IActionResult DeleteCategory(int id)
		{

			var values = _category.GetById(id);
			if (values != null)
			{
				_category.Delete(values);
				TempData.Put("Message", new ResultMessage()
				{
					Title = "Başarılı",
					Message = "Kategoriy silme işlemi başarıyla gerçekleşti",
					Css = "success"
				});
			}

			else
			{
				TempData.Put("Message", new ResultMessage()
				{
					Title = "Opps! Hata",
					Message = "Kategoriy silme işlemi başarısınz birşeyler ters gitti lütfen daha sonra tekrar deneyiniz",
					Css = "error"
				});
			}


			return RedirectToAction("CategoryList", "Admin");
		}

		[HttpPost]
		public IActionResult DeleteFromCategory(int id, int categoryid)
		{
			_category.DeleteFromCategory(id, categoryid);
			TempData.Put("Message", new ResultMessage()
			{
				Title = "Başarılı",
				Message = "İlişki silme işlemi başarılı bir şekilde gerçekleşti ürün tamamen değil sadece bu categoriden silildi!! :)",
				Css = "success"
			});
			return Redirect("/admin/editcategory/" + categoryid);
		}
	}
}
