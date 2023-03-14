using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Concrete;
using ShopApp.DataAccess.Concrete.EfCore;
using ShopApp.Entites;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.ViewComponents
{
    public class TotalBagViewComponent : ViewComponent
    {
        CartManager _cartManager = new CartManager(new EfCoreCartDal());
        private UserManager<AppUser> _userManager;

        public TotalBagViewComponent(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }


        public IViewComponentResult Invoke()
        {
            var cart = _cartManager.GetCartByUserId(_userManager.GetUserId((System.Security.Claims.ClaimsPrincipal)User));
            return View(new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(x => new CartItemModel()
                {
                    CartItemId = x.Id,
                    ProductId = x.ProductId,
                    Name = x.Product.Name,
                    Price = Convert.ToDecimal(x.Product.Price),
                    ImageUrl = x.Product.ImageUrl,
                    Quantity = x.Quantity

                }).ToList()
            });
        }
    }
}
