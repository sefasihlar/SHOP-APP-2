using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace ShopApp.WebUI.Models
{
    public class RegisterModel
    {
        public string? SellerNumber { get; set; }

        [Required(ErrorMessage = "Ad ve soyad alanı boş bırakılamaz.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Kullanıcı adı yalnızca harf, rakam ve alt çizgi içerebilir.Boşluk içermemeli")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Parola alanı boş bırakılamaz.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Parola ve tekrar parola alanları aynı olmalıdır.")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Tekrar parola alanı boş bırakılamaz.")]
        public string RePassword { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz.")]
        [Required(ErrorMessage = "E-posta alanı boş bırakılamaz.")]
        public string Email { get; set; }

        [RegularExpression(@"^(0|\+9)[0-9]{10}$", ErrorMessage = "Lütfen geçerli bir telefon numarası giriniz.")]
        public string Phone { get; set; }
    }
}
