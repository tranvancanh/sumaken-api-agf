

public partial class AGF_order_dat
{

    public partial class ORDER
    {
        // ■WMSからDCCへの搬送指示送信用データクラス

        // ■更新日時
        public string update_datetime;

        // ■データ更新日
        public string update_date;

        // ■データID
        public string date_ID;

        // ■上位キー
        public string superior_key;

        // ■関連上位キー
        public string related_sp_key;

        // ■指示区分
        public string order_type;

        // ■指示詳細
        public string order_detail;

        // ■荷取ST
        public string catch_ST;

        // ■荷取高さ
        public string catch_height;

        // ■荷降ST
        public string release_ST;

        // ■荷降高さ
        public string release_height;

        // ■優先順位
        public string priority_order;
        // ■号機
        public string machine_No;

        // 東山仕様
        public string Part_num;
        public string Quantity;
        public string LabelID;


        public string[] make_arry()
        {

            // ■設定したプロパティを1次元配列に格納


            string[] property_arry = new string[] { update_datetime, update_date, date_ID, superior_key, related_sp_key, order_type, order_detail, catch_ST, catch_height, release_ST, release_height, priority_order, machine_No };

            return property_arry;
        }

    }
    public partial class DCC_STATE
    {

        // ■更新日時
        public string update_datetime;

        // ■データ更新日
        public string update_date;

        // ■データID
        public string date_ID;

        // ■電源ON/OFF
        public string power;

        // ■運転状態
        public string drive;

        // ■オンライン状態

        public string online;

        // ■接続状態
        public string connection_SM;

        // ■エラーコード
        public string error_code;

        public string[] make_arry()
        {

            // ■設定したプロパティを1次元配列に格納

            string[] property_arry = new string[] { update_datetime, update_date, date_ID, power, drive, online, connection_SM, error_code };


            return property_arry;
        }

    }

    public partial class ORDER_STATE
    {


        // ■更新日時
        public string update_datetime;

        // ■データ更新日
        public string update_date;

        // ■データID
        public string date_ID;

        // ■上位キー
        public string superior_key;


        // ■搬送支持キー
        public string order_DCC;

        // ■指示応答状態
        public string reaction_to_order;


        // ■エラーコード
        public string error_code;

        // ■進捗詳細
        public string progress_detail;

        // ■指令インデックス
        public string order_index;

        // ■号機番号
        public string machine_No;

        public string[] make_arry()
        {

            // ■設定したプロパティを1次元配列に格納

            string[] property_arry = new string[] { update_datetime, update_date, date_ID, superior_key, order_DCC, reaction_to_order, error_code, progress_detail, order_index, machine_No };


            return property_arry;
        }

    }






}
