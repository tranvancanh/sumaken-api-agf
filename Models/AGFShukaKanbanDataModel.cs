namespace technoleight_THandy.Models
{
    public class AGFShukaKanbanDataModel
    {
        public int DepoID { get; set; }
        public int DepoCode { get; set;}
        public int HandyUserID {  get; set; }
        public int HandyPageID { get; set; }
        public DateTime ProcessceDate { get; set; } //処理日

        public string CustomerCode {  get; set; } // 得意先コード(得意先 + 工区)
        public string TokuiSakiCode { get; set; } //得意先
        public string KoKu { get; set; } //工区
        public string Ukeire { get; set; }//受入
        public string Hinban { get; set; }//品番
        public string Bin { get; set; } //便
        public DateTime Noki { get; set; } //納期
        public int SagyoShaCode { get; set; } //トラック業者コード
        public string SagyoShaName { get; set; } //トラック業者

        public string ScanString1 { get; set;  }
        public string ScanString2 { get; set;  }
        public DateTime ScanTime { get; set; }
    }
}
