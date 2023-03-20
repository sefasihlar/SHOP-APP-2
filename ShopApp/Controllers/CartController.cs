using IyzipayCore.Model;
using IyzipayCore.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.Business.Concrete;
using ShopApp.DataAccess.Concrete.EfCore;
using ShopApp.Entites;
using ShopApp.WebUI.Extensions;
using ShopApp.WebUI.Models;
using Options = IyzipayCore.Options;

namespace ShopApp.WebUI.Controllers
{

    public class CartController : Controller
    {
        private readonly IEmailSenderService _IEmailSederService;
        private UserManager<AppUser> _userManager;
        OrderManager _orderManager = new OrderManager(new EfCoreOrderDal());
        CartManager _cartManager = new CartManager(new EfCoreCartDal());

        public CartController(UserManager<AppUser> userManager, IEmailSenderService emailSenderService)
        {
            _userManager = userManager;
            _IEmailSederService = emailSenderService;
        }

        public IActionResult Index()
        {
            var cart = _cartManager.GetCartByUserId(_userManager.GetUserId(User));
            return View(new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(x => new CartItemModel()
                {
                    CartItemId = x.Id,
                    ProductId = x.ProductId,
                    Name = x.Product.Name,
                    Price = x.Product.Price,
                    ImageUrl = x.Product.ImageUrl,
                    Quantity = x.Quantity



                }).ToList()
            });
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            if (userId==null)
            {
                TempData.Put("Message", new ResultMessage()
                {
                    Title = "Sepete Ekle",
                    Message = "Giriş yapmanız gerekemektedir",
                    Css = "warning"
                });
                return RedirectToAction("Login","Account");
            }
            _cartManager.AddToCart(userId, productId, quantity);
            TempData.Put("Message", new ResultMessage()
            {
                Title = "Başarılı",
                Message = "Ürün sepete eklendi",
                Css = "success"
            });
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult DeleteFromCart(int productId)
        {
            _cartManager.DeleteFromCart(_userManager.GetUserId(User), productId);
            TempData.Put("Message", new ResultMessage()
            {
                Title = "Başarılı",
                Message = "Ürün sepetten silindi",
                Css = "success"
            });
            return RedirectToAction("Index", "Cart");
        }


        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = _cartManager.GetCartByUserId(_userManager.GetUserId(User));

            var orderModel = new OrderModel();

            orderModel.CartModel =

                new CartModel()
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(x => new CartItemModel()
                    {
                        CartItemId = x.Id,
                        ProductId = x.ProductId,
                        Name = x.Product.Name,
                        Price = x.Product.Price,
                        ImageUrl = x.Product.ImageUrl,
                        Quantity = x.Quantity

                    }).ToList()
                };
            return View(orderModel);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(OrderModel model)
        {

            var userId = _userManager.GetUserId(User);
            var cart = _cartManager.GetCartByUserId(userId);
            model.Condition = "Sipariş Alındı";
            model.CartModel = new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(x => new CartItemModel()
                {
                    CartItemId = x.Id,
                    ProductId = x.ProductId,
                    Name = x.Product.Name,
                    Price = x.Product.Price,
                    ImageUrl = x.Product.ImageUrl,
                    Quantity = x.Quantity

                }).ToList()
            };

            var payment = PaymentRrocess(model);

