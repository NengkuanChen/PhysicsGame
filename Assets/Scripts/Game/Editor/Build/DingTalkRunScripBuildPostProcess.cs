#if UNITY_EDITOR_OSX
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Game.Editor.Build
{
    public static class DingTalkRunScriptBuildPostProcess
    {
        [PostProcessBuild(int.MaxValue - 1)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }

            if (PlayerPrefs.GetInt(DingTalkConstant.EnableDingTalkSaveKey) == 0)
            {
                return;
            }

            var sendMsgCommand =
                $"curl '{DingTalkConstant.HookURL}' -H 'Content-Type: application/json' -d '{{\"msgtype\":\"text\",\"text\":{{\"content\":\"通知\\nXCode工程构建完成\"}}}}'";

            var projPath = PBXProject.GetPBXProjectPath(buildPath);
            Debug.Log($"xcode 工程路径. build path {buildPath}, project path {projPath}");
            var pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projPath);
            pbxProject.InsertShellScriptBuildPhase(int.MaxValue,
                pbxProject.GetUnityFrameworkTargetGuid(),
                "DingTalkInform",
                "/bin/sh",
                sendMsgCommand);
            pbxProject.WriteToFile(projPath);
        }
    }
}
#endif