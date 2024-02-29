using System.ComponentModel.DataAnnotations;

namespace SakaguraAGFWebApi.Models
{
    public class LoginModel
    {
        public class LoginPostBody
        {
            [Required]
            public int CompanyID { get; set; }
            [Required]
            public int HandyUserID { get; set; }
            [Required]
            public string HandyUserCode { get; set; } = string.Empty;
            [Required]
            public int PasswordMode { get; set; }
            public string HandyUserPassword { get; set; } = string.Empty;
            [Required]
            public string Device { get; set; } = string.Empty;
            [Required]
            public decimal HandyAppVersion { get; set; }
        }

        public class LoginPostBackContent
        {
            public string CompanyName { get; set; } = string.Empty;
            public string HandyUserName { get; set; } = string.Empty;
            public int DepoID { get; set; }
            public string DepoCode { get; set; } = string.Empty;
            public string DepoName { get; set; } = string.Empty;
            public int AdministratorFlag { get; set; }
            public int DefaultHandyPageID { get; set; }
        }

    }
}
