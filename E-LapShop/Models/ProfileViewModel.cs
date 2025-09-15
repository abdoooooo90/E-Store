using System.ComponentModel.DataAnnotations;

namespace E_LapShop.Models
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        [Display(Name = "الاسم الكامل")]
        public string? FullName { get; set; }

        [EmailAddress]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "رقم الموبايل")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "العنوان")]
        public string? Address { get; set; }
    }
}
