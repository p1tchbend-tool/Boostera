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

        public Settings(string ttermproPath, string ttpmacroPath, string winscpPath,
            string boosteraKeyPath, string searchFolder, string searchExclusionFolders, bool isStartUp, int modKey, Keys key) 
        {
            TtermproPath = ttermproPath;
            TtpmacroPath = ttpmacroPath;
            WinscpPath = winscpPath;
            BoosteraKeyPath = boosteraKeyPath;
            SearchFolder = searchFolder;
            SearchExclusionFolders = searchExclusionFolders;
            IsStartUp = isStartUp;
            ModKey = modKey;
            Key = key;
        }
    }
}