            if (User.IsInRole("User") == true)
            {
                if (payment.Status == "success")
                {
                    SaveOrder(model, payment, userId);
                    TempData.Put("Message", new ResultMessage()
                    {
                        Title = "Başarılı",
                        Message = "Siparişiniz başarıyla alındı.Durumu sipariş durumunuzu siparişlerim kısmında görüntüleyebilirsiniz",
                        Css = "success"
                    });
                    ClearCart(cart.Id.ToString());
                    await _IEmailSederService.SendEmailAsync("sihlarsefa7@gmail.com", "Yeni Sipariş Alındı", $"<!DOCTYPE html>\r\n<html>\r\n\r\n<head>\r\n    <title></title>\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />\r\n    <style type=\"text/css\">\r\n        @media screen {{\r\n            @font-face {{\r\n                font-family: 'Lato';\r\n                font-style: normal;\r\n                font-weight: 400;\r\n                src: local('Lato Regular'), local('Lato-Regular'), url(https://fonts.gstatic.com/s/lato/v11/qIIYRU-oROkIk8vfvxw6QvesZW2xOQ-xsNqO47m55DA.woff) format('woff');\r\n            }}\r\n\r\n            @font-face {{\r\n                font-family: 'Lato';\r\n                font-style: normal;\r\n                font-weight: 700;\r\n                src: local('Lato Bold'), local('Lato-Bold'), url(https://fonts.gstatic.com/s/lato/v11/qdgUG4U09HnJwhYI-uK18wLUuEpTyoUstqEm5AMlJo4.woff) format('woff');\r\n            }}\r\n\r\n            @font-face {{\r\n                font-family: 'Lato';\r\n                font-style: italic;\r\n                font-weight: 400;\r\n                src: local('Lato Italic'), local('Lato-Italic'), url(https://fonts.gstatic.com/s/lato/v11/RYyZNoeFgb0l7W3Vu1aSWOvvDin1pK8aKteLpeZ5c0A.woff) format('woff');\r\n            }}\r\n\r\n            @font-face {{\r\n                font-family: 'Lato';\r\n                font-style: italic;\r\n                font-weight: 700;\r\n                src: local('Lato Bold Italic'), local('Lato-BoldItalic'), url(https://fonts.gstatic.com/s/lato/v11/HkF_qI1x_noxlxhrhMQYELO3LdcAZYWl9Si6vvxL-qU.woff) format('woff');\r\n            }}\r\n        }}\r\n\r\n        /* CLIENT-SPECIFIC STYLES */\r\n        body,\r\n        table,\r\n        td,\r\n        a {{\r\n            -webkit-text-size-adjust: 100%;\r\n            -ms-text-size-adjust: 100%;\r\n        }}\r\n\r\n        table,\r\n        td {{\r\n            mso-table-lspace: 0pt;\r\n            mso-table-rspace: 0pt;\r\n        }}\r\n\r\n        img {{\r\n            -ms-interpolation-mode: bicubic;\r\n        }}\r\n\r\n        /* RESET STYLES */\r\n        img {{\r\n            border: 0;\r\n            height: auto;\r\n            line-height: 100%;\r\n            outline: none;\r\n            text-decoration: none;\r\n        }}\r\n\r\n        table {{\r\n            border-collapse: collapse !important;\r\n        }}\r\n\r\n        body {{\r\n            height: 100% !important;\r\n            margin: 0 !important;\r\n            padding: 0 !important;\r\n            width: 100% !important;\r\n        }}\r\n\r\n        /* iOS BLUE LINKS */\r\n        a[x-apple-data-detectors] {{\r\n            color: inherit !important;\r\n            text-decoration: none !important;\r\n            font-size: inherit !important;\r\n            font-family: inherit !important;\r\n            font-weight: inherit !important;\r\n            line-height: inherit !important;\r\n        }}\r\n\r\n        /* MOBILE STYLES */\r\n        @media screen and (max-width:600px) {{\r\n            h1 {{\r\n                font-size: 32px !important;\r\n                line-height: 32px !important;\r\n            }}\r\n        }}\r\n\r\n        /* ANDROID CENTER FIX */\r\n        div[style*=\"margin: 16px 0;\"] {{\r\n            margin: 0 !important;\r\n        }}\r\n    </style>\r\n</head>\r\n\r\n<body style=\"background-color: #f4f4f4; margin: 0 !important; padding: 0 !important;\">\r\n    <!-- HIDDEN PREHEADER TEXT -->\r\n    <div style=\"display: none; font-size: 1px; color: #fefefe; line-height: 1px; font-family: 'Lato', Helvetica, Arial, sans-serif; max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden;\"> We're thrilled to have you here! Get ready to dive into your new account.\r\n    </div>\r\n    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n        <!-- LOGO -->\r\n        <tr>\r\n            <td bgcolor=\"#FFA73B\" align=\"center\">\r\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n                    <tr>\r\n                        <td align=\"center\" valign=\"top\" style=\"padding: 40px 10px 40px 10px;\"> </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <td bgcolor=\"#FFA73B\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\">\r\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n                    <tr>\r\n                        <td bgcolor=\"#ffffff\" align=\"center\" valign=\"top\" style=\"padding: 40px 20px 20px 20px; border-radius: 4px 4px 0px 0px; color: #111111; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 48px; font-weight: 400; letter-spacing: 4px; line-height: 48px;\">\r\n                            <h1 style=\"font-size: 48px; font-weight: 400; margin: 2;\">Sipariş Aldık!</h1> <img src=\"https://img.icons8.com/clouds/512/shopping-cart-with-money.png\" width=\"125\" height=\"120\" style=\"display: block; border: 0px;\" />\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <td bgcolor=\"#f4f4f4\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\">\r\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n                    <tr>\r\n                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 20px 30px 40px 30px; color: #666666; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\">\r\n                            <p style=\"margin: 0;\"><div class='text-center'><strong>{model.CartModel.TotalPrice}</strong></div><br/>{model.FirstName} {model.LastName}'dan Sipariş aldık.<br/>İL:{model.City}<br/>İlçe:{model.District}<br/>Adres:{model.Address}</p>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td bgcolor=\"#ffffff\" align=\"left\">\r\n                            <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n                                <tr>\r\n                                    <td bgcolor=\"#ffffff\" align=\"center\" style=\"padding: 20px 30px 60px 30px;\">\r\n                                        <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n                                            <tr>\r\n                                                \r\n                                            </tr>\r\n                                        </table>\r\n                                    </td>\r\n                                </tr>\r\n                            </table>\r\n                        </td>\r\n                    </tr> <!-- COPY -->\r\n                    <tr>\r\n                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0px 30px 0px 30px; color: #666666; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\">\r\n                            <p style=\"margin: 0;\"></p>\r\n                        </td>\r\n                    </tr> <!-- COPY -->\r\n                    <tr>\r\n                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 20px 30px 20px 30px; color: #666666; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\">\r\n                            <p style=\"margin: 0;\"><a href=\"#\" target=\"_blank\" style=\"color: #FFA73B;\"></a></p>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0px 30px 20px 30px; color: #666666; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\">\r\n                            <p style=\"margin: 0;\"></p>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0px 30px 40px 30px; border-radius: 0px 0px 4px 4px; color: #666666; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\">\r\n                            <p style=\"margin: 0;\">:),<br>Meta Akdeniz Team</p>\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <td bgcolor=\"#f4f4f4\" align=\"center\" style=\"padding: 30px 10px 0px 10px;\">\r\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n                    <tr>\r\n                        <td bgcolor=\"#FFECD1\" align=\"center\" style=\"padding: 30px 30px 30px 30px; border-radius: 4px 4px 4px 4px; color: #666666; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\">\r\n                            <h2 style=\"font-size: 20px; font-weight: 400; color: #111111; margin: 0;\">Need more help?</h2>\r\n                            <p style=\"margin: 0;\"><a href=\"#\" target=\"_blank\" style=\"color: #FFA73B;\"></a></p>\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <td bgcolor=\"#f4f4f4\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\">\r\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n                    <tr>\r\n                        <td bgcolor=\"#f4f4f4\" align=\"left\" style=\"padding: 0px 30px 30px 30px; color: #666666; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 14px; font-weight: 400; line-height: 18px;\"> <br>\r\n                            <p style=\"margin: 0;\"><a href=\"#\" target=\"_blank\" style=\"color: #111111; font-weight: 700;\"></a>.</p>\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n</body>\r\n\r\n</html>");
                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    TempData.Put("Message", new ResultMessage()
                    {
                        Title = "Opps! Hata",
                        Message = "Birşeyler ters gitti.Lütefen bilgilerinizi kontrol ederek tekarar deneyiniz",
                        Css = "error"
                    });
                    return View(model);
                }
            }

