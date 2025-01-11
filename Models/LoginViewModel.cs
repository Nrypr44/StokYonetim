using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Kullanıcı Adı veya E-posta")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }
    }
 
}
