using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class OpenSaveFileTool
    {
        [MenuItem("Tools/打开存档")]
        private static void OpenSaveFile()
        {
            var saveFilePath = Application.persistentDataPath + "/save.txt";
            if (File.Exists(saveFilePath))
            {
                System.Diagnostics.Process.Start(saveFilePath);
            }
        }

        [MenuItem("Tools/打开存档文件夹")]
        private static void OpenSaveFileFolder()
        {
            var saveFileDirectory = Application.persistentDataPath;
            if (Directory.Exists(saveFileDirectory))
            {
                System.Diagnostics.Process.Start(saveFileDirectory);
            }
        }
    }
}