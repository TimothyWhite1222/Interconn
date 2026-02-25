using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Interconn.Models.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "請輸入姓名")]
        [DisplayName("姓名")]
        public string Name { get; set; }

        [Required(ErrorMessage = "請輸入Email")]
        [EmailAddress(ErrorMessage = "Email 格式錯誤")]
        [DisplayName("信箱")]
        public string Email { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        [DisplayName("密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "請再次輸入密碼")]
        [DisplayName("確認密碼")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "密碼與確認密碼不一致")]
        public string ConfirmPassword { get; set; }
    }
}

