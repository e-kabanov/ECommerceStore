using System.ComponentModel.DataAnnotations;

namespace ECommerceStore.DTOs
{
    public class CheckoutModel
    {
        [Required(ErrorMessage = "Имя обязательно")]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Номер телефона обязателен")]
        [Phone(ErrorMessage = "Неверный формат телефона")]
        [Display(Name = "Номер телефона")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Адрес обязателен")]
        [Display(Name = "Адрес доставки")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Выберите способ оплаты")]
        [Display(Name = "Способ оплаты")]
        public string PaymentMethod { get; set; }
    }
}
