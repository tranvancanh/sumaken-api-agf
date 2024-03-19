
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

public partial class Folder_class : IDisposable
{


    // ■読みに行くフォルダ &ファイル名

    // 搬送指示
    public string Share_Foleder;
    // = "\\nas1\d\data\system\佐藤　優太\500_案件\510_社外\513_G040_GKN(常滑)\AGF_TEST"

    public string ORDER;

    // DCC状態
    public string DCC_STATE;

    // ■ORDER_STATE
    public string ORDER_STATE;


    public string user;
    public string pass;
    public string PCname;

    private Common_Class c_class = new Common_Class();

    public Folder_class()
    {
        // Share_Foleder = GetMessage()
        var ht = GetPath();

        Share_Foleder = Conversions.ToString(ht["path"]);
        ORDER = Share_Foleder + @"\ORDER.csv";
        DCC_STATE = Share_Foleder + @"\DCC_STATE.csv";
        ORDER_STATE = Share_Foleder + @"\ORDER_STATE.csv";

        if (c_class.share_folder_path_switch() == "外部環境")
        {
            user = Conversions.ToString(ht["user"]);
            pass = Conversions.ToString(ht["pass"]);
            PCname = Conversions.ToString(ht["PCname"]);
            string result = ConnectSrv(Share_Foleder, user, pass);
            // 認証ダイアログGET奴
            // 最初にコネクションダイアログをつなぐ
            // Dim share_folder_connecting_result As Boolean
            // share_folder_connecting_result = ShareFolderConnection()
            // If share_folder_connecting_result = False Then
            // Console.Write(Now() & "接続に失敗しました。")
            // End If

        }

    }

    public Hashtable GetPath()
    {
        // 機能 　  : 
        // 返り値   : 連想配列
        // 引き数　 : 
        // 作成日 　: 2020/01/28
        // 作成者   : 
        // 機能説明 : webconfigで指定したxmlファイルを参照して、ターゲット共有ファイルのパス、パスワード、userIDを取得して連想配列に格納
        // 注意事項 :         
        // ___________________________________________________________________________________
        string xmlFilePath;
        XmlDocument xmlDoc;
        XmlElement xmlElement;


        var ht = new Hashtable() { { "path", "" }, { "user", "" }, { "pass", "" }, { "PCname", "" } };

        // XMLファイルのパスをweb.configから取得 
        xmlFilePath = "/MyConfig.xml";
        //xmlFilePath = System.Configuration.ConfigurationManager.AppSettings["MyConfig"];

        // XMLファイル読み込み 
        xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlFilePath);

        var pathsList = xmlDoc.SelectNodes("//paths");

        // 引数のNOに該当するメッセージを取得 
        xmlElement = (XmlElement)pathsList[0];


        foreach (XmlElement msgElement in xmlElement.ChildNodes)
        {
            switch (msgElement.Name ?? "")
            {
                case "path":
                    {
                        ht["path"] = msgElement.InnerText;
                        break;
                    }
                case "user":
                    {
                        ht["user"] = msgElement.InnerText;
                        break;
                    }
                case "pass":
                    {
                        ht["pass"] = msgElement.InnerText;
                        break;
                    }
                case "PCname":
                    {
                        ht["PCname"] = msgElement.InnerText;
                        break;
                    }

            }

            // noStr = msgElement.GetAttribute("no")
            // If noStr = no Then
            // returnMsg = msgElement.InnerText
            // Exit For
            // End If
        }

