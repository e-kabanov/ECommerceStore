using System.ComponentModel.DataAnnotations;

namespace ECommerceStore.DTOs
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Название товара обязательно")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Название должно быть от 3 до 200 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, 999999.99, ErrorMessage = "Цена должна быть от 0.01 до 999999.99")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Количество на складе обязательно")]
        [Range(0, int.MaxValue, ErrorMessage = "Количество не может быть отрицательным")]
        [Display(Name = "Количество на складе")]
        public int Stock { get; set; }

        [Display(Name = "Фото")]
        public string? Photo { get; set; }

        [Display(Name = "Файл изображения")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreatedAt { get; set; }
    }
}
