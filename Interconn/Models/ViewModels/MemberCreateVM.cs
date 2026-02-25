using System.ComponentModel.DataAnnotations;

namespace Interconn.Models.ViewModels
{
    public enum GenderType
    {
        Male,
        Female,
        Other
    }
    public class MemberCreateVM
    {       
        [Required(ErrorMessage = "請輸入名字")]
        [StringLength(50, ErrorMessage = "名字不能超過 50 個字元")]
        public string Name { get; set; }

        [Required(ErrorMessage = "請輸入性別")]
        public GenderType Gender { get; set; }

        [Required(ErrorMessage = "請輸入生日")]
        [DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }

        [StringLength(100, ErrorMessage = "內容不能超過 100 個字元")]
        public string Bio { get; set; }

        [MaxFileSize(15 * 1024 * 1024)] 
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png",".heic",".gif" })]
        public IEnumerable<IFormFile> Files { get; set; }
    }
}
