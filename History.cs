﻿namespace Boostera
{
    public class History
    {
        public string UniqueKey { get; set; }
        public string SearchKey { get; set; }
        public int Protocol { get; set; }
        public string Host { get; set; }
        public string User { get; set; }
        public string Port { get; set; }
        public string PrivateKey { get; set; }
        public string Password { get; set; }
        public string LogonScript { get; set; }
        public string WaitingString { get; set; }
        public int WaitingTime { get; set; }
        public bool IsForwarding { get; set; }
        public string ForwardingHost { get; set; }
        public string ForwardingUser { get; set; }
        public string ForwardingPort { get; set; }
        public string ForwardingPrivateKey { get; set; }
        public string ForwardingPassword { get; set; }
        public bool IsHide { get; set; }
        public string Tag { get; set; }

        public override string ToString()
        {
            return UniqueKey;
        }

        public History(string uniqueKey, string searchKey, int protocol, string host, string user, string port, string privateKey, string password,
            string logonScript, string waitingString, int waitingTime, bool isForwarding, string forwardingHost, string forwardingUser,
            string forwardingPort, string forwardingPrivateKey, string forwardingPassword, bool isHide, string tag)
        {
            UniqueKey = uniqueKey;
            SearchKey = searchKey;
            Protocol = protocol;
            Host = host;
            User = user;
            Port = port;
            PrivateKey = privateKey;
            Password = password;
            LogonScript = logonScript;
            WaitingString = waitingString;
            WaitingTime = waitingTime;
            IsForwarding = isForwarding;
            ForwardingHost = forwardingHost;
            ForwardingUser = forwardingUser;
            ForwardingPort = forwardingPort;
            ForwardingPrivateKey = forwardingPrivateKey;
            ForwardingPassword = forwardingPassword;
            IsHide = isHide;
            Tag = tag;
        }
    }
}
