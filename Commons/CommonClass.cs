
    public class Common_Class
    {
        public string Test_Switch()
        {
            // 機能 　  : 
            // 返り値   : なし
            // 引き数　 : なし
            // 作成日 　: 2019/9/17
            // 作成者   : 佐藤
            // 機能説明 : 本番環境・テスト環境切り替え
            // 注意事項 : 0がテスト、１が本番
            // 
            // ___________________________________________________________________________________
            string _test;
            _test = "1";
            return _test;
        }

        public string log_table_switch()
        {
            // 機能 　  : 
            // 返り値   : なし
            // 引き数　 : なし
            // 作成日 　: 2019/9/17
            // 作成者   : 佐藤
            // 機能説明 : 本番環境・テスト環境のログテーブル切り替え
            // 注意事項 : 0がテスト、１が本番
            // 
            // ___________________________________________________________________________________
            string log_tbl;
            if (Test_Switch() == "0")
            {
                log_tbl = "tokoname_rack_move_rec_TEST";
            }

            else
            {
                log_tbl = "tokoname_rack_move_record";
            }

            return log_tbl;
        }

        public string auto_rack_switch()
        {
            // 機能 　  : 
            // 返り値   : なし
            // 引き数　 : なし
            // 作成日 　: 2019/9/17
            // 作成者   : 佐藤
            // 機能説明 : 本番環境・テスト環境のログテーブル切り替え
            // 注意事項 : 0がテスト、１が本番
            // 
            // ___________________________________________________________________________________
            string auto_rack_tbl;
            if (Test_Switch() == "0")
            {
                auto_rack_tbl = "tokoname_autorack_loc_master_TEST";
            }

            else
            {
                auto_rack_tbl = "tokoname_autorack_loc_master";
            }

            return auto_rack_tbl;
        }

        public string domestic_rack_switch()
        {
            // 機能 　  : 
            // 返り値   : なし
            // 引き数　 : なし
            // 作成日 　: 2019/9/17
            // 作成者   : 佐藤
            // 機能説明 : 本番環境・テスト環境のログテーブル切り替え
            // 注意事項 : 0がテスト、１が本番
            // 
            // ___________________________________________________________________________________
            string auto_rack_tbl;
            if (Test_Switch() == "0")
            {
                auto_rack_tbl = "tokoname_domestic_stock_master_TEST";
            }

            else
            {
                auto_rack_tbl = "tokoname_domestic_stock_master";
            }

            return auto_rack_tbl;
        }

        public string shooter_master_switch()
        {
            // 機能 　  : 
            // 返り値   : なし
            // 引き数　 : なし
            // 作成日 　: 2019/9/17
            // 作成者   : 佐藤
            // 機能説明 : 本番環境・テスト環境のログテーブル切り替え
            // 注意事項 : 0がテスト、１が本番
            // 
            // ___________________________________________________________________________________
            string shooter_master;
            if (Test_Switch() == "0")
            {
                shooter_master = "tokoname_shooter_master_TEST";
            }

            else
            {
                shooter_master = "tokoname_shooter_master";
            }

            return shooter_master;
        }

        public string order_switch()
        {
            // 機能 　  : 
            // 返り値   : なし
            // 引き数　 : なし
            // 作成日 　: 2019/9/17
            // 作成者   : 佐藤
            // 機能説明 : 本番環境・テスト環境のログテーブル切り替え
            // 注意事項 : 0がテスト、１が本番
            // 
            // ___________________________________________________________________________________
            string order_table;
            if (Test_Switch() == "0")
            {
                order_table = "tokoname_ORDER_LOG_TEST";
            }

            else
            {
                order_table = "tokoname_ORDER_LOG";
            }

            return order_table;
        }


        public string share_folder_path_switch()
        {
            // 機能 　  : 
            // 返り値   : なし
            // 引き数　 : なし
            // 作成日 　: 2019/9/17
            // 作成者   : 佐藤
            // 機能説明 : 本番環境・テスト環境の共有フォルダ切り替え
            // 注意事項 : 0がテスト、１が本番        
            // ___________________________________________________________________________________
            string share_folder;
            if (Test_Switch() == "0")
            {
                share_folder = "身内環境";
            }

            else
            {
                share_folder = "外部環境";
            }

            return share_folder;
        }
    }

