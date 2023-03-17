using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
            return View(new OrderInfoListModel()
            {
                Orders = _orderManager.GetAll()
            });
        }

        public IActionResult OrderDetails(int id)
        {
            var order = _orderManager.GetWithOrderId(id);
            return View(order);
        }




        public IActionResult Index()
        {
            return View(new OrderInfoListModel()
            {
                Orders = _orderManager.GetAll()
            });
        }
        [HttpGet]
        public IActionResult CreateProduct()
        {
            ViewBag.Categories = _category.GetALl();

            return View(new AddProductModel()
            {

            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(AddProductModel model, int[] categoryIds, IFormFile file)
        {

            var values = new Product()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Gender = model.Gender,
                ImageUrl = file.FileName,
                Condition = model.Condition,

            };

            if (file != null)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Theme\\img\\product", file.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            if (values != null)
            {
                ip.Create(values);
            }

            if (values != null)
            {
                ip.Create(values, categoryIds);
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
            ViewBag.Categories = _category.GetALl();
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
                Description = values.Description,
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
            entity.Description = model.Description;
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
        public IActionResult CategoryEdit(int Id)
        {
            if (Id==0)
            {
                return View();
            }
            var values = _category.GetByIdWithProducuts(Id);

            

            var products = new CategoryModel()
            {
                Id = values.Id,
                Name = values.Name,
                Products = values.ProductCategories.Select(p => p.Product).ToList()
            };

            return View(products);

        }

        [HttpPost]
        public IActionResult CategoryEdit(CategoryModel model)
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
            return RedirectToAction("CategoryList","Admin");
        }
    }
}
