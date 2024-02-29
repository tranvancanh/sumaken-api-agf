using System.ComponentModel.DataAnnotations;

namespace SakaguraAGFWebApi.Models
{
    public class SettingModel
    {
        public class SettingPostBody
        {
            [Required]
            public string CompanyCode { get; set; } = string.Empty;
            [Required]
            public string CompanyPassword { get; set; } = string.Empty;
            [Required]
            public string HandyUserCode { get; set; } = string.Empty;
            [Required]
            public string Device { get; set; } = string.Empty;
            [Required]
            public string DeviceName { get; set; } = string.Empty;
        }

        public class SettingPostBackContent
        {
            public int CompanyID { get; set; }
            public int HandyUserID { get; set; }
            public int PasswordMode { get; set; }
        }

      }
}
