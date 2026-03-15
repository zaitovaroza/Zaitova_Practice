using System.ComponentModel.DataAnnotations;

namespace ZaitovaLibrary.DTO
{
    public class PartnerCreateUpdateDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Тип партнера обязателен")]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "Наименование обязательно")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Наименование должно быть от 2 до 255 символов")]
        public string CompanyName { get; set; } = string.Empty;

        public string? LegalAddress { get; set; }

        [RegularExpression(@"^\d{10,12}$", ErrorMessage = "ИНН должен содержать 10-12 цифр")]
        public string? Inn { get; set; }

        public string? DirectorFullname { get; set; }

        [RegularExpression(@"^\+?[\d\s\-\(\)]{10,20}$", ErrorMessage = "Введите корректный номер телефона")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Введите корректный email")]
        public string? Email { get; set; }

        [Range(0, 100, ErrorMessage = "Рейтинг должен быть от 0 до 100")]
        public int Rating { get; set; }
    }
}