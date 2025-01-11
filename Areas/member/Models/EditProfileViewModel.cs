namespace InventoryManagement.Areas.member.Models
{
    public class EditProfileViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string CurrentPassword { get; set; }

    }
}
