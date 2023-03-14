using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Concrete;
using ShopApp.DataAccess.Concrete.EfCore;
using ShopApp.Entites;
using ShopApp.WebUI.Extensions;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        CartManager _cartManager = new CartManager(new EfCoreCartDal());

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost]

        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser()
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    FullName = model.FullName,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //generate token
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new
                    {
                        userId = user.Id,
                        Token = code

                    });

                    //send email

                    await _emailSender.SendEmailAsync(model.Email, "Hesabı Onayla", $"Lütefen Mail Emial Hesabınızı onaylamak için linke <a href='https://localhost:44385{callbackUrl}'>tıklayınız</a> ");


                    TempData.Put("Message", new ResultMessage()
                    {
                        Title = "Hesap Onayı",
                        Message = "E posta adresinize gelen linkle hesabinizi onaylaylayiniz",
                        Css = "warning"
                    });

                    return RedirectToAction("Login", "Account");
                }



                TempData.Put("Message", new ResultMessage()
                {
                    Title = "Hata",
                    Message = "Mail yada şifre yanlış bilgiri doğru girdiğinizden emini olunuz",
                    Css = "error"
                });
                return View(model);

            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

           

            if (user == null)
            {
                ModelState.AddModelError("", "Bu Email adresiyle daha önce bir hesap oluşturulmamış");
                TempData.Put("Message", new ResultMessage()
                {
                    Title = "Hata",
                    Message = "Bu mail adresiyle daha önce bir hesap oluşturulmuş",
                    Css = "warning"
                });
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                TempData.Put("Message", new ResultMessage()
                {
                    Title = "Uyarı!",
                    Message = "Lütfen hesabınızı onaylayınız",
                    Css = "warning"
                });
                return View(model);
            }


            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (result.Succeeded)
            {
                TempData.Put("Message", new ResultMessage()
                {
                    Title = "Başarılı",
                    Message = "Giriş işlemi başarılı",
                    Css = "success"
                });
                return RedirectToAction("Index", "Home");
            }

            TempData.Put("Message", new ResultMessage()
            {
                Title = "Hata",
                Message = "Kullanıcı adı yada parola yanlış",
                Css = "success"
            });
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            TempData.Put("Message", new ResultMessage()
            {
                Title = "Oturum Kapatildi",
                Message = "Hesabiniz günvenli bir şekilde kapatıldı. ",
                Css = "warning"
            });

            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData.Put("Message", new ResultMessage()
                {
                    Title = "Hesap Onayı",
                    Message = "Hesap onayi için bilgileriniz yanlış",
                    Css = "error"
                });
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    //create cart object 

                    _cartManager.InitializeCart(Convert.ToString(user.Id));

                    TempData.Put("Message", new ResultMessage()
                    {
                        Title = "Hesap Onayı",
                        Message = "Hesabiniz başarıyla onayladı",
                        Css = "success"
                    });

                    return RedirectToAction("Login", "Account");
                }

            }

            TempData.Put("Message", new ResultMessage()
            {
                Title = "Hesap Onayı",
                Message = "Hesabiniz onaylanamadı",
                Css = "error"
            });
            return View();
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                TempData.Put("Message", new ResultMessage()
                {
                    Title = "Şifreyi ununttum",
                    Message = "Bilgilerniz hatali",
                    Css = "error"
                });
                return View();
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                TempData.Put("Message", new ResultMessage()
                {
                    Title = "Şifreyi unuttum",
                    Message = "Epostat adresi ile bir Kullanici bulunamadi ",
                    Css = "error"
                });
                return View();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action("ResetPassword", "Account", new
            {

                Token = code

            });

            //send email

            await _emailSender.SendEmailAsync(Email, "Parolayı yenile", $"Parolayi yenilemek için linke tıklayınız <a href='https://localhost:44385{callbackUrl}'>tıklayınız</a> ");
            TempData.Put("Message", new ResultMessage()
            {
                Title = "Şifreyi Unuttum",
                Message = "Parola yenilemek için hesabiniza Email gönderildi",
                Css = "warning"
            });
            return RedirectToAction("Login", "Account");


        }

        public IActionResult ResetPassword(string token)
        {
            if (token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ResetPasswordModel { Token = token };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                return View(model);

            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Home");
            }
            return View(model);
        }
    }
}
