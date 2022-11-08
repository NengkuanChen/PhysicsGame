using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Game.Editor.AssetBundle;
using Game.Editor.AssetProcess;
using Game.Editor.GameShader;
using Game.Editor.ResourceCheckTools;
using Game.Editor.UI;
using GameFramework;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;
using Debug = UnityEngine.Debug;

namespace Game.Editor.Build
{
    public static class BuildTools
    {
        public static int GetLatestSVNRevisionNumber()
        {
            if (EditorUtility.IsInWinOS)
            {
                var process = new Process();
                var startInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Application.dataPath,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = "/c svn info --show-item last-changed-revision",
                };
                process.StartInfo = startInfo;
                process.Start();

                var revision = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(revision))
                {
                    Debug.Log($"当前svn revision number: {revision}");
                }

                if (!string.IsNullOrEmpty(error))
                {
                    Debug.Log($"read svn revision err: {error}");
                }

                if (!int.TryParse(revision, out var result))
                {
                    throw new Exception($"解析SVN最新revision number出错, read: {revision}");
                }

                return result;
            }
            else
            {
                var process = new Process();
                var startInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Application.dataPath,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "/usr/local/bin/svn",
                    // CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = "info --show-item last-changed-revision"
                };
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                var revision = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(revision))
                {
                    Debug.Log($"当前svn revision number: {revision}");
                }

                if (!string.IsNullOrEmpty(error))
                {
                    Debug.Log($"read svn revision err: {error}");
                }

                if (!int.TryParse(revision, out var result))
                {
                    throw new Exception($"解析SVN最新revision number出错, read: {revision}");
                }

                return result;
            }
        }

        [MenuItem("打包/构建AssetBundle(Android)")]
        public static void BuildAndroidAssetBundles()
        {
            SpriteAtlasCreator.AutoAssign();
            MeshReadableCheck.AutoSetMeshAssetReadWriteOption();
            MaterialCheckTool.Run();
            ShaderCheckWindow.ReplaceUnityShaderToOurs();
            BuildAssetBundles(Platform.Android);
        }

        public static BuildResult BuildAndroid(string appName,
                                               string appVersion,
                                               int bundleVersion,
                                               out string outputFilePath)
        {
            var now = DateTime.Now;
            var dateFormat = $"{now.Year:0000}-{now.Month:00}{now.Day:00}-{now.Hour:00}{now.Minute:00}";

            var recommendApkFileName = $"{appName}_{appVersion}_{bundleVersion}_{dateFormat}.apk";
            outputFilePath = $"{Application.dataPath.Replace("Assets", "")}/Build/{recommendApkFileName}";
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] {"Assets/GameFramework.unity"},
                locationPathName = outputFilePath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };
            var result = BuildPipeline.BuildPlayer(buildPlayerOptions);
            return result.summary.result;
        }

        [MenuItem("打包/构建AssetBundle(IOS)")]
        public static void BuildIOSAssetBundles()
        {
            SpriteAtlasCreator.AutoAssign();
            MeshReadableCheck.AutoSetMeshAssetReadWriteOption();
            MaterialCheckTool.Run();
            ShaderCheckWindow.ReplaceUnityShaderToOurs();
            BuildAssetBundles(Platform.IOS);
        }

        public static BuildResult BuildIOS_XCode_Project(bool replaceMode, out string buildPath)
        {
            buildPath = $"{Application.dataPath.Replace("Assets", "")}/Build";
            var emptyOutputFolder =
                !Directory.Exists(buildPath) || !Directory.EnumerateFileSystemEntries(buildPath).Any();

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] {"Assets/GameFramework.unity"},
                locationPathName = buildPath,
                target = BuildTarget.iOS,
                options = (emptyOutputFolder || replaceMode)
                    ? BuildOptions.None
                    : BuildOptions.AcceptExternalModificationsToPlayer
            };
            var result = BuildPipeline.BuildPlayer(buildPlayerOptions);
            return result.summary.result;
        }

        private static void BuildAssetBundles(Platform buildPlatform)
        {
            if (buildPlatform != Platform.Android && buildPlatform != Platform.IOS)
            {
                Debug.LogError("只支持android和ios一键打包");
                return;
            }

            AssetBundleDeployer.AutoAssignAssetBundles();

            var assetBundleRootPath = $"{Application.dataPath.Replace("Assets", "")}AssetBundle";
            if (!Directory.Exists(assetBundleRootPath))
            {
                Directory.CreateDirectory(assetBundleRootPath);
            }

            var controller = new ResourceBuilderController();
            if (!controller.Load())
            {
                throw new GameFrameworkException("Load configuration failure.");
            }

            Debug.Log("Load configuration success.");

            controller.OutputDirectory = assetBundleRootPath;
            controller.BuildEventHandlerTypeName = "None";
            controller.Platforms = buildPlatform;
            controller.OnLoadingResource += (index, count) =>
            {
                var progress = (float) index / count;
                UnityEditor.EditorUtility.DisplayProgressBar("构建AssetBundle中", "loading resources", progress);
            };
            controller.OnLoadingAsset += (index, count) =>
            {
                var progress = (float) index / count;
                UnityEditor.EditorUtility.DisplayProgressBar("构建AssetBundle中", "loading assets", progress);
            };
            controller.OnAnalyzingAsset += (index, count) =>
            {
                var progress = (float) index / count;
                UnityEditor.EditorUtility.DisplayProgressBar("构建AssetBundle中", "analyzing assets", progress);
            };
            controller.ProcessingAssetBundle += (assetName, progress) =>
            {
                UnityEditor.EditorUtility.DisplayProgressBar("构建AssetBundle中",
                    $"process assets: {assetName}",
                    progress);
                return false;
            };
            controller.ProcessingBinary += (assetName, progress) =>
            {
                UnityEditor.EditorUtility.DisplayProgressBar("构建AssetBundle中",
                    $"process binaries: {assetName}",
                    progress);
                return false;
            };

            if (!controller.BuildResources())
            {
                throw new GameFrameworkException("Build resources failure.");
            }

            Debug.Log("Build resources success.");
            controller.Save();

            string platformABRootPath;
            if (buildPlatform == Platform.Android)
            {
                platformABRootPath = $"{controller.OutputPackagePath}/Android";
            }
            else
            {
                platformABRootPath = $"{controller.OutputPackagePath}/IOS";
            }

            var streamingAssetPath = $"{Application.dataPath}/StreamingAssets";
            if (!Directory.Exists(streamingAssetPath))
            {
                Directory.CreateDirectory(streamingAssetPath);
            }
            else
            {
                //clear streaming asset folder
                var streamingAssetDirectoryInfo = new DirectoryInfo(streamingAssetPath);
                streamingAssetDirectoryInfo.Delete(true);
            }

            //copy files
            Debug.Log($"copy asset bundle files from {platformABRootPath} to {streamingAssetPath}");

            SyncDirectoryFiles(platformABRootPath,
                streamingAssetPath,
                copyFile =>
                {
                    UnityEditor.EditorUtility.DisplayProgressBar("构建AssetBundle中", $"copy file: {copyFile}", 1f);
                });
            Debug.Log("构建AssetBundle完成");
            UnityEditor.EditorUtility.ClearProgressBar();

            AssetDatabase.Refresh();
        }

        private static void SyncDirectoryFiles(string srcPath, string dstPath, Action<string> onCopy)
        {
            if (!Directory.Exists(srcPath))
            {
                Debug.LogError($"src path is not exist: {srcPath}");
                return;
            }

            if (!Directory.Exists(dstPath))
            {
                Directory.CreateDirectory(dstPath);
            }

            var allFiles = Directory.GetFiles(srcPath);
            foreach (var file in allFiles)
            {
                onCopy?.Invoke(file);
                File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
            }

            var allDirectories = Directory.GetDirectories(srcPath);
            foreach (var directory in allDirectories)
            {
                SyncDirectoryFiles(directory, Path.Combine(dstPath, Path.GetFileName(directory)), onCopy);
            }
        }
    }
}