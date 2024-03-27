using System;

namespace technoleight_THandy.Models
{
    public class AGFScanRecordModel
    {
        public int DepoID {  get; set; }
        public int HandyUserID { get; set; }
        public int HandyOperationClass { get; set; }
        public string HandyOperationMessage { get; set; }
        public string Device { get; set; }
        public int HandyPageID { get; set; }
        public string ScanString1 {  get; set; }
        public string ScanString2 {  get; set; }
        public string ScanString3 { get; set; }
        public DateTime ScanTime { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
