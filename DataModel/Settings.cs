using System.Windows.Forms;

namespace Boostera
{
    internal class Settings
    {
        internal string TtermproPath { get; set; }
        internal string TtpmacroPath { get; set; }
        internal string WinscpPath { get; set; }
        internal string SearchFolder { get; set; }
        internal string SearchExclusionFolders { get; set; }
        internal string BoosteraKeyPath { get; set; }
        internal bool IsLogging { get; set; }
        internal string LogFolder { get; set; }
        internal bool IsStartUp { get; set; }
        internal int ModKey { get; set; }
        internal Keys Key { get; set; }
        internal string TtlFileName { get; set; }

        internal Settings(string ttermproPath, string ttpmacroPath, string winscpPath, string boosteraKeyPath, string searchFolder,
            string searchExclusionFolders, bool isLogging, string logFolder, bool isStartUp, int modKey, Keys key, string ttlFileName)
        {
            TtermproPath = ttermproPath;
            TtpmacroPath = ttpmacroPath;
            WinscpPath = winscpPath;
            BoosteraKeyPath = boosteraKeyPath;
            SearchFolder = searchFolder;
            SearchExclusionFolders = searchExclusionFolders;
            IsLogging = isLogging;
            LogFolder = logFolder;
            IsStartUp = isStartUp;
            ModKey = modKey;
            Key = key;
            TtlFileName = ttlFileName;
        }
    }
}
