using ShopApp.Entites;
using System.ComponentModel.DataAnnotations;


namespace ShopApp.WebUI.Models
{
    public class AddProductModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(60, MinimumLength = 6, ErrorMessage = "Ürün ismi boş bırakılamaz")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Ürün açıklaması boş bırakılamaz")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Ürün fotografı boş bırakılamaz")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Fiyat alanı boş bırakılamaz")]
        public int Price { get; set; }

        public string? Gender { get; set; }

        [Required(ErrorMessage = "Ürün durumu boş bırkılamaz")]
        public string? Condition { get; set; }



        public List<Category> Categories { get; set; }
        public List<Category> SelectedCategories { get; set; }
    }
}
