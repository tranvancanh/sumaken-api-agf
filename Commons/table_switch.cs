
public partial class table_switch
{
    public string rack_move_record;
    public string auto_rack_master;
    public string domestic_stcok_master;
    public string shootermaster;
    public string order_log_table;
    public string _9bux_master;
    public string EDL_EDD_master;
    public string log_table;
    public string XG3_master;
    public string warehouse_stock_master;
    public string ItemTypeTagMap;
    public string ItemTypeTagName;

    private Common_Class c = new Common_Class();
    public table_switch()
    {
        // 機能 　  : tableをtestと本番で切替える
        // 返り値   : 
        // 引き数　 : 
        // 作成日 　: 2021/04/06
        // 作成者   : 佐藤
        // 機能説明 :
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________
        if (c.Test_Switch() == "1")
        {


            auto_rack_master = "tokoname_autorack_loc_master";
            domestic_stcok_master = "tokoname_domestic_stock_master";
            shootermaster = "tokoname_shooter_master";
            order_log_table = "tokoname_ORDER_LOG";
            _9bux_master = "tokoname_9BUX_stock";
            EDL_EDD_master = "tokoname_EDL_EDD_stock";
            XG3_master = "tokoname_XG3_stock";
            log_table = "tokoname_rack_move_record";
            warehouse_stock_master = "tokoname_warehouse_stock";
            ItemTypeTagMap = "tokoname_tag_map";
            ItemTypeTagName = "tokoname_tag_name";
        }
        else
        {
            auto_rack_master = "tokoname_autorack_loc_master_TEST";
            domestic_stcok_master = "tokoname_domestic_stock_master_TEST";
            shootermaster = "tokoname_shooter_master_TEST";
            order_log_table = "tokoname_ORDER_LOG_TEST";
            _9bux_master = "tokoname_9BUX_stock_1";
            EDL_EDD_master = "tokoname_EDL_EDD_stock_TEST";
            XG3_master = "tokoname_XG3_stock";
            log_table = "tokoname_rack_move_rec_TEST";
            warehouse_stock_master = "tokoname_warehouse_stock_TEST";
            ItemTypeTagMap = "tokoname_tag_map";
            ItemTypeTagName = "tokoname_tag_name";
        }

    }


}