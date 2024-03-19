
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic
using System.Data;
using System.Data.SqlClient;

public partial class MSSQLAccess : IDisposable
{

    protected SqlConnection conn;
    protected SqlCommand cmd;
    protected SqlDataAdapter da;
    protected SqlDataReader rd;
    protected SqlTransaction trn;
    protected bool trn_rollback_flag = false;


    // ラ･プラスDB
    // Protected ConnectionString_laplacedb As String = System.Configuration.ConfigurationManager.ConnectionStrings("laplacedbConnectionString").ConnectionString

    // アサヒサンクリーンDB
    // Protected GetConS As New TZNconf
    // Protected ConnectionString_ascdb As String = GetConS.TZN

    // 東山DB
    protected string ConnectionString = "Data Source=SQLTOZAN;Failover Partner=GWTOZAN2;Initial Catalog=tozandb;User ID=tozanadmin;Password=0526250148;Connection Timeout=86400;MultipleActiveResultSets=True;";

    protected DataTable dt01;

    protected List<SQLParam> lstMbrSQLParam = new List<SQLParam>();

    // SQL Paramater用構造体
    protected partial struct SQLParam
    {
        public string strParam;
        public SqlDbType dbType;
        public object objValue;
    }

    // =================================================
    // ----- DB接続を行うためのClass
    // =================================================

    public string ADB()
    {
        // 機能 　  : DB接続
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2009年12月07日
        // 作成者   : 加藤  宏章
        // 機能説明 : SQLサーバの接続文字列
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        return ConnectionString;

    }

    // <summary>
    // このSQLクラスで使用するDBを指定します。
    // </summary>
    // <param name="db">"ascdb" / "laplacedb" / "tozandb" / (ConectionString直接指定)</param>
    // Public Sub New(ByVal db As String)
    // '機能 　  : 
    // '返り値   : なし
    // '引き数　 : なし
    // '作成日 　: 2012年09月19日
    // '作成者   : 
    // '機能説明 : 
    // '注意事項 : 
    // '
    // '___________________________________________________________________________________

    // db = db.ToLower

    // Select Case db
    // Case "ascdb"
    // ConnectionString = ConnectionString_ascdb

    // 'Case "laplacedb"
    // '    ConnectionString = ConnectionString_laplacedb

    // 'Case "tozandb"
    // '    ConnectionString = ConnectionString_tozandb
    // Case Else   'ConnectionString直接指定も可
    // ConnectionString = db
    // End Select


    // conn = New SqlConnection(Me.ADB())
    // conn.Open()
    // cmd = New SqlCommand
    // da = New SqlDataAdapter

    // cmd = conn.CreateCommand

    // End Sub

    public MSSQLAccess()
    {
        // 機能 　  : 
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2012年09月19日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        // ConnectionString = ConnectionString_ascdb
        ConnectionString = ConnectionString;


        conn = new SqlConnection(ADB());
        conn.Open();

        cmd = new SqlCommand();
        da = new SqlDataAdapter();

        cmd = conn.CreateCommand();

    }

    public bool SetTransaction()
    {
        // 機能 　  : トランザクションの開始
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2012年10月26日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        try
        {
            trn = conn.BeginTransaction();   // トランザクション処理の開始
            cmd.Transaction = trn;         // コマンドをトランザクション処理に組み入れる

            return true;
        }

        catch (Exception ex)
        {
            return false;
        }

    }

    public bool CommitTransaction()
    {
        // 機能 　  : トランザクションのコミット
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2012年10月26日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        try
        {
            trn.Commit();
            trn.Dispose();
            trn = null;

            return true;
        }

        catch (Exception ex)
        {

            RollbackTransaction();

            return false;

        }

    }

    private bool RollbackTransaction()
    {
        // 機能 　  : トランザクションのロールバック
        // 返り値   : True :ロールバックした
        // False:ロールバックしなかった
        // 引き数　 : なし
        // 作成日 　: 2013年08月30日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        if (trn == null == true)
        {

            // Transactionオブジェクトが解放(trn = Nothing)済みの場合はCommit済みなので、ロールバックしない。
            return false;
        }
        else
        {
            trn.Rollback();          // トランザクション処理のRollback
            trn_rollback_flag = true;    // RollBackフラグをON
            trn.Dispose();
            trn = null;
            return true;

            // ViewParamTable() ←デバッグ時、コメントのままクイックウォッチするとパラメータの一覧が確認できます。
            // cmd.CommandText  ←デバッグ時、コメントのままクイックウォッチすると最後に実行されたSQL構文が確認できます。
        }


    }

