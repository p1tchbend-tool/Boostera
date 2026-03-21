using System;
using System.IO;
using System.Windows.Forms;

namespace Boostera
{
    internal class Constants
    {
        internal class App
        {
            internal static readonly string SshFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh");

            internal static readonly string BoosteraDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Boostera");

            internal static readonly string BoosteraMacroFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".Boostera");

            internal static readonly string DefaultBoosteraKeyPath = Path.Combine(App.BoosteraDataFolder, "Boostera.Key");

            internal static readonly string DefaultTtermproPath = @"C:\Program Files\teraterm5\ttermpro.exe";
            internal static readonly string DefaultTtpmacroPath = @"C:\Program Files\teraterm5\ttpmacro.exe";
            internal static readonly string DefaultWinscpPath = @"C:\Program Files (x86)\WinSCP\WinSCP.exe";

            internal static readonly string DefaultSearchFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            internal static readonly string[] DefaultSearchExclusionFolders = {
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            };

            internal static readonly bool DefaultLogging = true;
            internal static readonly string DefaultLogFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".Boostera\log");

            internal static readonly bool DefaultStartUp = true;

            internal static readonly string DefaultTtlFileName = @"{{protocol}}_{{user}}@{{host}}{{istag:_}}{{tag}}";
        }

        internal class Log
        {
            internal static readonly Logger Logger = new Logger(
                Path.Combine(App.BoosteraDataFolder, "log"), "Boostera.log", 10000);
        }

        internal class Hoykey
        {
            internal static readonly HotKey ProgramHotKey = new HotKey();
            internal static readonly int HotKeyShowForm = 1;

            internal static readonly int DefaultHotModkey = HotKey.MOD_KEY_ALT;
            internal static readonly Keys DefaultHotKey = Keys.T;
        }
    }
}
