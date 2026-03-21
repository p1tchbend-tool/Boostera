namespace Boostera
{
    internal class History
    {
        internal string UniqueKey { get; set; }
        internal string SearchKey { get; set; }
        internal int Protocol { get; set; }
        internal string Host { get; set; }
        internal string User { get; set; }
        internal string Port { get; set; }
        internal string PrivateKey { get; set; }
        internal string Password { get; set; }
        internal bool IsEnvPassword { get; set; }
        internal string LogonScript { get; set; }
        internal string WaitingString { get; set; }
        internal string WaitingTime { get; set; }
        internal bool IsForwarding { get; set; }
        internal string ForwardingHost { get; set; }
        internal string ForwardingUser { get; set; }
        internal string ForwardingPort { get; set; }
        internal string ForwardingPrivateKey { get; set; }
        internal string ForwardingPassword { get; set; }
        internal bool ForwardingIsEnvPassword { get; set; }
        internal bool IsHide { get; set; }
        internal string Tag { get; set; }

        public override string ToString()
        {
            return UniqueKey;
        }

        internal History(string uniqueKey, string searchKey, int protocol, string host, string user, string port, string privateKey, string password,
            bool isEnvPassword, string logonScript, string waitingString, string waitingTime, bool isForwarding, string forwardingHost, string forwardingUser,
            string forwardingPort, string forwardingPrivateKey, string forwardingPassword, bool forwardingIsEnvPassword, bool isHide, string tag)
        {
            UniqueKey = uniqueKey;
            SearchKey = searchKey;
            Protocol = protocol;
            Host = host;
            User = user;
            Port = port;
            PrivateKey = privateKey;
            Password = password;
            IsEnvPassword = isEnvPassword;
            LogonScript = logonScript;
            WaitingString = waitingString;
            WaitingTime = waitingTime;
            IsForwarding = isForwarding;
            ForwardingHost = forwardingHost;
            ForwardingUser = forwardingUser;
            ForwardingPort = forwardingPort;
            ForwardingPrivateKey = forwardingPrivateKey;
            ForwardingPassword = forwardingPassword;
            ForwardingIsEnvPassword = forwardingIsEnvPassword;
            IsHide = isHide;
            Tag = tag;
        }
    }
}
