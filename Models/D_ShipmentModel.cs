namespace SakaguraAGFWebApi.Models
{
    public class D_ShipmentModel
    {
        public long ShipmentID { get; set; }
        public long ScanRecordID { get; set; }
        public long ShipmentInstructionDetailID { get; set; }
        public long StoreOutID { get; set; }
        public string HandyMatchClass { get; set; }
        public string HandyMatchResult { get; set; }
        public DateTime ShipmentDate { get; set; }
        public int DepoID { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DeliveryTimeClass { get; set; }
        public string DeliverySlipNumber { get; set; }
        public int DeliverySlipRowNumber { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierClass { get; set; }
        public string ProductCode { get; set; }
        public string ProductAbbreviation { get; set; }
        public string ProductManagementClass { get; set; }
        public int ProductLabelBranchNumber { get; set; }
        public string NextProcess1 { get; set; }
        public string Location1 { get; set; }
        public string NextProcess2 { get; set; }
        public string Location2 { get; set; }
        public DateTime CustomerDeliveryDate { get; set; }
        public string CustomerDeliveryTimeClass { get; set; }
        public string CustomerDeliverySlipNumber { get; set; }
        public int CustomerDeliverySlipRowNumber { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerClass { get; set; }
        public string CustomerName { get; set; }
        public string CustomerProductCode { get; set; }
        public string CustomerProductAbbreviation { get; set; }
        public string CustomerProductManagementClass { get; set; }
        public int CustomerProductLabelBranchNumber { get; set; }
        public string CustomerNextProcess1 { get; set; }
        public string CustomerLocation1 { get; set; }
        public string CustomerNextProcess2 { get; set; }
        public string CustomerLocation2 { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string CustomerOrderClass { get; set; }
        public int LotQuantity { get; set; }
        public int FractionQuantity { get; set; }
        public int Quantity { get; set; }
        public string Packing { get; set; }
        public int PackingCount { get; set; }
        public string LotNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool DeleteFlag { get; set; }
        public long DeleteShipmentID { get; set; }
        public string Remark { get; set; }
    }
}
