using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Entites;
using ShopApp.WebUI.Extensions;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
	public class RoleController : Controller
	{
		private readonly RoleManager<AppRole> _roleManager;

		public RoleController(RoleManager<AppRole> roleManager)
		{
			_roleManager = roleManager;
		}

		public IActionResult Index()
		{
			var values = new AppRoleListModel()
			{
				Roles = _roleManager.Roles.ToList(),
			};

			return View(values);
		}

		[HttpGet]
		public IActionResult Create()
		{

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(RoleModel model)
		{
			var values = new AppRole()
			{
				Name = model.Name,
			};


			var result = await _roleManager.CreateAsync(values);

			if (result.Succeeded)
			{
				TempData.Put("message", new ResultMessage()
				{
					Title = "Başarılı",
					Message = "Role başarıyla eklendi",
					Css = "success"
				});
				return RedirectToAction("Index", "Role");
			}



			TempData.Put("message", new ResultMessage()
			{
				Title = "Hata",
				Message = "Role ekleme işlemi başarısız.Lütfen tekarar deneyiniz",
				Css = "error"
			});

			return View(model);
		}

		[HttpGet]
		public IActionResult Detail(int id)
		{

			var values = _roleManager.Roles.FirstOrDefault(x => x.Id == id);


			if (values == null)
			{
				TempData.Put("message", new ResultMessage()
				{
					Title = "Hata",
					Message = "Role bulunamadı lütfen tekrar deneyiniz",
					Css = "error"
				});
			}

			var model = new RoleModel()
			{
				Id = values.Id,
				Name = values.Name,

			};

			return View(model);
		}


		[HttpPost]
		public async Task<IActionResult> Update(RoleModel model)
		{

			var values = _roleManager.Roles.FirstOrDefault(x => x.Id == model.Id);

			if (values != null)
			{
				values.Name = model.Name;
				var result = await _roleManager.UpdateAsync(values);
				TempData.Put("message", new ResultMessage()
				{
					Title = "Başarılı",
					Message = "Role günceleme işlemi başarılı",
					Css = "success"
				});
				if (result.Succeeded)
				{

					return RedirectToAction("Index", "Role");
				}
			}
			TempData.Put("message", new ResultMessage()
			{
				Title = "Hata",
				Message = "Role ekle işlemi başarısız.Lütfen daha sonra tekarar deneyiniz",
				Css = "error"
			});
			return View(model);
		}




	}

}