    public void ResetTransactionFlag()
    {
        // 機能 　  : トランザクションのフラグのリセット
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2012年10月26日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        trn_rollback_flag = false;

    }

    public DataTable SelectData(string SELECT_CMD)
    {
        // 機能 　  : SQLの実行(Select文)
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2012年09月19日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        var lstParam = new List<SqlParameter>();
        var dt1 = new DataTable();

        if (SELECT_CMD.Contains("SELECT") | SELECT_CMD.Contains("select"))
        {

            cmd.CommandText = SELECT_CMD;
            da.SelectCommand = cmd;

            cmd.Parameters.Clear();

            if (lstMbrSQLParam.Count == 0)
            {
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(dt1);   // SELECTの実行
            }

            else
            {
                for (int i = 0, loopTo = lstMbrSQLParam.Count - 1; i <= loopTo; i++)
                {
                    lstParam.Add(new SqlParameter(lstMbrSQLParam[i].strParam, lstMbrSQLParam[i].dbType));
                    cmd.Parameters.Add(lstParam[i]);
                }

                for (int j = 0, loopTo1 = lstParam.Count - 1; j <= loopTo1; j++)
                    lstParam[j].Value = lstMbrSQLParam[j].objValue;

                da.SelectCommand.CommandTimeout = 0;
                da.Fill(dt1);
            }   // SELECTの実行
        }

        return dt1;

    }

    public DataTable StoredSelectData(string STORED_PROCEDURE_CMD)
    {
        // 機能 　  : ストアドプロシージャ文を使ってDataTable取り出し
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2012年09月19日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________
        int i;
        var lstParam = new List<SqlParameter>();
        var dt1 = new DataTable();

        // コマンドタイプをストアドプロシージャーにする
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = STORED_PROCEDURE_CMD;
        da.SelectCommand = cmd;

        cmd.Parameters.Clear();

        if (lstMbrSQLParam.Count == 0)
        {
            // 'パラメータなし
            da.Fill(dt1);   // SELECTの実行
        }
        else
        {
            var loopTo = lstMbrSQLParam.Count - 1;
            for (i = 0; i <= loopTo; i++)
            {
                lstParam.Add(new SqlParameter(lstMbrSQLParam[i].strParam, lstMbrSQLParam[i].dbType));
                cmd.Parameters.Add(lstParam[i]);
            }

            for (int j = 0, loopTo1 = lstParam.Count - 1; j <= loopTo1; j++)
                lstParam[j].Value = lstMbrSQLParam[j].objValue;
            da.Fill(dt1);
        }   // SELECTの実行

        // コマンドタイプを元に戻す
        cmd.CommandType = CommandType.Text;

        return dt1;

    }


