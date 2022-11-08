#if UNITY_EDITOR
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace Game.Editor
{
    public static class OpenInFileBrowser
    {
        private static void OpenInMac(string path)
        {
            var openInsidesOfFolder = false;

            // try mac
            var macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

            if (Directory.Exists(macPath)
            ) // if path requested is a folder, automatically open insides of that folder
            {
                openInsidesOfFolder = true;
            }

            if (!macPath.StartsWith("\""))
            {
                macPath = "\"" + macPath;
            }

            if (!macPath.EndsWith("\""))
            {
                macPath = macPath + "\"";
            }

            var arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;

            try
            {
                Process.Start("open", arguments);
            }
            catch (Win32Exception e)
            {
                // tried to open mac finder in windows
                // just silently skip error
                // we currently have no platform define for the current OS we are in, so we resort to this
                e.HelpLink = ""; // do anything with this variable to silence warning about not using it
            }
        }

        private static void OpenInWin(string path)
        {
            var openInsidesOfFolder = false;

            // try windows
            var winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

            if (Directory.Exists(winPath)
            ) // if path requested is a folder, automatically open insides of that folder
            {
                openInsidesOfFolder = true;
            }

            try
            {
                Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
            }
            catch (Win32Exception e)
            {
                // tried to open win explorer in mac
                // just silently skip error
                // we currently have no platform define for the current OS we are in, so we resort to this
                e.HelpLink = ""; // do anything with this variable to silence warning about not using it
            }
        }

        public static void Open(string path)
        {
            if (EditorUtility.IsInWinOS)
            {
                OpenInWin(path);
            }
            else if (EditorUtility.IsInMacOS)
            {
                OpenInMac(path);
            }
            else // couldn't determine OS
            {
                OpenInWin(path);
                OpenInMac(path);
            }
        }
    }
}
#endif