            if (User.IsInRole("Seller") == true)
            {
                //kullanıcının rolune bağlı  da tutulabilir
                if (model.CardNumber == null & model.Cvv == null & model.CardName == null & model.ExpirationYear == null)
                {

                    SaveOrder(model, payment, userId);
                    TempData.Put("Message", new ResultMessage()
                    {
                        Title = "Bayi Siparişi Başarılı",
                        Message = "Siparişiniz başarıyla alındı.Durumu sipariş durumunuzu siparişlerim kısmında görüntüleyebilirsiniz",
                        Css = "success"
                    });

                    ClearCart(cart.Id.ToString());
                    await _IEmailSederService.SendEmailAsync("sihlarsefa7@gmail.com@gmail.com", "Yeni Sipariş Alındı", $"<!DOCTYPE html>\r\n<html>\r\n\r\n<head>\r\n    <title></title>\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />\r\n    <style type=\"text/css\">\r\n        @media screen {{\r\n            @font-face {{\r\n                font-family: 'Lato';\r\n                font-style: normal;\r\n                font-weight: 400;\r\n                src: local('Lato Regular'), local('Lato-Regular'), url(https://fonts.gstatic.com/s/lato/v11/qIIYRU-oROkIk8vfvxw6QvesZW2xOQ-xsNqO47m55DA.woff) format('woff');\r\n            }}\r\n\r\n            @font-face {{\r\n                font-family: 'Lato';\r\n                font-style: normal;\r\n                font-weight: 700;\r\n                src: local('Lato Bold'), local('Lato-Bold'), url(https://fonts.gstatic.com/s/lato/v11/qdgUG4U09HnJwhYI-uK18wLUuEpTyoUstqEm5AMlJo4.woff) format('woff');\r\n            }}\r\n\r\n            @font-face {{\r\n                font-family: 'Lato';\r\n                font-style: italic;\r\n                font-weight: 400;\r\n                src: local('Lato Italic'), local('Lato-Italic'), url(https://fonts.gstatic.com/s/lato/v11/RYyZNoeFgb0l7W3Vu1aSWOvvDin1pK8aKteLpeZ5c0A.woff) format('woff');\r\n            }}\r\n\r\n            @font-face {{\r\n                font-family: 'Lato';\r\n                font-style: italic;\r\n                font-weight: 700;\r\n                src: local('Lato Bold Italic'), local('Lato-BoldItalic'), url(https://fonts.gstatic.com/s/lato/v11/HkF_qI1x_noxlxhrhMQYELO3LdcAZYWl9Si6vvxL-qU.woff) format('woff');\r\n            }}\r\n        }}\r\n\r\n        /* CLIENT-SPECIFIC STYLES */\r\n        body,\r\n        table,\r\n        td,\r\n        a {{\r\n            -webkit-text-size-adjust: 100%;\r\n            -ms-text-size-adjust: 100%;\r\n        }}\r\n\r\n        table,\r\n        td {{\r\n            mso-table-lspace: 0pt;\r\n            mso-table-rspace: 0pt;\r\n        }}\r\n\r\n        img {{\r\n            -ms-interpolation-mode: bicubic;\r\n        }}\r\n\r\n        /* RESET STYLES */\r\n        img {{\r\n            border: 0;\r\n            height: auto;\r\n            line-height: 100%;\r\n            outline: none;\r\n            text-decoration: none;\r\n        }}\r\n\r\n        table {{\r\n            border-collapse: collapse !important;\r\n        }}\r\n\r\n        body {{\r\n            height: 100% !important;\r\n            margin: 0 !important;\r\n            padding: 0 !important;\r\n            width: 100% !important;\r\n        }}\r\n\r\n        /* iOS BLUE LINKS */\r\n        a[x-apple-data-detectors] {{\r\n            color: inherit !important;\r\n            text-decoration: none !important;\r\n            font-size: inherit !important;\r\n            font-family: inherit !important;\r\n            font-weight: inherit !important;\r\n            line-height: inherit !important;\r\n        }}\r\n\r\n        /* MOBILE STYLES */\r\n        @media screen and (max-width:600px) {{\r\n            h1 {{\r\n                font-size: 32px !important;\r\n                line-height: 32px !important;\r\n            }}\r\n        }}\r\n\r\n        /* ANDROID CENTER FIX */\r\n        div[style*=\"margin: 16px 0;\"] {{\r\n            margin: 0 !important;\r\n        }}\r\n    </style>\r\n</head>\r\n\r\n<body style=\"background-color: #f4f4f4; margin: 0 !important; padding: 0 !important;\">\r\n    <!-- HIDDEN PREHEADER TEXT -->\r\n    <div style=\"display: none; font-size: 1px; color: #fefefe; line-height: 1px; font-family: 'Lato', Helvetica, Arial, sans-serif; max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden;\"> We're thrilled to have you here! Get ready to dive into your new account.\r\n    </div>\r\n    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n        <!-- LOGO -->\r\n        <tr>\r\n            <td bgcolor=\"#FFA73B\" align=\"center\">\r\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n                    <tr>\r\n                        <td align=\"center\" valign=\"top\" style=\"padding: 40px 10px 40px 10px;\"> </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <td bgcolor=\"#FFA73B\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\">\r\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n                    <tr>\r\n                        <td bgcolor=\"#ffffff\" align=\"center\" valign=\"top\" style=\"padding: 40px 20px 20px 20px; border-radius: 4px 4px 0px 0px; color: #111111; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 48px; font-weight: 400; letter-spacing: 4px; line-height: 48px;\">\r\n                            <h1 style=\"font-size: 48px; font-weight: 400; margin: 2;\">Sipariş Aldık!</h1> <img src=\"https://img.icons8.com/clouds/512/shopping-cart-with-money.png\" width=\"125\" height=\"120\" style=\"display: block; border: 0px;\" />\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <td bgcolor=\"#f4f4f4\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\">\r\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n                    <tr>\r\n                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 20px 30px 40px 30px; color: #666666; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\">\r\n                            <p style=\"margin: 0;\"><div class='text-center'><strong>{model.CartModel.TotalPrice}</strong></div><br/>{model.FirstName} {model.LastName}'dan Sipariş aldık.<br/>İL:{model.City}<br/>İlçe:{model.District}<br/>Adres:{model.Address}</p>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td bgcolor=\"#ffffff\" align=\"left\">\r\n                            <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n                                <tr>\r\n                                    <td bgcolor=\"#ffffff\" align=\"center\" style=\"padding: 20px 30px 60px 30px;\">\r\n                                        <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n                                            <tr>\r\n                                                \r\n                                            </tr>\r\n                                        </table>\r\n                                    </td>\r\n                                </tr>\r\n                            </table>\r\n                        </td>\r\n                    </tr> <!-- COPY -->\r\n                    <tr>\r\n                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0px 30px 0px 30px; color: #666666; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\">\r\n                            <p style=\"margin: 0;\"></p>\r\n                        </td>\r\n                    </tr> <!-- COPY -->\r\n                    <tr>\r\n                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 20px 30px 20px 30px; color: #666666; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\">\r\n                            <p style=\"margin: 0;\"><a href=\"#\" target=\"_blank\" style=\"color: #FFA73B;\"></a></p>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0px 30px 20px 30px; color: #666666; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\">\r\n                            <p style=\"margin: 0;\"></p>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0px 30px 40px 30px; border-radius: 0px 0px 4px 4px; color: #666666; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\">\r\n                            <p style=\"margin: 0;\">:),<br>Meta Akdeniz Team</p>\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <td bgcolor=\"#f4f4f4\" align=\"center\" style=\"padding: 30px 10px 0px 10px;\">\r\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n                    <tr>\r\n                        <td bgcolor=\"#FFECD1\" align=\"center\" style=\"padding: 30px 30px 30px 30px; border-radius: 4px 4px 4px 4px; color: #666666; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\">\r\n                            <h2 style=\"font-size: 20px; font-weight: 400; color: #111111; margin: 0;\">Need more help?</h2>\r\n                            <p style=\"margin: 0;\"><a href=\"#\" target=\"_blank\" style=\"color: #FFA73B;\"></a></p>\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <td bgcolor=\"#f4f4f4\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\">\r\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n                    <tr>\r\n                        <td bgcolor=\"#f4f4f4\" align=\"left\" style=\"padding: 0px 30px 30px 30px; color: #666666; font-family: 'Lato', Helvetica, Arial, sans-serif; font-size: 14px; font-weight: 400; line-height: 18px;\"> <br>\r\n                            <p style=\"margin: 0;\"><a href=\"#\" target=\"_blank\" style=\"color: #111111; font-weight: 700;\"></a>.</p>\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n</body>\r\n\r\n</html>");
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        private void SaveOrder(OrderModel model, Payment payment, string userId)
        {
            var order = new Order();

            order.OrderNumber = new Random().Next(111111, 999999).ToString();
            order.OrderState = EnumOrderState.Completed;
            order.EnumPaymentTypes = EnumPaymentTypes.CreditCart;
            order.PaymentId = payment.PaymentId;
            order.ConversationId = payment.ConversationId;
            order.OrderTime = DateTime.Now;
            order.FistName = model.FirstName;
            order.LastName = model.LastName;
            order.Email = model.Mail;
            order.Phone = model.Phone;
            order.Address = model.Address;
            order.City = model.City;
            order.District = model.District;
            order.UserId = Convert.ToInt32(userId);

            foreach (var item in model.CartModel.CartItems)
            {
                var orderItem = new OrderItem()
                {
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId,

                };
                order.OrderItems.Add(orderItem);
            }

            _orderManager.Create(order);

        }

        private void ClearCart(string cartId)
        {
            _cartManager.ClearCart(cartId);
        }

        private Payment PaymentRrocess(OrderModel model)
        {
            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItem basketItem;

            Options options = new Options();
            options.SecretKey = "sandbox-dLiR5tLPlelW4arCS8Tx0pnIL31Dafka";
            options.ApiKey = "sandbox-Kt1U46q5ZNkac869S5Eknr17p327Jvt1";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();

            request.ConversationId = Guid.NewGuid().ToString();
            request.Price = model.CartModel.TotalPrice().ToString();
            request.PaidPrice = model.CartModel.TotalPrice().ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = model.CartModel.CartId.ToString();
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.CardName;
            paymentCard.CardNumber = model.CardNumber;
            paymentCard.ExpireMonth = model.ExpirationMonth;
            paymentCard.ExpireYear = model.ExpirationYear;
            paymentCard.Cvc = model.Cvv;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            //PaymentCard paymentCard = new PaymentCard();
            //paymentCard.CardHolderName = model.CardName;
            //paymentCard.CardNumber = "5528790000000008";
            //paymentCard.ExpireMonth = "12";
            //paymentCard.ExpireYear = "2030";
            //paymentCard.Cvc = "123";

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = model.FirstName;
            buyer.Surname = model.LastName;
            buyer.GsmNumber = model.Phone;
            buyer.Email = model.Mail;
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = model.Address;
            buyer.Ip = "85.34.78.112";
            buyer.City = model.City;
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = model.FirstName;
            shippingAddress.City = model.City;
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = model.City;
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = model.FirstName;
            billingAddress.City = model.FirstName;
            billingAddress.Country = "Turkey";
            billingAddress.Description = model.Address;
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;




            foreach (var item in model.CartModel.CartItems)
            {
                basketItem = new BasketItem();
                basketItem.Id = item.ProductId.ToString();
                basketItem.Name = item.Name;
                basketItem.Category1 = "Phone";
                basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                //Buradaki hata bulunmuş olabilir
                basketItem.Price = (item.Price * item.Quantity).ToString();
                basketItems.Add(basketItem);
            }


            request.BasketItems = basketItems;

            return Payment.Create(request, options);


        }

        public IActionResult UpdateOrder(OrderModel model)
        {
            var order = _orderManager.GetById(model.Id);

            if (order != null)
            {
                order.Condition = model.Condition;
                _orderManager.Update(order);
                return RedirectToAction("Index","Admin");
            }

            return NotFound();  
        }


        public IActionResult GetOrders()
        {
            var orders = _orderManager.GetOrders(_userManager.GetUserId(User));
            var orderListModel = new List<OrderListModel>();
            OrderListModel orderModel;
            foreach (var item in orders)
            {
                orderModel = new OrderListModel();
                orderModel.OrderId = item.Id;
                orderModel.OrderNumber = item.OrderNumber;
                orderModel.OrderTime = item.OrderTime;
                orderModel.Phone = item.Phone;
                orderModel.FistName = item.FistName;
                orderModel.LastName = item.LastName;
                orderModel.Email = item.Email;
                orderModel.Condition = item.Condition;
                orderModel.Address = item.Address;
                orderModel.City = item.City;
                orderModel.District = item.District;

                orderModel.OrderItems = item.OrderItems.Select(x => new OrderItemModel()
                {
                    OrderItemId = x.OrderId,
                    Name = x.Product.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    ImgUrl = x.Product.ImageUrl,


                }).ToList();


                orderListModel.Add(orderModel);

            }
            return View(orderListModel);
        }


        public IActionResult OrderDetails(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Order values = _orderManager.GetById((int)id);

            return View(values);
        }


        public PartialViewResult Navbar()
        {
            var cart = _cartManager.GetCartByUserId(_userManager.GetUserId(User));
            return PartialView(new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(x => new CartItemModel()
                {
                    CartItemId = x.Id,
                    ProductId = x.ProductId,
                    Name = x.Product.Name,
                    Price = x.Product.Price,
                    ImageUrl = x.Product.ImageUrl,
                    Quantity = x.Quantity

                }).ToList()
            });
        }
    }
}