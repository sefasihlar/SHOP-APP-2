using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace ShopApp.WebUI.Models
{
    public class RegisterModel
    {
        public string? SellerNumber { get; set; }

        [Required(ErrorMessage = "Ad ve soyad alanı boş bırakılamaz.")]
        public string FullName { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_ğüşıöçĞÜŞİÖÇ]+$",
         ErrorMessage = "Kullanıcı adı yalnızca harf, rakam içerebilir.Boşluk içermemeli.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Parola alanı boş bırakılamaz.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d.@$!%*?&_-ğüşıöçĞÜŞİÖÇ]{6,}$",
          ErrorMessage = "Şifre en az bir büyük harf, bir rakam ve en az 6 karakter uzunluğunda olmalıdır.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Parola ve tekrar parola alanları aynı olmalıdır.")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Tekrar parola alanı boş bırakılamaz.")]
        public string RePassword { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz.")]
        [Required(ErrorMessage = "E-posta alanı boş bırakılamaz.")]
        public string Email { get; set; }

        [RegularExpression(@"^(05\d{9})$", ErrorMessage = "Lütfen geçerli bir  telefon numarası giriniz.")]
        [Required(ErrorMessage = "Telefon numarası alanı boş bırakılamaz.")]
        public string Phone { get; set; }
    }
}
