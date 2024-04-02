using Dapper;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AGF_operater
{

    public partial class AgfOpreate
    {
        // 機能　 　　: AGF操作にかかわる関数をまとめたくらす
        // 返り値 　　:
        // 引き数 　　: 
        // 作成日 　　: 2019/10/29
        // 作成者 　　: y.sato
        // 機能説明　 : 
        // 注意事項　 : 
        // ______________________________________________________________

        private Common_Class c = new Common_Class();
        private table_switch tables = new table_switch();
        private string auto_rack;
        private string log_table;
        private string order_table;

        public AgfOpreate()
        {
            auto_rack = tables.auto_rack_master;
            log_table = tables.log_table;
            order_table = tables.order_log_table;
        }



        // Public Function DCC_State_Check() As AGF_order_dat.DCC_STATE
        // '機能　 　　: DCCの状態を取得する。専用クラスの各プロパティに放り込む
        // '返り値 　　:
        // '引き数 　　: 
        // '作成日 　　: 2019/10/29
        // '作成者 　　: y.sato
        // '機能説明　 : 
        // '注意事項　 : 
        // '______________________________________________________________


        // '■DCC状態の確認
        // Dim file_No_dcc As Integer = 1

        // Dim DCC_STATE_Path As String = folder.DCC_STATE
        // Dim Share_foleder As String = folder.Share_Foleder
        // '■リネーム用
        // Dim Read_DCC_STATE_Path As String = Share_foleder & "\DCC_STATE_1.csv"
        // Dim dcc_state_dat As New AGF_order_dat.DCC_STATE
        // '■名前の変更
        // Try

        // '■指定ファイルが見つからなかった場合一定回数トライしてだめだったらエラー
        // Dim file_exist As Boolean = False
        // Dim vali_coun As Integer = 0
        // Const try_count As Integer = 6

        // Do While file_exist = False And vali_coun < try_count
        // file_exist = System.IO.File.Exists(DCC_STATE_Path)
        // vali_coun += 1
        // If vali_coun >= try_count Then
        // Dim ex As New System.IO.FileNotFoundException
        // Throw ex
        // End If
        // Loop
        // System.IO.File.Move(DCC_STATE_Path, Read_DCC_STATE_Path)
        // FileOpen(file_No_dcc, Read_DCC_STATE_Path, OpenMode.Input, OpenAccess.Default, OpenShare.LockRead)

        // '■仕様不明だが最後行をとることにする。追加モードで記述されるなら間違いではないはず

        // Do Until EOF(file_No_dcc)
        // Input(file_No_dcc, dcc_state_dat.update_datetime)
        // Input(file_No_dcc, dcc_state_dat.update_date)
        // Input(file_No_dcc, dcc_state_dat.date_ID)
        // Input(file_No_dcc, dcc_state_dat.power)
        // Input(file_No_dcc, dcc_state_dat.drive)
        // Input(file_No_dcc, dcc_state_dat.online)
        // Input(file_No_dcc, dcc_state_dat.connection_SM)
        // Input(file_No_dcc, dcc_state_dat.error_code)


        // Loop
        // Catch ex As System.IO.FileNotFoundException
        // Return Nothing
        // Catch
        // Return Nothing
        // Finally
        // FileClose(file_No_dcc)
        // '■読み取ったらファイル削除
        // System.IO.File.Delete(Read_DCC_STATE_Path)
        // End Try


        // Return dcc_state_dat
        // End Function

        // Public Function ORDER_ORDER_STATE_Check() As AGF_order_dat.ORDER_STATE
        // '機能　 　　: ORDER_STATEの状態を取得して専用クラスの各プロパティに放り込む
        // '返り値 　　:
        // '引き数 　　: 
        // '作成日 　　: 2019/10/29
        // '作成者 　　: y.sato
        // '機能説明　 : 
        // '注意事項　 : 
        // '______________________________________________________________

        // '■ORDER_STATE状態の確認
        // Dim file_No_dcc As Integer = 1
        // Dim ORDER_STATE_Path As String = folder.ORDER_STATE
        // Dim Share_foleder As String = folder.Share_Foleder
        // '■リネーム用
        // Dim Read_ORDER_STATE_Path As String = Share_foleder & "\ORDER_STATE_1.csv"
        // Dim order_state_dat As New AGF_order_dat.ORDER_STATE
        // '■名前の変更
        // Try

        // '■指定ファイルが見つからなかった場合一定回数トライしてだめだったらエラー
        // Dim file_exist As Boolean = False
        // Dim vali_coun As Integer = 0
        // Const try_count As Integer = 6

        // Do While file_exist = False And vali_coun < try_count
        // file_exist = System.IO.File.Exists(ORDER_STATE_Path)
        // vali_coun += 1
        // If vali_coun >= try_count Then
        // Dim ex As New System.IO.FileNotFoundException
        // Throw ex
        // End If
        // Loop
        // System.IO.File.Move(ORDER_STATE_Path, Read_ORDER_STATE_Path)
        // FileOpen(file_No_dcc, Read_ORDER_STATE_Path, OpenMode.Input, OpenAccess.Default, OpenShare.LockRead)

        // '■ORDER_STATE.csvの各レコードに対して処理後、最後行を現状として出力する。
        // Do Until EOF(file_No_dcc)
        // Input(file_No_dcc, order_state_dat.update_datetime)
        // Input(file_No_dcc, order_state_dat.update_date)
        // Input(file_No_dcc, order_state_dat.date_ID)
        // Input(file_No_dcc, order_state_dat.superior_key)
        // Input(file_No_dcc, order_state_dat.order_DCC)
        // Input(file_No_dcc, order_state_dat.reaction_to_order)
        // Input(file_No_dcc, order_state_dat.error_code)
        // Input(file_No_dcc, order_state_dat.progress_detail)
        // Input(file_No_dcc, order_state_dat.order_index)
        // Input(file_No_dcc, order_state_dat.machine_No)


        // '■レコードごとDB更新の必要があるかチェック
        // Dim error_code As String = order_state_dat.error_code

        // If error_code = "0" Then
        // Call Reflect_ORDER_STATE_to_DB(order_state_dat)
        // Else


        // '■エラーあり
        // Select Case error_code
        // Case "1001"
        // Case "1002"
        // Case "1003"
        // Case "1004"
        // Case "1005"
        // Case "1006"
        // Case "1007"
        // Case "1008"
        // '■指示のエラー

        // Case "1011"
        // Case "1012"
        // Case "1013"
        // Case "1014"
        // '■DCCのエラー

        // Case "1020"
        // Case "1021"
        // '■荷のエラー(二重格納、空出庫）



        // End Select
        // End If

        // Loop
        // Catch ex As System.IO.FileNotFoundException
        // Return Nothing
        // Catch ex As System.Exception
        // Console.WriteLine(ex.Message)

        // Return Nothing
        // Finally
        // FileClose(file_No_dcc)
        // '■読み取ったらファイル削除
        // System.IO.File.Delete(Read_ORDER_STATE_Path)
        // End Try


        // Return order_state_dat
        // End Function

        // Private Function ORDER_STATE_CHECK_upSQL(rack_type As String, tori_ As String, progress As String) As String
        // '機能　 　　: ORDER_ORDER_STATE_Check 中での共通SQL作成処理
        // '返り値 　　:
        // '引き数 　　:ラックの種類と取合棚（入・出）か自動棚か、  "catch" or "release" 　を指定
        // '作成日 　　: 2019/10/29
        // '作成者 　　: y.sato
        // '機能説明　 : 
        // '注意事項　 : 
        // '______________________________________________________________

        // Dim strSQL_update As String = ""
        // Dim from_to As String
        // Dim in_out As String
        // Dim labelID As String = ""
        // If progress = "catch" Then
        // from_to = "@from"
        // in_out = "出庫"
        // labelID = "''"
        // ElseIf progress = "release" Then
        // from_to = "@to"
        // in_out = "入庫"
        // labelID = "@labelID"
        // strSQL_update = "UPDATE tokoname_ORDER_LOG SET done = 1 WHERE superior_key = @superior_key;"
        // Else
        // Return Nothing
        // End If
        // strSQL_update &= "UPDATE " & auto_rack & " SET labelID = " & labelID & " , reserved = '0' WHERE rackID = " & from_to & "  AND in_or_out = " & tori_ & " ;" &
        // "INSERT INTO  " & log_table & " ( rack_type,move_type,labelID, part_num,quantity," &
        // "rackID,read_time,cuser) " &
        // "VALUES ('" & rack_type & "','" & in_out & "',@labelID,@part_num,@quantity," & from_to & " ,@datetime,@machine_No) ;"


        // Return strSQL_update
        // End Function

        // Private Sub Reflect_ORDER_STATE_to_DB(order_state_dat As AGF_order_dat.ORDER_STATE)
        // '機能　 　　: ORDER_STATEのレコードに対してDB処理をかけるか判断
        // '返り値 　　:
        // '引き数 　　: AGF_order_dat.ORDER_STATE つまりレコード
        // '作成日 　　: 2019/10/29
        // '作成者 　　: y.sato
        // '機能説明　 : 
        // '注意事項　 : 
        // '______________________________________________________________

        // '■取得したレコードごとに処理をかける
        // '■常時監視が想定されているため本来は最後のみ読み取ればよいが
        // '万が一、読みこぼしがあった場合を想定して全行に処理を行う。            
        // '検証


        // Dim reaction_to_order As String = order_state_dat.reaction_to_order
        // Dim detail_progress As String = order_state_dat.progress_detail

        // Select Case detail_progress
        // Case "1003"
        // Case "1008"

        // Using SQLTZN As New MSSQLAccess
        // SQLTZN.ClearParam()
        // SQLTZN.SetParam("@superior_key", SqlDbType.NVarChar, order_state_dat.superior_key)
        // Dim selSQL As String

        // Dim dt As DataTable
        // selSQL = " SELECT  CASE WHEN C.number = C.catch_ST then 'FROM' WHEN C.number = C.release_ST THEN 'TO' ELSE 'ERROR' END " &
        // "  AS from_or_to  , c.* FROM (SELECT rackID,in_or_out,number,reserved,catch_ST,release_ST,B.part_num,B.labelID ,B.quantity " &
        // " FROM " & auto_rack & " AS A " &
        // " INNER JOIN   " &
        // "  (SELECT top 1 catch_ST ,release_ST ,part_num,labelID,quantity " &
        // "   FROM [tozandb].[dbo].[tokoname_ORDER_LOG] " &
        // "   WHERE superior_key = @superior_key and done <> 1 " &
        // "   ORDER BY cntID DESC ) AS B  " &
        // "   ON A.number = B.catch_ST or B.release_ST = A.number ) AS C "

        // dt = SQLTZN.SelectData(selSQL)
        // Dim dr1 As DataRow()
        // dr1 = dt.Select("from_or_to = 'FROM' ")
        // Dim dr2 As DataRow()
        // dr2 = dt.Select("from_or_to = 'TO' ")

        // Dim loca_FROM_tori As String
        // Dim loca_TO_tori As String
        // If dt.Rows.Count > 0 Then
        // loca_FROM_tori = dr1(0).Item("in_or_out")
        // loca_TO_tori = dr2(0).Item("in_or_out")
        // Else

        // '■ORDERログに指示がない　エラー

        // End If

        // SQLTZN.SetParam("@from", SqlDbType.NVarChar, dr1(0).Item("rackID"))
        // SQLTZN.SetParam("@to", SqlDbType.NVarChar, dr2(0).Item("rackID"))
        // SQLTZN.SetParam("@part_num", SqlDbType.NVarChar, dr1(0).Item("part_num"))
        // SQLTZN.SetParam("@labelID", SqlDbType.NVarChar, dr1(0).Item("labelID"))
        // SQLTZN.SetParam("@quantity", SqlDbType.NVarChar, dr1(0).Item("quantity"))
        // SQLTZN.SetParam("@datetime", SqlDbType.DateTime, Now())
        // SQLTZN.SetParam("@machine_No", SqlDbType.NVarChar, "AGF" & order_state_dat.machine_No)


        // Dim tori_in As String = "1" '取合棚入庫専用
        // Dim tori_out As String = "2" '取合棚出庫専用
        // Dim not_tori As String = "0" '自動棚
        // Dim strSQL_update As String = ""
        // Select Case detail_progress
        // 'エラーでなければログの上位キーから情報を得る
        // '得た情報を使ってDBに更新をかける。


        // Case "1003" '荷降走行中
        // '■移動先と移動元が自動棚か取合い棚（入庫用出庫用）か判断
        // '■干渉する棚の種類によって処理を変える。
        // '■マスターを更新　IDの場所入れ替え + ログを更新
        // '■取合棚もしくは自動棚から出庫
        // '■荷降し走行中　= 荷物をとり終わっているので場所をデータ上開けるために更新
        // If loca_FROM_tori = tori_in And loca_TO_tori = not_tori Then
        // '■取合棚から出庫
        // strSQL_update = ORDER_STATE_CHECK_upSQL("取合棚", tori_in, "catch")
        // SQLTZN.UpdateData(strSQL_update)

        // ElseIf loca_FROM_tori = not_tori And loca_TO_tori = tori_out Then
        // '■自動棚から出庫
        // strSQL_update = ORDER_STATE_CHECK_upSQL("自動棚", not_tori, "catch")
        // SQLTZN.UpdateData(strSQL_update)

        // Else
        // '■取合棚→取合棚、自動棚→自動棚　？？？                                    


        // End If

        // Case "1008" '搬送完了

        // '■移動先と移動元が自動棚か取合い棚（入庫用出庫用）か判断
        // '■干渉する棚の種類によって処理を変える。
        // '■マスターを更新　IDの場所入れ替え + 予約フラグ解除 + ログを更新
        // '■自動棚もしくは取合棚に入庫

        // If loca_FROM_tori = tori_in And loca_TO_tori = not_tori Then
        // '■自動棚へ入庫
        // strSQL_update = ORDER_STATE_CHECK_upSQL("自動棚", not_tori, "release")


        // SQLTZN.UpdateData(strSQL_update)
        // ElseIf loca_FROM_tori = not_tori And loca_TO_tori = tori_out Then
        // '■取合い棚へ入庫
        // strSQL_update = ORDER_STATE_CHECK_upSQL("取合棚", tori_out, "release")
        // SQLTZN.UpdateData(strSQL_update)
        // Else
        // '■取合棚→取合棚、自動棚→自動棚　？？？                                    


        // End If
        // End Select
        // End Using

        // Case Else
        // End Select



        // If reaction_to_order = ("0" Or "1" Or "8") Then
        // '検証スルー　→　処理を続ける。

        // Else

        // '検証失敗　→　エラーを報告して、処理を止める。
        // Select Case reaction_to_order
        // Case "9"

        // Case "11"
        // End Select
        // End If




        // End Sub


        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="priority"></param>
        /// <param name="machine_No"></param>
        public void make_ORDER(AGF_order_dat.ORDER order, string agf_shared_folder, SqlConnection sqlConnection, SqlTransaction sqlTransaction, string priority = "0", string machine_No = "0")
        {
            // 機能　 　　: ORDER.csvを作成
            // 返り値 　　:
            // 引き数 　　:指示区分: order_type , 出発地 : _FROM([change_luggage_station]) , 到達地: _TO([change_address]) ,優先度: "0 ～7" ,号機: "1～?"
            // 作成日 　　: 2019/10/29
            // 作成者 　　: y.sato
            // 機能説明　 : 
            // 注意事項　 : 
            // ______________________________________________________________


            // ■値を入れる
            // 更新日時
            order.update_datetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            // データ更新日
            order.update_date = DateTime.Now.ToString("yyyyMMdd");
            // データID (その日でユニーク）
            order.date_ID = Unique_data_ID();
            // ■その日でユニークな値を設定する処理

            // 上位キー    指定があれば、指定通りにする     
            string superior_key;
            if (order.superior_key != "")
            {
                superior_key = order.superior_key;
            }
            else
            {
                // ■ユニークな値を設定する処理
                superior_key = Get_Unique_superior_key(sqlConnection, sqlTransaction);
            }
            order.superior_key = superior_key;

            // 関連上位キー
            string releated_sp_key = "";
            order.related_sp_key = releated_sp_key;
            // 指示区分
            //order.order_type = order.order_type;
            // 指示詳細
            string order_detail = "";
            order.order_detail = order_detail;
            // 荷取りST
            //order.catch_ST = order.catch_ST;
            // ■対象に荷物が入っているか確認処理
            // 荷取高さ
            order.catch_height = "0";

            // 荷降ST

            //order.release_ST = order.release_ST;

            // ■搬送指示であればラックの予約フラグ更新
            if (order.order_type == "2001" | order.order_type == "1")
            {
                //Reserve_rack(order.catch_ST, order.release_ST, sqlConnection, sqlTransaction); // Canh_TV コメント
            }

            // ■対象に荷物が入って "いない" か確認処理

            // 荷降高さ
            order.release_height = "0";

            // 優先順位         

            order.priority_order = priority;

            // 号機指定
            order.machine_No = machine_No;

            string[] arry = order.make_arry();

            ORDER_writer(agf_shared_folder, arry, order, sqlConnection, sqlTransaction);

        }

        private void ORDER_writer(string agf_shared_folder, string[] arry, AGF_order_dat.ORDER order, SqlConnection sqlConnection, SqlTransaction sqlTransaction)
        {
            // 機能　 　　: csvに書き込む文字配列を受け取って、共有フォルダcsvファイルに書き込み
            // 返り値 　　:
            // 引き数 　　:
            // 作成日 　　: 2020/2/8
            // 作成者 　　: y.sato
            // 機能説明　 : 
            // 注意事項　 : 
            // ______________________________________________________________
            string ORDER_Path;
            int file_No_order = 1;
            using (var folder = new Folder_class(agf_shared_folder))
            {
                ORDER_Path = folder.ORDER;
            }
            // ファイルオープン
            // ■成否確認
            try
            {

                // ■指定ファイルが見つからなかった場合一定回数トライしてだめだったらエラー
                bool file_exist = false;
                int vali_coun = 0;
                const int try_count = 10;
                while (file_exist == false & vali_coun < try_count)
                {
                    try
                    {
                        FileSystem.FileClose(file_No_order);
                        FileSystem.FileOpen(file_No_order, ORDER_Path, OpenMode.Append, OpenAccess.Default, OpenShare.LockReadWrite);
                        break;
                    }
                    catch
                    {
                        vali_coun += 1;
                        System.Threading.Thread.Sleep(500);
                        if (vali_coun >= try_count)
                        {
                            var ex = new FileNotFoundException();
                            throw ex;
                        }
                    }
                }

                for (int i = 0, loopTo = arry.Length - 1; i <= loopTo; i++)
                {
                    if (i == arry.Length - 1)
                    {
                        FileSystem.PrintLine(file_No_order, arry[i]);
                    }
                    else
                    {
                        FileSystem.Print(file_No_order, Operators.AddObject(arry[i], ","));
                    }
                }

                LOG_write(order, sqlConnection, sqlTransaction);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                FileSystem.FileClose(file_No_order);
            }
        }

        private string Get_Unique_superior_key()
        {
            // 機能　 　　: ORDER.csv作成時 ユニークな上位キーを作成
            // 返り値 　　:
            // 引き数 　　:
            // 作成日 　　: 2019/11/13
            // 作成者 　　: y.sato
            // 機能説明　 : 
            // 注意事項　 : 
            // ______________________________________________________________

            string superior_key = "";
            using (var SQLTZN = new MSSQLAccess())
            {


                int length = 10;
                string passwordChars = "0123456789"; // 上位キーに使う文字
                string unique_evi = "";
                while (!(unique_evi is null))
                {
                    var sb = new StringBuilder(length);
                    var r = new Random();
                    for (int i = 0, loopTo = length - 1; i <= loopTo; i++)
                    {
                        // 文字の位置をランダムに選択
                        int pos = r.Next(passwordChars.Length);
                        // 選択された位置の文字を取得
                        char c = passwordChars[pos];
                        // 文字列の先頭が0だったら再変換
                        if (i == 0 & Conversions.ToString(c) == "0")
                        {
                            while (Conversions.ToString(c) == "0")
                                c = passwordChars[r.Next(passwordChars.Length)];
                        }
                        // パスワードに追加
                        sb.Append(c);
                    }
                    superior_key = sb.ToString();

                    // ORDER_LOGにアクセスして被りがないか調べる
                    string strSQL_superior = "SELECT TOP 1 superior_key FROM " + order_table + " WHERE superior_key = '" + superior_key + "'";
                    unique_evi = (string)SQLTZN.ExecuteScalar(strSQL_superior);

                }
            }
            return superior_key;
        }
        private void LOG_write(AGF_order_dat.ORDER order_dat)
        {
            // 機能 　  : 
            // 返り値   : なし
            // 引き数　 : なし
            // 作成日 　: 2019/10/25
            // 作成者   : 
            // 機能説明 : ORDER_LOGの更新
            // 注意事項 : 
            // 
            // ___________________________________________________________________________________

            using (var SQLTZN = new MSSQLAccess())
            {
                SQLTZN.ClearParam();

                SQLTZN.SetParam("@update_datetime", SqlDbType.NVarChar, order_dat.update_datetime);
                SQLTZN.SetParam("@update_date", SqlDbType.NVarChar, order_dat.update_date);
                SQLTZN.SetParam("@dataID", SqlDbType.NVarChar, order_dat.date_ID);
                SQLTZN.SetParam("@superior_key", SqlDbType.NVarChar, order_dat.superior_key);
                SQLTZN.SetParam("@related_sp_key", SqlDbType.NVarChar, order_dat.related_sp_key);
                SQLTZN.SetParam("@order_type", SqlDbType.NVarChar, order_dat.order_type);
                SQLTZN.SetParam("@order_detail", SqlDbType.NVarChar, order_dat.order_detail);
                SQLTZN.SetParam("@catch_ST", SqlDbType.NVarChar, order_dat.catch_ST);
                SQLTZN.SetParam("@catch_height", SqlDbType.NVarChar, order_dat.catch_height);
                SQLTZN.SetParam("@release_ST", SqlDbType.NVarChar, order_dat.release_ST);
                SQLTZN.SetParam("@release_height", SqlDbType.NVarChar, order_dat.release_height);
                SQLTZN.SetParam("@priority_order", SqlDbType.NVarChar, order_dat.priority_order);
                SQLTZN.SetParam("@machine_No", SqlDbType.NVarChar, order_dat.machine_No);

                SQLTZN.SetParam("@part_num", SqlDbType.NVarChar, order_dat.Part_num);
                SQLTZN.SetParam("@labelID", SqlDbType.NVarChar, order_dat.LabelID);
                SQLTZN.SetParam("@quantity", SqlDbType.NVarChar, order_dat.Quantity);

                string strSQL_LOG_up = "INSERT INTO " + order_table + " (update_datetime,update_date,dataID,superior_key,related_sp_key,order_type,order_detail, " + "catch_ST, catch_height,release_ST,release_height,priority_order,machine_No,part_num,labelID,quantity) VALUES (@update_datetime,@update_date,@dataID,@superior_key,@related_sp_key,@order_type,@order_detail, " + "@catch_ST,@catch_height,@release_ST,@release_height,@priority_order,@machine_No,@part_num,@labelID,@quantity) ";

                SQLTZN.InsertData(strSQL_LOG_up);

            }

        }

        private string Get_Unique_superior_key(SqlConnection sqlConnection, SqlTransaction sqlTransaction)
        {
            // 機能　 　　: ORDER.csv作成時 ユニークな上位キーを作成
            // 返り値 　　:
            // 引き数 　　:
            // 作成日 　　: 2019/11/13
            // 作成者 　　: y.sato
            // 機能説明　 : 
            // 注意事項　 : 
            // ______________________________________________________________

            string superior_key = "";

            int length = 10;
            string passwordChars = "0123456789"; // 上位キーに使う文字
            string unique_evi = "";
            while (!(unique_evi is null))
            {
                var sb = new StringBuilder(length);
                var r = new Random();
                for (int i = 0, loopTo = length - 1; i <= loopTo; i++)
                {
                    // 文字の位置をランダムに選択
                    int pos = r.Next(passwordChars.Length);
                    // 選択された位置の文字を取得
                    char c = passwordChars[pos];
                    // 文字列の先頭が0だったら再変換
                    if (i == 0 & Conversions.ToString(c) == "0")
                    {
                        while (Conversions.ToString(c) == "0")
                            c = passwordChars[r.Next(passwordChars.Length)];
                    }
                    // パスワードに追加
                    sb.Append(c);
                }
                superior_key = sb.ToString();

                var table = new DataTable();
                // ORDER_LOGにアクセスして被りがないか調べる
                string strSQL_superior = "SELECT TOP 1 superior_key FROM " + order_table + " WHERE superior_key = '" + superior_key + "'";
                var reader = sqlConnection.ExecuteReader(strSQL_superior, null, sqlTransaction);
                table.Load(reader);
                if(table.Rows.Count <= 0)
                {
                    unique_evi = null;
                }
                else
                {
                    unique_evi = Convert.ToString(table.Rows[0]["superior_key"]);
                }
                //unique_evi = (string)sqlConnection.ExecuteScalar(strSQL_superior, sqlTransaction);

            }
            
            return superior_key;
        }
        private void LOG_write(AGF_order_dat.ORDER order_dat, SqlConnection sqlConnection, SqlTransaction sqlTransaction)
        {
            // 機能 　  : 
            // 返り値   : なし
            // 引き数　 : なし
            // 作成日 　: 2019/10/25
            // 作成者   : 
            // 機能説明 : ORDER_LOGの更新
            // 注意事項 : 
            // 
            // ___________________________________________________________________________________

            string strSQL_LOG_up = "INSERT INTO " + order_table + 
                " (update_datetime,update_date,dataID,superior_key,related_sp_key,order_type,order_detail, " + "catch_ST, catch_height,release_ST,release_height,priority_order,machine_No, A_AGF_Motion_control_id, done_datetime) " +
                "VALUES (@update_datetime,@update_date,@dataID,@superior_key,@related_sp_key,@order_type,@order_detail, " + "@catch_ST,@catch_height,@release_ST,@release_height,@priority_order,@machine_No,@A_AGF_Motion_control_id, @done_datetime) ";
            var param = new
            {
                update_datetime = order_dat.update_datetime,
                update_date = order_dat.update_date,
                dataID = order_dat.date_ID,
                superior_key = order_dat.superior_key,
                related_sp_key = order_dat.related_sp_key,
                order_type = order_dat.order_type,
                order_detail = order_dat.order_detail,
                catch_ST = order_dat.catch_ST,
                catch_height = order_dat.catch_height,
                release_ST = order_dat.release_ST,
                release_height = order_dat.release_height,
                priority_order = order_dat.priority_order,
                machine_No = order_dat.machine_No,
                A_AGF_Motion_control_id = order_dat.A_AGF_Motion_control_id,
                done_datetime = DateTime.Now,

                //part_num = order_dat.Part_num,
                //labelID = order_dat.LabelID,
                //quantity = order_dat.Quantity,
            };
            sqlConnection.Execute(strSQL_LOG_up, param, sqlTransaction);

            //using (var SQLTZN = new MSSQLAccess())
            //{
            //    SQLTZN.ClearParam();

            //    SQLTZN.SetParam("@update_datetime", SqlDbType.NVarChar, order_dat.update_datetime);
            //    SQLTZN.SetParam("@update_date", SqlDbType.NVarChar, order_dat.update_date);
            //    SQLTZN.SetParam("@dataID", SqlDbType.NVarChar, order_dat.date_ID);
            //    SQLTZN.SetParam("@superior_key", SqlDbType.NVarChar, order_dat.superior_key);
            //    SQLTZN.SetParam("@related_sp_key", SqlDbType.NVarChar, order_dat.related_sp_key);
            //    SQLTZN.SetParam("@order_type", SqlDbType.NVarChar, order_dat.order_type);
            //    SQLTZN.SetParam("@order_detail", SqlDbType.NVarChar, order_dat.order_detail);
            //    SQLTZN.SetParam("@catch_ST", SqlDbType.NVarChar, order_dat.catch_ST);
            //    SQLTZN.SetParam("@catch_height", SqlDbType.NVarChar, order_dat.catch_height);
            //    SQLTZN.SetParam("@release_ST", SqlDbType.NVarChar, order_dat.release_ST);
            //    SQLTZN.SetParam("@release_height", SqlDbType.NVarChar, order_dat.release_height);
            //    SQLTZN.SetParam("@priority_order", SqlDbType.NVarChar, order_dat.priority_order);
            //    SQLTZN.SetParam("@machine_No", SqlDbType.NVarChar, order_dat.machine_No);

            //    SQLTZN.SetParam("@part_num", SqlDbType.NVarChar, order_dat.Part_num);
            //    SQLTZN.SetParam("@labelID", SqlDbType.NVarChar, order_dat.LabelID);
            //    SQLTZN.SetParam("@quantity", SqlDbType.NVarChar, order_dat.Quantity);

            //    string strSQL_LOG_up = "INSERT INTO " + order_table + " (update_datetime,update_date,dataID,superior_key,related_sp_key,order_type,order_detail, " + "catch_ST, catch_height,release_ST,release_height,priority_order,machine_No,part_num,labelID,quantity) VALUES (@update_datetime,@update_date,@dataID,@superior_key,@related_sp_key,@order_type,@order_detail, " + "@catch_ST,@catch_height,@release_ST,@release_height,@priority_order,@machine_No,@part_num,@labelID,@quantity) ";

            //    SQLTZN.InsertData(strSQL_LOG_up);

            //}

        }


        public string Get_TO_rack_number(string rack_type_TO)
        {
            // 機能 　  : 
            // 返り値   : 棚番号（AGF用）
            // 引き数　 : 棚の種類( 0:自動棚 , 1:入庫専用取合棚, 2:出庫専用取合棚 , 3:輸出梱包エリア棚)
            // 作成日 　: 2019/11/13
            // 作成者   : 佐藤　
            // 機能説明 : 入庫先の棚種類を指定するとランダムで空いている棚番号(AGF用）を返す
            // 
            // ___________________________________________________________________________________
            using (var SQLTZN = new MSSQLAccess())
            {
                string rack_number_TO;
                string strSQL_get_random_rackNo = " SELECT TOP 1 number FROM  " + auto_rack + " WHERE labelID = '' and reserved <> 1 and in_or_out = '" + rack_type_TO + "' ORDER BY priority ,number ";
                rack_number_TO = (string)SQLTZN.ExecuteScalar(strSQL_get_random_rackNo);


                return rack_number_TO;
            }

        }
        public void Reserve_rack(string rack_number_FROM, string rack_number_TO)
        {
            // 機能 　  : 
            // 返り値   :
            // 引き数　 : 使用する棚number 
            // 作成日 　: 2019/11/13
            // 作成者   : 佐藤　
            // 機能説明 : 指定された棚number予約に予約フラグを立てる。
            // 
            // ___________________________________________________________________________________
            using (var SQLTZN = new MSSQLAccess())
            {

                string strSQL_reserve = " UPDATE " + auto_rack + " SET reserved = 1  WHERE number IN ('" + rack_number_FROM + "','" + rack_number_TO + "')";
                SQLTZN.UpdateData(strSQL_reserve);

            }


        }
        public void Reserve_rack(string rack_number_FROM, string rack_number_TO, SqlConnection sqlConnection, SqlTransaction sqlTransaction)
        {
            // 機能 　  : 
            // 返り値   :
            // 引き数　 : 使用する棚number 
            // 作成日 　: 2019/11/13
            // 作成者   : 佐藤　
            // 機能説明 : 指定された棚number予約に予約フラグを立てる。
            // 
            // ___________________________________________________________________________________
            
            string strSQL_reserve = " UPDATE " + auto_rack + " SET reserved = 1  WHERE number IN ('" + rack_number_FROM + "','" + rack_number_TO + "')";
            sqlConnection.Execute(strSQL_reserve, sqlTransaction);

        }
        public string Unique_data_ID()
        {
            // 機能 　  :  ORDER.csvに必要なその日(同一ファイル内でユニークな8桁の数字を返す
            // 返り値   :　日付と合わせて一意のID
            // 引き数　 : なし
            // 作成日 　: 2019/12/10
            // 作成者   : 佐藤　
            // 機能説明 : 時間 6 桁 + ランダム2桁を合わせる
            // 
            // ___________________________________________________________________________________

            string unique_d_id;

            string time_string = DateTime.Now.ToLongTimeString();
            time_string = time_string.Replace(":", "");
            if (time_string.Length <= 5)
            {
                time_string = "0" + time_string;
            }

            int length = 2;
            string passwordChars = "0123456789"; // 上位キーに使う文字

            var sb = new StringBuilder(length);

            var r = new Random();
            for (int i = 0, loopTo = length - 1; i <= loopTo; i++)
            {

                // 文字の位置をランダムに選択
                int pos = r.Next(passwordChars.Length);
                // 選択された位置の文字を取得
                char c = passwordChars[pos];
                // パスワードに追加
                sb.Append(c);
            }


            unique_d_id = time_string + sb.ToString();

            return unique_d_id;
        }



    }
}