    public bool ExecuteReaderSetSQL(string SELECT_CMD)
    {
        // 機能 　  : SQLの実行(Select文)
        // 返り値   : 読取成功True,失敗はFalse 
        // 引き数　 : ARG1 - Select文
        // 作成日 　: 2012年11月17日
        // 作成者   : 星野　邦晴
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        // 'SQL構文の実行をします。
        int i;
        var lstParam = new List<SqlParameter>();
        dt01 = new DataTable();

        if (SELECT_CMD.Contains("SELECT") | SELECT_CMD.Contains("select"))
        {

            cmd.CommandText = SELECT_CMD;
            cmd.Parameters.Clear();

            if (lstMbrSQLParam.Count == 0)
            {
                try
                {
                    // 'パラメータなし、ExcecuteReaderで実行
                    rd = cmd.ExecuteReader();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {

                var loopTo = lstMbrSQLParam.Count - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    lstParam.Add(new SqlParameter(lstMbrSQLParam[i].strParam, lstMbrSQLParam[i].dbType));
                    cmd.Parameters.Add(lstParam[i]);
                }

                // 'For i = 0 To lstMbrSQLParam(0).objValue.Count - 1
                // 'ExecuteReaderは実行が一つしかできないためパラメータの値の複数は対応できず
                i = 0;

                for (int j = 0, loopTo1 = lstParam.Count - 1; j <= loopTo1; j++)
                    lstParam[j].Value = lstMbrSQLParam[j].objValue;

                try
                {
                    // 'パラメータなし、ExcecuteReaderで実行
                    rd = cmd.ExecuteReader();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }
        else
        {
            return false;
        }

    }

    public Dictionary<string, object> ExecuteReader()
    {
        // 機能 　  : 1行1行　実行して値の取り出しを行う
        // 返り値   : フィールド名と値を返す 
        // 引き数　 : なし
        // 作成日 　: 2012年11月17日
        // 作成者   : 星野　邦晴
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        var dict = new Dictionary<string, object>();

        try
        {
            if (rd.Read())
            {
                for (int i = 0, loopTo = rd.GetSchemaTable().Rows.Count - 1; i <= loopTo; i++)
                    dict.Add(rd.GetName(i), rd.GetValue(i));
            }
        }

        catch (Exception ex)
        {
            ExecuteReaderClose();
        }

        return dict;

    }

    public void ExecuteReaderClose()
    {
        // 機能 　  : ExecuteReader は解放しないと次が使えない
        // 返り値   : 解放処理
        // 引き数　 : なし
        // 作成日 　: 2012年11月17日
        // 作成者   : 星野　邦晴
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        rd.Close();
        rd = null;

    }

    public bool InsertData(string INSERT_CMD)
    {
        // 機能 　  : SQLの実行(Insert文) パラメータQUERY対応版
        // 返り値   : なし
        // 引き数　 : 1:SQL構文
        // 作成日 　: 2012年09月19日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        // トランザクション処理を行い、ロールバックが発生した場合は、フラグを手動でOFFにするまではInsertとDeleteは動作しない。
        if (trn_rollback_flag == true)
        {
            return false;
            return default;
        }

        if (INSERT_CMD.Contains("INSERT") | INSERT_CMD.Contains("insert"))
        {
            cmd.CommandText = INSERT_CMD;
            cmd.Parameters.Clear();

            if (lstMbrSQLParam.Count == 0)    // パラメータ設定なしの場合------------------
            {
                cmd.CommandTimeout = 0;

                // トランザクション中かどうか
                if (trn == null)
                {
                    cmd.ExecuteNonQuery();   // INSERTの実行
                }
                else
                {
                    try
                    {
                        // cmd.Prepare()
                        cmd.ExecuteNonQuery();   // INSERTの実行
                    }
                    catch (Exception ex)
                    {

                        RollbackTransaction();

                        return false;
                        return default;
                    }
                }
            }

            // トランザクション中かどうか
            else if (trn == null)    // パラメータ設定ありの場合---------------------------------------------
            {
                ExecuteNonQueryUsingParameters(); // パラメータ付きInsertの実行
            }
            else
            {
                try
                {
                    ExecuteNonQueryUsingParameters(); // パラメータ付きInsertの実行
                }
                catch (Exception ex)
                {

                    RollbackTransaction();

                    // ViewParamTable()

                    return false;
                    return default;
                }
            }

            return true;
        }
        else
        {
            return false;
        }

    }

    public bool DeleteData(string DELETE_CMD)
    {
        // 機能 　  : SQLの実行(Delete文)
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2012年09月19日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        // トランザクション処理を行い、ロールバックが発生した場合は、フラグを手動でOFFにするまではInsertとDeleteは動作しない。
        if (trn_rollback_flag == true)
        {
            return false;
            return default;
        }

        if (DELETE_CMD.Contains("DELETE") | DELETE_CMD.Contains("delete") | DELETE_CMD.Contains("TRUNCATE"))
        {
            cmd.CommandText = DELETE_CMD;
            cmd.Parameters.Clear();

            if (lstMbrSQLParam.Count == 0)    // パラメータ設定なしの場合------------------
            {

                cmd.CommandTimeout = 0;

                // トランザクション中かどうか
                if (trn == null)
                {
                    cmd.ExecuteNonQuery();   // DELETEの実行
                }
                else
                {
                    try
                    {
                        cmd.ExecuteNonQuery();   // DELETEの実行
                    }
                    catch (Exception ex)
                    {

                        RollbackTransaction();

                        return false;
                        return default;
                    }

                }
            }

            // トランザクション中かどうか
            else if (trn == null)    // パラメータ設定ありの場合---------------------------------------------
            {
                ExecuteNonQueryUsingParameters(); // パラメータ付きDELETEの実行
            }
            else
            {
                try
                {
                    ExecuteNonQueryUsingParameters(); // パラメータ付きDELETEの実行
                }
                catch (Exception ex)
                {

                    RollbackTransaction();

                    return false;
                    return default;
                }
            }

            return true;
        }
        else
        {
            return false;
        }

    }

    public bool UpdateData(string UPDATE_CMD)
    {
        // 機能 　  : SQLの実行(Update文)
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2012年09月19日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        if (UPDATE_CMD.Contains("UPDATE") | UPDATE_CMD.Contains("update"))
        {

            cmd.CommandText = UPDATE_CMD;
            cmd.Parameters.Clear();

            if (lstMbrSQLParam.Count == 0)    // パラメータ設定なしの場合------------------
            {
                cmd.CommandTimeout = 0;

                // トランザクション中かどうか
                if (trn == null)
                {
                    cmd.ExecuteNonQuery();   // UPDATEの実行
                }
                else
                {
                    try
                    {
                        cmd.ExecuteNonQuery();   // UPDATEの実行
                    }
                    catch (Exception ex)
                    {

                        RollbackTransaction();

                        return false;
                        return default;
                    }
                }
            }

            // トランザクション中かどうか
            else if (trn == null)    // パラメータ設定ありの場合---------------------------------------------
            {
                ExecuteNonQueryUsingParameters(); // パラメータ付きUpdateの実行
            }
            else
            {

                try
                {
                    ExecuteNonQueryUsingParameters(); // パラメータ付きUpdateの実行
                }
                catch (Exception ex)
                {

                    RollbackTransaction();

                    return false;
                    return default;
                }

            }

            return true;
        }
        else
        {
            return false;

        }

    }

    private void ExecuteNonQueryUsingParameters()
    {
        // 機能 　  : パラメータ付きSQLの実行
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2012年11月16日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : cmd.CommandText にSQL構文がセットされている前提で動作します。
        // 
        // ___________________________________________________________________________________

        int i;
        var lstParam = new List<SqlParameter>();

        var loopTo = lstMbrSQLParam.Count - 1;
        for (i = 0; i <= loopTo; i++)
        {
            lstParam.Add(new SqlParameter(lstMbrSQLParam[i].strParam, lstMbrSQLParam[i].dbType));
            cmd.Parameters.Add(lstParam[i]);
        }


        for (int j = 0, loopTo1 = lstParam.Count - 1; j <= loopTo1; j++)
            lstParam[j].Value = lstMbrSQLParam[j].objValue;

        // エラー時のデバッグ用(本番ではコメント化を推奨)
        string strSQL = cmd.CommandText;    // strSQL = SQL構文
        var dt1 = ViewParamTable();      // dt1    = パラメータ一覧

        cmd.CommandTimeout = 0;
        cmd.ExecuteNonQuery();   // SQLの実行 

    }

    public DataTable InsertDataLog(string INSERT_CMD)
    {
        // 機能 　  : SQLの実行(Insert文) パラメータQUERY対応版
        // 返り値   : なし
        // 引き数　 : 1:SQL構文
        // 作成日 　: 2012年09月19日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        var logdt = new DataTable();
        // トランザクション処理を行い、ロールバックが発生した場合は、フラグを手動でOFFにするまではInsertとDeleteは動作しない。
        if (trn_rollback_flag == true)
        {
            return null;
            return default;
        }

        if (INSERT_CMD.Contains("INSERT") | INSERT_CMD.Contains("insert"))
        {
            cmd.CommandText = INSERT_CMD;
            da.InsertCommand = cmd;
            cmd.Parameters.Clear();

            if (lstMbrSQLParam.Count == 0)    // パラメータ設定なしの場合------------------
            {
                cmd.CommandTimeout = 0;

                // トランザクション中かどうか
                if (trn == null)
                {
                    // cmd.ExecuteNonQuery()   'INSERTの実行
                    da.Fill(logdt);   // SELECTの実行
                }
                else
                {
                    try
                    {
                        // cmd.Prepare()
                        // cmd.ExecuteNonQuery()   'INSERTの実行
                        da.Fill(logdt);   // SELECTの実行
                    }
                    catch (Exception ex)
                    {

                        RollbackTransaction();

                        return null;
                        return default;
                    }
                }
            }

            // トランザクション中かどうか
            else if (trn == null)    // パラメータ設定ありの場合---------------------------------------------
            {
                ExecuteNonQueryUsingParametersLog(); // パラメータ付きInsertの実行
            }
            else
            {
                try
                {
                    ExecuteNonQueryUsingParametersLog(); // パラメータ付きInsertの実行
                }
                catch (Exception ex)
                {

                    RollbackTransaction();

                    // ViewParamTable()

                    return null;
                    return default;
                }
            }

            return logdt;
        }
        else
        {
            return null;

        }

    }

    public DataTable DeleteDataLog(string DELETE_CMD)
    {
        // 機能 　  : SQLの実行(Delete文)
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2012年09月19日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        var logdt = new DataTable();
        // トランザクション処理を行い、ロールバックが発生した場合は、フラグを手動でOFFにするまではInsertとDeleteは動作しない。
        if (trn_rollback_flag == true)
        {
            return null;
            return default;
        }

        if (DELETE_CMD.Contains("DELETE") | DELETE_CMD.Contains("delete") | DELETE_CMD.Contains("TRUNCATE"))
        {
            cmd.CommandText = DELETE_CMD;
            da.DeleteCommand = cmd;
            cmd.Parameters.Clear();

            if (lstMbrSQLParam.Count == 0)    // パラメータ設定なしの場合------------------
            {
                cmd.CommandTimeout = 0;

                // トランザクション中かどうか
                if (trn == null)
                {
                    // cmd.ExecuteNonQuery()   'DELETEの実行
                    da.Fill(logdt);   // SELECTの実行
                }
                else
                {
                    try
                    {
                        // cmd.ExecuteNonQuery()   'DELETEの実行
                        da.Fill(logdt);   // SELECTの実行
                    }
                    catch (Exception ex)
                    {

                        RollbackTransaction();

                        return null;
                        return default;
                    }
                }
            }

            // トランザクション中かどうか
            else if (trn == null)    // パラメータ設定ありの場合---------------------------------------------
            {
                ExecuteNonQueryUsingParametersLog(); // パラメータ付きDELETEの実行
            }
            else
            {
                try
                {
                    ExecuteNonQueryUsingParametersLog(); // パラメータ付きDELETEの実行
                }
                catch (Exception ex)
                {

                    RollbackTransaction();

                    return null;
                    return default;
                }
            }

            return logdt;
        }
        else
        {
            return null;
        }

    }

    public DataTable UpdateDataLog(string UPDATE_CMD)
    {
        // 機能 　  : SQLの実行(Update文)
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2012年09月19日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        // Dim conn2 As New SqlConnection(Me.ADB())
        // conn2.Open()
        // Dim cmd2 As New SqlCommand
        // cmd2 = conn2.CreateCommand

        var logdt = new DataTable();
        if (UPDATE_CMD.Contains("UPDATE") | UPDATE_CMD.Contains("update"))
        {

            cmd.CommandText = UPDATE_CMD;
            da.SelectCommand = cmd;
            cmd.Parameters.Clear();

            if (lstMbrSQLParam.Count == 0)    // パラメータ設定なしの場合------------------
            {

                cmd.CommandTimeout = 0;

                // トランザクション中かどうか
                if (trn == null)
                {
                    da.Fill(logdt);   // SELECTの実行
                }
                // rd = cmd2.ExecuteReader()   'UPDATEの実行
                // Dim Fcnt As Integer = rd.FieldCount
                // While rd.Read
                // Dim strAction As String = rd.Item(0)
                // Dim strDeleted As String = ""
                // Dim strInserted As String = ""
                // Dim strrd As String = ""
                // For i As Integer = 1 To rd.FieldCount - 1
                // If IsDBNull(rd.Item(i)) Then
                // strrd = ""
                // Else
                // Select Case TypeName(rd.Item(i))
                // Case "Byte()"
                // strrd = CType(rd.Item(i), Byte())(0).ToString
                // Case Else
                // strrd = rd.Item(i).ToString
                // End Select

                // End If
                // If i > rd.FieldCount / 2 Then
                // strDeleted += strrd & ","
                // Else
                // strInserted += strrd & ","
                // End If
                // Next
                // Dim InsertSQLStr = "INSERT INTO tzn_kintai_Log VALUES ('" & strAction & "', '" & strDeleted & "', '" & strInserted & "')"
                // InsertData(InsertSQLStr)
                // End While
                else
                {
                    try
                    {
                        da.Fill(logdt);   // SELECTの実行'rd = cmd2.ExecuteReader()   'UPDATEの実行
                    }
                    catch (Exception ex)
                    {
                        RollbackTransaction();

                        return null; // Return False
                        return default;
                    }
                }
            }

            // トランザクション中かどうか
            else if (trn == null)    // パラメータ設定ありの場合---------------------------------------------
            {
                ExecuteNonQueryUsingParametersLog(); // パラメータ付きUpdateの実行
            }
            else
            {
                try
                {
                    ExecuteNonQueryUsingParametersLog(); // パラメータ付きUpdateの実行
                }
                catch (Exception ex)
                {

                    RollbackTransaction();

                    return null; // Return False
                    return default;
                }
            }

            return logdt; // Return True
        }
        else
        {
            return null;
        } // Return False

    }

    private DataTable ExecuteNonQueryUsingParametersLog()
    {
        // 機能 　  : パラメータ付きSQLの実行
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2012年11月16日
        // 作成者   : 
        // 機能説明 : 
        // 注意事項 : cmd.CommandText にSQL構文がセットされている前提で動作します。
        // 
        // ___________________________________________________________________________________

        int i;
        var lstParam = new List<SqlParameter>();

        var loopTo = lstMbrSQLParam.Count - 1;
        for (i = 0; i <= loopTo; i++)
        {
            lstParam.Add(new SqlParameter(lstMbrSQLParam[i].strParam, lstMbrSQLParam[i].dbType));
            cmd.Parameters.Add(lstParam[i]);
        }

        for (int j = 0, loopTo1 = lstParam.Count - 1; j <= loopTo1; j++)
            lstParam[j].Value = lstMbrSQLParam[j].objValue;

        // エラー時のデバッグ用(本番ではコメント化を推奨)
        string strSQL = cmd.CommandText;    // strSQL = SQL構文
        var dt1 = ViewParamTable();      // dt1    = パラメータ一覧

        cmd.CommandTimeout = 0;
        var logdt = new DataTable();
        da.Fill(logdt);   // SELECTの実行'cmd.ExecuteNonQuery()   'SQLの実行 
        return logdt;

    }

    public object ExecuteScalar(string SQL_CMD)
    {
        // 機能 　  : 返り値が１件(１行かつ１フィールド)しか返らないSQLを実行するときに使用する
        // 返り値   : SQLの返り値
        // 引き数　 : SQL構文
        // 作成日 　: 2011年09月23日
        // 作成者   : 加藤　宏章
        // 機能説明 : 
        // 注意事項 : ExecuteNonQueryとは違い、パラメータをList型で複数渡しても１度しか実行しない。
        // 
        // ___________________________________________________________________________________

        int i;
        var lstParam = new List<SqlParameter>();

        cmd.CommandText = SQL_CMD;
        cmd.Parameters.Clear();

        object parm = "";

        if (lstMbrSQLParam.Count == 0)
        {
            parm = cmd.ExecuteScalar();
        }
        else
        {
            var loopTo = lstMbrSQLParam.Count - 1;
            for (i = 0; i <= loopTo; i++)
            {
                lstParam.Add(new SqlParameter(lstMbrSQLParam[i].strParam, lstMbrSQLParam[i].dbType));
                cmd.Parameters.Add(lstParam[i]);
            }

            for (int j = 0, loopTo1 = lstParam.Count - 1; j <= loopTo1; j++)
                lstParam[j].Value = lstMbrSQLParam[j].objValue;

            // ViewParamTable() ←エラー時、この関数をクイックウォッチするとパラメータ一覧が確認できる。

            parm = cmd.ExecuteScalar();

        }

        return parm;

    }

    /// <summary>
    /// SQL構文のパラメータを指定します。
    /// 第３引数をObject型のList変数で渡すと、自動的に複数回のSQL(UPDATE/INSERT)が動作します。
    /// </summary>
    /// <param name="strParam1">@から始まるパラメータ文字列</param>
    /// <param name="dbType1">パラメータ文字列のデータ型(SqlDbType.***)</param>
    /// <param name="Value1">パラメータ文字列に結びつける値</param>
    public void SetParam(string strParam1, SqlDbType dbType1, object Value1)
    {
        // 機能 　  : パラメータのセット「SELECTなどはこちら」
        // 返り値   : SQLの返り値
        // 引き数　 : ARG1 - パラメータ名
        // ARG2 - パラメータの型
        // ARG3 - パラメータの値(単一)
        // 作成日 　: 2012年11月20日
        // 作成者   : 加藤　宏章
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        SQLParam stcSQLParam;

        // 既に同一のパラメータ文字列がセットされてたら一旦除去
        for (int i = 0, loopTo = lstMbrSQLParam.Count - 1; i <= loopTo; i++)
        {
            {
                var withBlock = lstMbrSQLParam[i];
                if ((withBlock.strParam ?? "") == (strParam1 ?? ""))
                {
                    lstMbrSQLParam.RemoveAt(i);
                    break;
                }
            }
        }

        stcSQLParam.strParam = strParam1;
        stcSQLParam.dbType = dbType1;
        stcSQLParam.objValue = Value1;

        lstMbrSQLParam.Add(stcSQLParam);

    }

    /// <summary>
    /// SQL構文のパラメータを指定します。
    /// </summary>
    /// <param name="strParam1">@から始まるパラメータ文字列</param>
    /// <param name="strType1">パラメータ文字列のデータ型(nvarchar/string/int/double/decimal/date/datetime/datetimeMilliseconds/boolean/bit)</param>
    /// <param name="Value1">パラメータ文字列に結びつける値</param>
    public void SetParam(string strParam1, string strType1, object Value1)
    {
        // 機能 　  : パラメータのセット「SELECTなどはこちら」
        // 返り値   : SQLの返り値
        // 引き数　 : ARG1 - パラメータ名
        // ARG2 - パラメータの型(文字列で指定)
        // ARG3 - パラメータの値(単一)
        // 作成日 　: 2014年02月11日
        // 作成者   : 加藤　宏章
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        SQLParam stcSQLParam;

        // 既に同一のパラメータ文字列がセットされてたら一旦除去
        for (int i = 0, loopTo = lstMbrSQLParam.Count - 1; i <= loopTo; i++)
        {
            {
                var withBlock = lstMbrSQLParam[i];
                if ((withBlock.strParam ?? "") == (strParam1 ?? ""))
                {
                    lstMbrSQLParam.RemoveAt(i);
                    break;
                }
            }
        }

        stcSQLParam.strParam = strParam1;

        stcSQLParam.objValue = Value1;

        strType1 = Strings.StrConv(strType1, VbStrConv.Uppercase);   // 大文字変換
        strType1 = Strings.StrConv(strType1, VbStrConv.Narrow);      // 半角変換

        switch (strType1 ?? "")
        {
            case "NVARCHAR":
                {
                    stcSQLParam.dbType = SqlDbType.NVarChar;
                    stcSQLParam.objValue = Value1.ToString();
                    break;
                }
            case "STRING":
                {
                    stcSQLParam.dbType = SqlDbType.NVarChar;
                    stcSQLParam.objValue = Value1.ToString();
                    break;
                }
            case "INT":
                {
                    stcSQLParam.dbType = SqlDbType.Int;
                    if (string.IsNullOrEmpty(Value1.ToString()))
                    {
                        stcSQLParam.objValue = 0;
                    }

                    break;
                }
            case "DOUBLE":
                {
                    stcSQLParam.dbType = SqlDbType.BigInt;
                    break;
                }
            case "DECIMAL":
                {
                    stcSQLParam.dbType = SqlDbType.Decimal;
                    break;
                }
            case "DATE":
                {
                    stcSQLParam.dbType = SqlDbType.Date;
                    if (string.IsNullOrEmpty(Value1.ToString()))
                    {
                        stcSQLParam.objValue = "1900/01/01 0:00:00";
                    }
                    else if (Information.IsDate(Value1) == false && Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Value1, 0, false)))
                    {
                        stcSQLParam.objValue = "1900/01/01 0:00:00";
                    }

                    break;
                }
            case "DATETIME":
                {
                    stcSQLParam.dbType = SqlDbType.DateTime2;
                    if (string.IsNullOrEmpty(Value1.ToString()))
                    {
                        stcSQLParam.objValue = "1900/01/01 0:00:00";
                    }
                    else if (Information.IsDate(Value1) == false && Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Value1, 0, false)))
                    {
                        stcSQLParam.objValue = "1900/01/01 0:00:00";
                    }
                    if (Information.IsDate(Value1) == true)
                    {
                        stcSQLParam.objValue = Conversions.ToDate(Value1).ToString("yyyy/MM/dd HH:mm:ss");  // データ上のミリ秒を切り捨てる
                    }

                    break;
                }
            case "DATETIMEMILLISECONDS": // ミリ秒も有効な日付型
                {
                    stcSQLParam.dbType = SqlDbType.DateTime2;
                    if (string.IsNullOrEmpty(Value1.ToString()))
                    {
                        stcSQLParam.objValue = "1900/01/01 0:00:00";
                    }
                    else if (Information.IsDate(Value1) == false && Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Value1, 0, false)))
                    {
                        stcSQLParam.objValue = "1900/01/01 0:00:00";
                    }

