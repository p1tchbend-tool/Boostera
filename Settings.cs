using System.Windows.Forms;

namespace Boostera
{
    public class Settings
    {
        public string TtermproPath { get; set; }
        public string TtpmacroPath { get; set; }
        public string WinscpPath { get; set; }
        public string SearchFolder { get; set; }
        public string SearchExclusionFolders { get; set; }
        public string BoosteraKeyPath { get; set; }
        public bool IsStartUp { get; set; }
        public int ModKey { get; set; }
        public Keys Key { get; set; }
        public bool IsSavePrivateKey { get; set; }
        public bool IsSavePassword { get; set; }
        public bool IsSaveForwardingPrivateKey { get; set; }
        public bool IsSaveForwardingPassword { get; set; }

        public Settings(string ttermproPath, string ttpmacroPath, string winscpPath, string searchFolder, string searchExclusionFolders, string boosteraKeyPath,
            bool isStartUp, int modKey, Keys key, bool isSavePrivateKey, bool isSavePassword, bool isSaveForwardingPrivateKey, bool isSaveForwardingPassword) 
        {
            TtermproPath = ttermproPath;
            TtpmacroPath = ttpmacroPath;
            WinscpPath = winscpPath;
            SearchFolder = searchFolder;
            SearchExclusionFolders = searchExclusionFolders;
            BoosteraKeyPath = boosteraKeyPath;
            IsStartUp = isStartUp;
            ModKey = modKey;
            Key = key;
            IsSavePrivateKey = isSavePrivateKey;
            IsSavePassword = isSavePassword;
            IsSaveForwardingPrivateKey = isSaveForwardingPrivateKey;
            IsSaveForwardingPassword = isSaveForwardingPassword;
        }
    }
}
