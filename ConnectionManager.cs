using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Boostera
{
    public class ConnectionManager
    {
        public async Task Connect(string ttermproPath, string winscpPath, bool isLogging, string logFolder, string protocolText, string host, string user, string port, string privateKey,
            string password, bool isEnvPassword, string logonScript, string waitingString, string waitingTime, bool isForwarding, string forwardingHost, string forwardingUser,
            string forwardingPort, string forwardingLocalPort, string forwardingPrivateKey, string forwardingPassword, bool forwardingIsEnvPassword, bool isHide, string tag)
        {
            if (isEnvPassword) password = Environment.GetEnvironmentVariable(password, EnvironmentVariableTarget.User);
            if (forwardingIsEnvPassword) forwardingPassword = Environment.GetEnvironmentVariable(forwardingPassword, EnvironmentVariableTarget.User);

            if (isForwarding)
            {
                var arguments = forwardingHost + ":" + forwardingPort + " /ssh2";
                if (isHide)
                {
                    arguments += " /V";
                }
                else
                {
                    arguments += " /I";
                }
                arguments += " /ssh-L" + forwardingLocalPort + ":" + host + ":" + port;

                if (!string.IsNullOrEmpty(forwardingPrivateKey))
                {
                    arguments += " /auth=publickey /user=" + forwardingUser + " /keyfile=\"" + forwardingPrivateKey + "\"";
                }
                else
                {
                    arguments += " /auth=password /user=" + forwardingUser;
                }
                if (!string.IsNullOrEmpty(forwardingPassword)) arguments += " /passwd=\"" + forwardingPassword + "\"";

                var windowTitle = forwardingUser + "@" + forwardingHost;
                arguments += " /W=" + windowTitle;

                var psi = new ProcessStartInfo(ttermproPath);
                psi.UseShellExecute = true;
                psi.Arguments = arguments;
                Process.Start(psi);
                await Task.Delay(3000);
            }

            if (protocolText == "SSH")
            {
                var arguments = string.Empty;
                if (isForwarding)
                {
                    arguments = "localhost:" + forwardingLocalPort + " /nosecuritywarning /ssh2";  // トンネリング時に限り検証無効
                }
                else
                {
                    arguments = host + ":" + port + " /ssh2";
                }

                if (!string.IsNullOrEmpty(privateKey))
                {
                    arguments += " /auth=publickey /user=" + user + " /keyfile=\"" + privateKey + "\"";
                }
                else
                {
                    arguments += " /auth=password /user=" + user;
                }
                if (!string.IsNullOrEmpty(password)) arguments += " /passwd=\"" + password + "\"";

                if (isLogging || !string.IsNullOrEmpty(logonScript))
                {
                    if (!Directory.Exists(Path.Combine(Program.BoosteraDataFolder, ".temp")))
                        Directory.CreateDirectory(Path.Combine(Program.BoosteraDataFolder, ".temp"));

                    var tempTtlPath = Path.Combine(Program.BoosteraDataFolder, ".temp\\logon.ttl");
                    arguments += " /M=\"" + tempTtlPath + "\"";

                    var script = string.Empty;

                    if (isLogging)
                    {
                        var logFile = Path.Combine(logFolder, "%Y%m%d-%H%M%S_" + user + "@" + host + "_#" + tag + ".log");
                        script += $@"logclose
logopen '{logFile}' 0 1 1 1 1 1 0
";
                    }

                    if (!string.IsNullOrEmpty(logonScript))
                    {
                        script += $@"wait '{waitingString}'
mpause {waitingTime}
sendln '{logonScript}'
";
                    }

                    script += $@"filedelete '{tempTtlPath}'";
                    File.WriteAllText(tempTtlPath, script);
                }

                var windowTitle = user + "@" + host;
                arguments += " /W=" + windowTitle;

                var psi = new ProcessStartInfo(ttermproPath);
                psi.UseShellExecute = true;
                psi.Arguments = arguments;
                Process.Start(psi);
            }
            else if (protocolText == "RDP")
            {
                var arguments1 = string.Empty;
                var arguments2 = string.Empty;

                if (isForwarding)
                {
                    arguments1 = "/generic:TERMSRV/localhost /user:" + user + " /pass:" + password;
                    arguments2 = "/v:localhost:" + forwardingLocalPort;
                }
                else
                {
                    arguments1 = "/generic:TERMSRV/" + host + " /user:" + user + " /pass:" + password;
                    arguments2 = "/v:" + host + ":" + port;
                }

                var psi1 = new ProcessStartInfo("cmdkey");
                psi1.UseShellExecute = true;
                psi1.WindowStyle = ProcessWindowStyle.Minimized;
                psi1.Arguments = arguments1;
                var process = Process.Start(psi1);
                process.WaitForExit();

                var psi2 = new ProcessStartInfo("mstsc");
                psi2.UseShellExecute = true;
                psi2.Arguments = arguments2;
                Process.Start(psi2);
            }
            else if (protocolText == "SFTP")
            {
                var arguments = string.Empty;

                if (isForwarding)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        arguments = "sftp://" + user + ":" + password + "@localhost:" + forwardingLocalPort;
                    }
                    else
                    {
                        arguments = "sftp://" + user + "@localhost:" + forwardingLocalPort;
                    }
                    if (!string.IsNullOrEmpty(privateKey)) arguments += " /privatekey=\"" + privateKey + "\"";

                    arguments += " /hostkey=\"*\"";  // トンネリング時に限り検証無効
                }
                else
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        arguments = "sftp://" + user + ":" + password + "@" + host + ":" + port;
                    }
                    else
                    {
                        arguments = "sftp://" + user + "@" + host + ":" + port;
                    }
                    if (!string.IsNullOrEmpty(privateKey)) arguments += " /privatekey=\"" + privateKey + "\"";
                }

                arguments += " /sessionname=" + user + "@" + host;

                var psi = new ProcessStartInfo(winscpPath);
                psi.UseShellExecute = true;
                psi.Arguments = arguments;
                Process.Start(psi);
            }
        }

        public string ExportTtl(string targetFolder, string protocolText, string host, string user, string port, string privateKey, string password, bool isEnvPassword,
            string logonScript, string waitingString, string waitingTime, bool isForwarding, string forwardingHost, string forwardingUser, string forwardingPort,
            string forwardingLocalPort, string forwardingPrivateKey, string forwardingPassword, bool forwardingIsEnvPassword, bool isHide, string tag)
        {
            var ttl = TTL_TEMPLATE.Replace("{{Protocol}}", protocolText);
            ttl = ttl.Replace("{{Host}}", host);
            ttl = ttl.Replace("{{User}}", user);
            ttl = ttl.Replace("{{Port}}", port);
            ttl = ttl.Replace("{{PrivateKey}}", privateKey.Replace(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "%USERPROFILE%"));
            ttl = ttl.Replace("{{Password}}", password);
            ttl = ttl.Replace("{{IsEnvPassword}}", isEnvPassword.ToString().ToLower());
            ttl = ttl.Replace("{{LogonScript}}", logonScript);
            ttl = ttl.Replace("{{WaitingString}}", waitingString);
            ttl = ttl.Replace("{{WaitingTime}}", waitingTime);
            ttl = ttl.Replace("{{IsForwarding}}", isForwarding.ToString().ToLower());
            ttl = ttl.Replace("{{ForwardingHost}}", forwardingHost);
            ttl = ttl.Replace("{{ForwardingUser}}", forwardingUser);
            ttl = ttl.Replace("{{ForwardingPort}}", forwardingPort);
            ttl = ttl.Replace("{{ForwardingLocalPort}}", forwardingLocalPort);
            ttl = ttl.Replace("{{ForwardingPrivateKey}}", forwardingPrivateKey.Replace(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "%USERPROFILE%"));
            ttl = ttl.Replace("{{ForwardingPassword}}", forwardingPassword);
            ttl = ttl.Replace("{{ForwardingIsEnvPassword}}", forwardingIsEnvPassword.ToString().ToLower());
            ttl = ttl.Replace("{{IsHide}}", isHide.ToString().ToLower());
            ttl = ttl.Replace("{{Tag}}", tag);

            var ttlFileName = protocolText + "_" + user + "@" + host;
            if (!string.IsNullOrEmpty(tag)) ttlFileName += "_" + tag;
            ttlFileName = Regex.Replace(ttlFileName, @"[<>:""/\\|?*]", "");
            ttlFileName = ttlFileName.ToLower() + ".ttl";

            File.WriteAllText(Path.Combine(targetFolder, ttlFileName), ttl);
            return Path.Combine(targetFolder, ttlFileName);
        }

        public Dictionary<string, string> ImportTtl(string ttlFilePath)
        {
            var ttl = File.ReadAllText(ttlFilePath);
            var dict = new Dictionary<string, string>();

            dict.Add("Protocol", RegexMatchedGroupText(ttl, @"^Protocol\s+=\s+'(.*?)'"));
            dict.Add("Host", RegexMatchedGroupText(ttl, @"^Host\s+=\s+'(.*?)'"));
            dict.Add("User", RegexMatchedGroupText(ttl, @"^User\s+=\s+'(.*?)'"));
            dict.Add("Port", RegexMatchedGroupText(ttl, @"^Port\s+=\s+'(.*?)'"));
            dict.Add("PrivateKey", RegexMatchedGroupText(ttl, @"^PrivateKey\s+=\s+'(.*?)'")
                .Replace("%USERPROFILE%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
            dict.Add("Password", RegexMatchedGroupText(ttl, @"^Password\s+=\s+'(.*?)'"));
            dict.Add("IsEnvPassword", RegexMatchedGroupText(ttl, @"^IsEnvPassword\s+=\s+'(.*?)'"));
            dict.Add("LogonScript", RegexMatchedGroupText(ttl, @"^LogonScript\s+=\s+'(.*?)'"));
            dict.Add("WaitingString", RegexMatchedGroupText(ttl, @"^WaitingString\s+=\s+'(.*?)'"));
            dict.Add("WaitingTime", RegexMatchedGroupText(ttl, @"^WaitingTime\s+=\s+(\d+)"));
            dict.Add("IsForwarding", RegexMatchedGroupText(ttl, @"^IsForwarding\s+=\s+'(.*?)'"));
            dict.Add("ForwardingHost", RegexMatchedGroupText(ttl, @"^ForwardingHost\s+=\s+'(.*?)'"));
            dict.Add("ForwardingUser", RegexMatchedGroupText(ttl, @"^ForwardingUser\s+=\s+'(.*?)'"));
            dict.Add("ForwardingPort", RegexMatchedGroupText(ttl, @"^ForwardingPort\s+=\s+'(.*?)'"));
            dict.Add("ForwardingPrivateKey", RegexMatchedGroupText(ttl, @"^ForwardingPrivateKey\s+=\s+'(.*?)'")
                .Replace("%USERPROFILE%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
            dict.Add("ForwardingPassword", RegexMatchedGroupText(ttl, @"^ForwardingPassword\s+=\s+'(.*?)'"));
            dict.Add("ForwardingIsEnvPassword", RegexMatchedGroupText(ttl, @"^ForwardingIsEnvPassword\s+=\s+'(.*?)'"));
            dict.Add("IsHide", RegexMatchedGroupText(ttl, @"^IsHide\s+=\s+'(.*?)'"));
            dict.Add("Tag", RegexMatchedGroupText(ttl, @"^Tag\s+=\s+'(.*?)'"));

            return dict;
        }

        private string RegexMatchedGroupText(string text, string pattern)
        {
            var matchedGroupText = string.Empty;
            try
            {
                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                matchedGroupText = match.Groups[1].Value;
            }
            catch { }
            return matchedGroupText;
        }

        private static readonly string TTL_TEMPLATE = @"Protocol = '{{Protocol}}'
Host = '{{Host}}'
User = '{{User}}'
Port = '{{Port}}'
PrivateKey = '{{PrivateKey}}'
Password = '{{Password}}'
IsEnvPassword = '{{IsEnvPassword}}'
LogonScript = '{{LogonScript}}'
WaitingString = '{{WaitingString}}'
WaitingTime = {{WaitingTime}}
IsForwarding = '{{IsForwarding}}'
ForwardingHost = '{{ForwardingHost}}'
ForwardingUser = '{{ForwardingUser}}'
ForwardingPort = '{{ForwardingPort}}'
ForwardingLocalPort = '{{ForwardingLocalPort}}'
ForwardingPrivateKey = '{{ForwardingPrivateKey}}'
ForwardingPassword = '{{ForwardingPassword}}'
ForwardingIsEnvPassword = '{{ForwardingIsEnvPassword}}'
IsHide = '{{IsHide}}'
Tag = '{{Tag}}'

getenv 'BOOSTERA_IsLogging' IsLogging
getenv 'BOOSTERA_LogFolder' LogFolder
getenv 'BOOSTERA_WinscpPath' WinscpPath

expandenv PrivateKey
expandenv ForwardingPrivateKey

strcompare IsEnvPassword 'true'
if result == 0 then
    getenv Password Password
endif

strcompare ForwardingIsEnvPassword 'true'
if result == 0 then
    getenv ForwardingPassword ForwardingPassword
endif

buf = ''
strcompare IsForwarding 'true'
if result == 0 then
    strconcat buf ForwardingHost
    strconcat buf ':'
    strconcat buf ForwardingPort
    strconcat buf ' /ssh2'

    strcompare IsHide 'true'
    if result == 0 then
        strconcat buf ' /V'
    else
        strconcat buf ' /I'
    endif

    strconcat buf ' /ssh-L'
    strconcat buf forwardingLocalPort
    strconcat buf ':'
    strconcat buf Host
    strconcat buf ':'
    strconcat buf Port

    strcompare ForwardingPrivateKey ''
    if result != 0 then
        strconcat buf ' /auth=publickey /user='
        strconcat buf ForwardingUser
        strconcat buf ' /keyfile=""'
        strconcat buf ForwardingPrivateKey
        strconcat buf '""'
    else
        strconcat buf ' /auth=password /user='
        strconcat buf ForwardingUser
    endif

    strcompare ForwardingPassword ''
    if result != 0 then
        strconcat buf ' /passwd=""'
        strconcat buf ForwardingPassword
        strconcat buf '""'
    endif

    strconcat buf ' /W='
    strconcat buf ForwardingUser
    strconcat buf '@'
    strconcat buf ForwardingHost

    connect buf
    wait ''
    mpause 3000
    unlink
endif

buf = ''
strcompare Protocol 'SSH'
if result == 0 then

    strcompare IsForwarding 'true'
    if result == 0 then
        strconcat buf 'localhost:'
        strconcat buf forwardingLocalPort
        strconcat buf ' /nosecuritywarning /ssh2'
    else
        strconcat buf Host
        strconcat buf ':'
        strconcat buf Port
        strconcat buf ' /ssh2'
    endif

    strcompare PrivateKey ''
    if result != 0 then
        strconcat buf ' /auth=publickey /user='
        strconcat buf User
        strconcat buf ' /keyfile=""'
        strconcat buf PrivateKey
        strconcat buf '""'
    else
        strconcat buf ' /auth=password /user='
        strconcat buf User
    endif

    strcompare Password ''
    if result != 0 then
        strconcat buf ' /passwd=""'
        strconcat buf Password
        strconcat buf '""'
    endif

    strconcat buf ' /W='
    strconcat buf User
    strconcat buf '@'
    strconcat buf Host

    connect buf

    strcompare IsLogging 'true'
    if result == 0 then
        logFile = ''
        strconcat logFile LogFolder
        strconcat logFile '\'
        strconcat logFile '%Y%m%d-%H%M%S_'
        strconcat logFile User
        strconcat logFile '@'
        strconcat logFile Host
        strconcat logFile '_#'
        strconcat logFile Tag
        strconcat logFile '.log'

        logclose
        logopen logFile 0 1 1 1 1 1 0
    endif

    strcompare LogonScript ''
    if result != 0 then
        wait WaitingString
        mpause WaitingTime
        sendln LogonScript
    endif
endif

strcompare Protocol 'RDP'
if result == 0 then
    strconcat buf 'cmdkey'

    strcompare IsForwarding 'true'
    if result == 0 then
        strconcat buf ' /generic:TERMSRV/localhost /user:'
        strconcat buf User
        strconcat buf ' /pass:'
        strconcat buf Password
    else
        strconcat buf ' /generic:TERMSRV/'
        strconcat buf Host
        strconcat buf ' /user:'
        strconcat buf User
        strconcat buf ' /pass:'
        strconcat buf Password
    endif

    exec buf 'minimize' 1

    buf = ''
    strconcat buf 'mstsc'

    strcompare IsForwarding 'true'
    if result == 0 then
        strconcat buf ' /v:localhost:'
        strconcat buf forwardingLocalPort
    else
        strconcat buf ' /v:'
        strconcat buf Host
        strconcat buf ':'
        strconcat buf Port
    endif

    exec buf
endif

strcompare Protocol 'SFTP'
if result == 0 then
    strconcat buf WinscpPath

    strcompare IsForwarding 'true'
    if result == 0 then

        strcompare Password ''
        if result != 0 then
            strconcat buf ' sftp://'
            strconcat buf User
            strconcat buf ':'
            strconcat buf Password
            strconcat buf '@localhost:'
            strconcat buf forwardingLocalPort
        else
            strconcat buf ' sftp://'
            strconcat buf User
            strconcat buf '@localhost:'
            strconcat buf forwardingLocalPort
        endif

        strcompare PrivateKey ''
        if result != 0 then
            strconcat buf ' /privatekey=""'
            strconcat buf PrivateKey
            strconcat buf '""'
        endif

        strconcat buf ' /hostkey=""*""'
    else
        strcompare Password ''
        if result != 0 then
            strconcat buf ' sftp://'
            strconcat buf User
            strconcat buf ':'
            strconcat buf Password
            strconcat buf '@'
            strconcat buf Host
            strconcat buf ':'
            strconcat buf Port
        else
            strconcat buf ' sftp://'
            strconcat buf User
            strconcat buf '@'
            strconcat buf Host
            strconcat buf ':'
            strconcat buf Port
        endif

        strcompare PrivateKey ''
        if result != 0 then
            strconcat buf ' /privatekey=""'
            strconcat buf PrivateKey
            strconcat buf '""'
        endif
    endif

    strconcat buf ' /sessionname='
    strconcat buf User
    strconcat buf '@'
    strconcat buf Host

    exec buf
endif
";
    }
}