                    break;
                }
            case "BOOLEAN":
                {
                    stcSQLParam.dbType = SqlDbType.Bit;
                    switch (Value1.ToString().ToUpper() ?? "")
                    {
                        case "TRUE":
                            {
                                stcSQLParam.objValue = true;
                                break;
                            }
                        case "FALSE":
                            {
                                stcSQLParam.objValue = false;
                                break;
                            }
                        case "1":
                            {
                                stcSQLParam.objValue = true;
                                break;
                            }
                        case "0":
                            {
                                stcSQLParam.objValue = false;
                                break;
                            }
                    }

                    break;
                }
            case "BIT":
                {
                    stcSQLParam.dbType = SqlDbType.Bit;
                    switch (Value1.ToString().ToUpper() ?? "")
                    {
                        case "TRUE":
                            {
                                stcSQLParam.objValue = true;
                                break;
                            }
                        case "FALSE":
                            {
                                stcSQLParam.objValue = false;
                                break;
                            }
                        case "1":
                            {
                                stcSQLParam.objValue = true;
                                break;
                            }
                        case "0":
                            {
                                stcSQLParam.objValue = false;
                                break;
                            }
                    }

                    break;
                }

            default:
                {
                    stcSQLParam.dbType = SqlDbType.NVarChar;
                    stcSQLParam.objValue = "型指定エラー(MSSQLAccess)";
                    break;
                }
                // Exit Sub
        }

        lstMbrSQLParam.Add(stcSQLParam);

    }

    /// <summary>
    /// SQL構文のパラメータをクリアします。
    /// 引数に@から始まるパラメータ文字列を指定すると、特定のパラメータのみ削除できます。
    /// </summary>
    public void ClearParam()
    {
        // 機能 　  : パラメータをすべてクリアーします
        // 返り値   : パラメータをなくします
        // 引き数　 : '作成日 　: 2012年11月20日
        // 作成者   : 加藤　宏章
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        lstMbrSQLParam.Clear();

    }

    /// <summary>
    /// SQL構文のパラメータをクリアします。
    /// 引数に@から始まるパラメータ文字列を指定すると、特定のパラメータのみ削除できます。
    /// </summary>
    /// <param name="strParam1">@から始まるパラメータ文字列</param>
    public void ClearParam(string strParam1)
    {
        // 機能 　  : パラメータをすべてクリアーします
        // 返り値   : パラメータをなくします
        // 引き数　 : '作成日 　: 2012年11月20日
        // 作成者   : 加藤　宏章
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________
        int i;

        var loopTo = lstMbrSQLParam.Count - 1;
        for (i = 0; i <= loopTo; i++)
        {
            {
                var withBlock = lstMbrSQLParam[i];
                if ((withBlock.strParam ?? "") == (strParam1 ?? ""))
                {
                    lstMbrSQLParam.RemoveAt(i);
                    return;
                }
            }

        }

    }

    public DataTable ViewParamTable()
    {
        // 機能 　  : パラメータ一覧の表示
        // 返り値   : パラメータ一覧のDataTable
        // 引き数　 : 
        // 作成日 　: 2012年12月11日
        // 作成者   : 加藤　宏章
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        DataTable dt1;
        DataRow dr1;

        dt1 = new DataTable();
        // dt1.TableName = ""
        dt1.Columns.Add("strParam", Type.GetType("System.String"));
        dt1.Columns.Add("dbType", Type.GetType("System.String"));
        dt1.Columns.Add("objValue", Type.GetType("System.String"));

        for (int i = 0, loopTo = lstMbrSQLParam.Count - 1; i <= loopTo; i++)
        {
            {
                var withBlock = lstMbrSQLParam[i];
                dr1 = dt1.NewRow();
                dr1["strParam"] = withBlock.strParam;
                dr1["dbType"] = withBlock.dbType;
                dr1["objValue"] = withBlock.objValue;

                dt1.Rows.Add(dr1);
            }
        }

        return dt1;

        dt1.Dispose();

    }

    public bool GetRollbackFlag()
    {

        return trn_rollback_flag;

    }

    private bool disposedValue = false;        // 重複する呼び出しを検出するには

    // IDisposable
    protected virtual void Dispose(bool disposing)
    {
        // 機能 　  : SQL文が終了すると自動でDisposeします
        // 返り値   : トランザクション、コネクション、データリーダなどのすべて解放します
        // 引き数　 : '作成日 　: 2012年11月20日
        // 作成者   : 加藤　宏章
        // 機能説明 : 
        // 注意事項 : 
        // 
        // ___________________________________________________________________________________

        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: 他の状態を解放します (マネージ オブジェクト)。
            }

            // TODO: ユーザー独自の状態を解放します (アンマネージ オブジェクト)。

            // Disposeの時点でTransactionオブジェクトが解放(trn = Nothing)されずに残っている場合、エラーとみなし、トランザクション処理のRollback。
            // これを防ぐには、必ず CommitTransaction() を実行すること。
            RollbackTransaction();

            if (rd == null == false)
            {
                rd.Close();
                rd = null;
            }
            da.Dispose();
            cmd.Dispose();
            conn.Close();
            conn.Dispose();

            // TODO: 大きなフィールドを null に設定します。
        }
        disposedValue = true;

    }

    #region  IDisposable Support 
    // このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
    public void Dispose()
    {
        // このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion

}