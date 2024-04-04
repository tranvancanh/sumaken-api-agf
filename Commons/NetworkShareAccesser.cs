using System.Net;
using System.Runtime.InteropServices;

namespace sumaken_api_agf.Commons
{
    public class NetworkShareAccesser : IDisposable
    {

        string _remoteUncName;
        string _userName;
        string _password;
        bool _connected = false;

        public bool Connected => _connected;

        public NetworkShareAccesser(string remoteUncName, NetworkCredential credentials)
        {
            _remoteUncName = remoteUncName;
            _userName = credentials.UserName;
            _password = credentials.Password;

            var value = ConnectToRemote(_remoteUncName, _userName, _password);
            if(value == 0 || value == 1219)
                _connected = true;
            else
                _connected = false;
        }

        public void Dispose()
        {
            if (_connected)
                DisconnectRemote(_remoteUncName);
        }

        int ConnectToRemote(string remoteUnc, string username, string password)
        {
            var netResource = new NetResource
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = remoteUnc
            };

            var result = WNetAddConnection2(
                netResource,
                password,
                username,
                0);

            return result;
        }

        void DisconnectRemote(string remoteUnc)
        {
            WNetCancelConnection2(remoteUnc, 0, true);
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource,
            string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags,
            bool force);


        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public ResourceScope Scope;
            public ResourceType ResourceType;
            public ResourceDisplaytype DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        public enum ResourceScope : int
        {
            Connected = 1,
            GlobalNetwork,
            Remembered,
            Recent,
            Context
        };

        public enum ResourceType : int
        {
            Any = 0,
            Disk = 1,
            Print = 2,
            Reserved = 8,
        }

        public enum ResourceDisplaytype : int
        {
            Generic = 0x0,
            Domain = 0x01,
            Server = 0x02,
            Share = 0x03,
            File = 0x04,
            Group = 0x05,
            Network = 0x06,
            Root = 0x07,
            Shareadmin = 0x08,
            Directory = 0x09,
            Tree = 0x0a,
            Ndscontainer = 0x0b
        }
    }
}
