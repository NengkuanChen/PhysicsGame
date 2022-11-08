using System;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace Game.Editor
{
    public static class EditorUtility
    {
        public static bool IsInMacOS =>
            SystemInfo.operatingSystem.IndexOf("Mac OS", StringComparison.Ordinal) != -1;

        public static bool IsInWinOS =>
            SystemInfo.operatingSystem.IndexOf("Windows", StringComparison.Ordinal) != -1;
        
        public static string GetRegularPath(string path)
        {
            return path.Replace("\\", "/");
        }
        
        public static string GetProjectPath(string fullPath)
        {
            var dataPath = Application.dataPath;
            dataPath = GetRegularPath(dataPath);
            return $"Assets/{fullPath.Replace(dataPath, "")}";
        }
        
    }
}