        return ht;
    }
    public bool ShareFolderConnection()
    {

        bool result;
        // 共有フォルダのユーザー認証 (ダイアログで入力する手筈をプログラムで行う)
        var p = new Process();
        try
        {
            // Processオブジェクトを作成
            // ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
            // tozan-netサーバと接続
            p.StartInfo.FileName = Environment.GetEnvironmentVariable("ComSpec");
            // 出力を読み取れるようにする
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = false;
            // ウィンドウを表示しないようにする
            p.StartInfo.CreateNoWindow = true;
            // コマンドラインを指定（"/c"は実行後閉じるために必要）
            p.StartInfo.Arguments = "/c NET USE " + PCname + " " + pass + " /user:" + user;

            // 起動
            p.Start();

            // 出力を読み取る
            string results = p.StandardOutput.ReadToEnd();

            // プロセス終了まで待機する
            // WaitForExitはReadToEndの後である必要がある
            // (親プロセス、子プロセスでブロック防止のため)
            p.WaitForExit();

            result = true;
            return result;
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
            return false;
        }
        finally
        {
            p.Close();
        }

    }

    [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2W", CharSet = CharSet.Unicode)]
    public static extern int WNetAddConnection3(ref NETRESOURCE lpNetResource, string lpPassword, string lpUserName, int dwFlags);

    [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2W")]
    public static extern int WNetCancelConnection3(string lpName, int dwFlags, int fForce);

    public const int RESOURCE_CONNECTED = 0x1;
    public const int RESOURCETYPE_ANY = 0x0;
    public const int RESOURCEDISPLAYTYPE_SHARE = 0x3;
    public const int CONNECT_UPDATE_PROFILE = 0x1;
    // 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public partial struct NETRESOURCE
    {
        public int dwScope;
        public int dwType;
        public int dwDisplayType;
        public int dwUsage;
        public string lpLocalName;
        public string lpRemoteName;
        public string lpComment;
        public string lpProvider;
    }

    public string ConnectSrv(string networkPath, string userId, string password)
    {
        // 機能 　  : 
        // 返り値   : 空文字 or エラー文
        // 引き数　 : 共有フォルダパス, ユーザーID , パスワード
        // 作成日 　: 2020/01/28
        // 作成者   : 
        // 機能説明 : 指定共有フォルダにuserIdとpassを使ってアクセス
        // 注意事項 :         
        // ___________________________________________________________________________________


        int result;
        NETRESOURCE myResource;
        myResource.dwScope = 2; // RESOURCE_CONNECTED
        myResource.dwType = 1; // RESOURCETYPE_ANY
        myResource.dwDisplayType = 3; // RESOURCEDISPLAYTYPE_SHARE
        myResource.dwUsage = default;
        myResource.lpComment = null;
        myResource.lpLocalName = null;
        myResource.lpProvider = null;
        myResource.lpRemoteName = networkPath;

        try
        {
            // 接続あったら一回解除
            DisconnectSrv(Share_Foleder, user, pass);
            // ' なぜか Password が UserID より先！   '0 => CONNECT_UPDATE_PROFILE? 
            result = Folder_class.WNetAddConnection3(ref myResource, password, userId, 0);
            switch (result)
            {

                case 0:
                    {
                        Debug.Print("接続処理:正常終了。:" + result);
                        break;
                    }

                case 53:
                    {
                        Debug.Print("接続処理:PCが見つかりません。:" + result);
                        break;
                    }

                case 67:
                    {
                        Debug.Print("接続処理:パスが見つかりません。（共有になっていません）:" + result);
                        break;
                    }

                case 85:
                    {
                        Debug.Print("接続処理:既に接続済みです。:" + result);
                        break;
                    }

                default:
                    {
                        Debug.Print("接続処理:サーバーに接続できません。:" + result);
                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            return "接続に失敗しました " + ex.Message;

        }

        return "";
    }

    public void DisconnectSrv(string networkPath, string userId, string password)
    {
        // 機能 　  : 
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2020/01/28
        // 作成者   : 
        // 機能説明 : 共有フォルダの接続を閉じる
        // 注意事項 :         
        // ___________________________________________________________________________________
        string arglpName = Path.GetDirectoryName(networkPath);
        Folder_class.WNetCancelConnection3(arglpName, 0, Conversions.ToInteger(true));
    }

    private bool disposedValue = false;
    protected virtual void Dispose(bool disposing)
    {
        // 機能 　  : 
        // 返り値   : なし
        // 引き数　 : なし
        // 作成日 　: 2020/01/28
        // 作成者   : 
        // 機能説明 : usingの終わりに共有フォルダの接続閉じさせる
        // 注意事項 :         
        // ___________________________________________________________________________________

        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: 他の状態を解放します (マネージ オブジェクト)。
            }

            // TODO: ユーザー独自の状態を解放します (アンマネージ オブジェクト)。

            DisconnectSrv(Share_Foleder, user, pass);


            // TODO: 大きなフィールドを null に設定します。
        }
        disposedValue = true;

    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
