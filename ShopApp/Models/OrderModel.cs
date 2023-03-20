using System.ComponentModel.DataAnnotations;

namespace ShopApp.WebUI.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public String? Condition { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        [Display(Name = "Şehir")]
        public String? City { get; set; }
        public String? District { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationMonth { get; set; }
        public string ExpirationYear { get; set; }
        public string Cvv { get; set; }

        public CartModel? CartModel { get; set; }



    }
}
