using System.ComponentModel.DataAnnotations;

namespace SakaguraAGFWebApi.Models
{
    public class ReceiveDeleteModel
    {
        public class ReceiveDeletePostBody
        {
            [Required]
            public string DeleteReceiveStartDate { get; set; } = "";
            [Required]
            public int UserID { get; set; }
        }

        public class ReceiveDeletePostBackContent
        {
            public int DeleteReceiveDataCount { get; set; }
        }

    }
}
