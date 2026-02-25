using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Interconn.Models.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "請填入Email")]
        [EmailAddress(ErrorMessage = "Email 格式錯誤")]
        [DisplayName("帳號(信箱)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        [DisplayName("密碼")]
        public string Password { get; set; }
    }
}
