namespace Boostera
{
    public class History
    {
        public string UniqueKey { get; set; }
        public int Protocol { get; set; }
        public string Host { get; set; }
        public string User { get; set; }
        public string Port { get; set; }
        public string PrivateKey { get; set; }
        public string Password { get; set; }
        public bool IsForwarding { get; set; }
        public string ForwardingHost { get; set; }
        public string ForwardingUser { get; set; }
        public string ForwardingPort { get; set; }
        public string ForwardingLocalPort { get; set; }
        public bool IsHide { get; set; }
        public string ForwardingPrivateKey { get; set; }
        public string ForwardingPassword { get; set; }
        public string Tag { get; set; }
        public string LogonScript { get; set; }
        public string SearchKey { get; set; }

        public override string ToString()
        {
            return UniqueKey;
        }

        public History(string uniqueKey, int protocol, string host, string user, string port, string privateKey, string password, bool isForwarding, string forwardingHost,
            string forwardingUser, string forwardingPort, string forwardingLocalPort, bool isHide, string forwardingPrivateKey, string forwardingPassword, string tag, string logonScript, string searchKey)
        {
            UniqueKey = uniqueKey;
            Protocol = protocol;
            Host = host;
            User = user;
            Port = port;
            PrivateKey = privateKey;
            Password = password;
            IsForwarding = isForwarding;
            ForwardingHost = forwardingHost;
            ForwardingUser = forwardingUser;
            ForwardingPort = forwardingPort;
            ForwardingLocalPort = forwardingLocalPort;
            IsHide = isHide;
            ForwardingPrivateKey = forwardingPrivateKey;
            ForwardingPassword = forwardingPassword;
            Tag = tag;
            LogonScript = logonScript;
            SearchKey = searchKey;
        }
    }
}
