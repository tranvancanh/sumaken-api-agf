namespace SakaguraAGFWebApi.Models
{
    public class StoreInModel
    {
        public class D_StoreIn
        {
            public long StoreInID { get; set; }
            public long ScanRecordID { get; set; }
            public long ReceiveID { get; set; }
            public int DepoID { get; set; } 
            public DateTime StoreInDate { get; set; }
            public string ProductCode { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public string Packing { get; set; } = string.Empty;
            public int PackingCount { get; set; }
            public string StockLocation1 { get; set; } = string.Empty;
            public string StockLocation2 { get; set; } = string.Empty;
            public string Remark { get; set; } = string.Empty;
            public DateTime CreateDate { get; set; }
            public int CreateUserID { get; set; }
        }

    }

}
