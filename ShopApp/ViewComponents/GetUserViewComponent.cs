using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Concrete;
using ShopApp.DataAccess.Concrete.EfCore;
using ShopApp.Entites;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.ViewComponents
{
    public class GetUserViewComponent:ViewComponent
    {
        UserManager _appUserManager = new UserManager(new EfCoreUserDal());

        private UserManager<AppUser> _userManager;

        public GetUserViewComponent(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            var userId = _userManager.GetUserId((System.Security.Claims.ClaimsPrincipal)User);

            var values = _appUserManager.GetById(Convert.ToInt32(userId));
            return View(new AppUserModel()
            {
                Id = values.Id,
                UserName = values.UserName,
                FullName = values.FullName,
                Email = values.Email,
                PhoneNumber = values.PhoneNumber,
                SellerNumber = values.SellerNumber,
            });
        }
    }
}
