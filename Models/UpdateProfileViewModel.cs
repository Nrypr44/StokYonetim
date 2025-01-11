using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class UpdateProfileViewModel
    {
        [Required]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Telefon Numarası")]
        public string PhoneNumber { get; set; }
    }
}
