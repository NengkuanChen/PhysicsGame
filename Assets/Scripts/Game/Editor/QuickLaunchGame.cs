using System.Collections.Generic;
using Game.GameSystem;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

namespace Game.Editor
{
    [InitializeOnLoad]
    public static class QuickLaunchGame
    {
        private const string NeedRestoreSceneSaveKey = "Editor_NeedRestoreSceneAssetPath";

        private static int entrySystemNameIndex;

        static QuickLaunchGame()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            // ToolbarExtender.LeftToolbarGUI.Add(OnToolBarGUILeft);
            ToolbarExtender.RightToolbarGUI.Add(OnToolBarGUIRight);
            entrySystemNameIndex = PlayerPrefs.GetInt(EntrySystemConfig.SystemNameIndexSaveKey);
        }

        private static void OnToolBarGUILeft()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            entrySystemNameIndex =
                EditorGUILayout.Popup(entrySystemNameIndex, EntrySystemConfig.EntrySystemNames, GUILayout.Width(100));
            if (EditorGUI.EndChangeCheck())
            {
                PlayerPrefs.SetInt(EntrySystemConfig.SystemNameIndexSaveKey, entrySystemNameIndex);
            }

            GUILayout.Space(20);
        }

        private static void OnToolBarGUIRight()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            if (GUILayout.Button(new GUIContent("启动游戏", "从当前场景快速启动游戏,停止游戏后恢复到当前场景"), GUILayout.MaxWidth(100)))
            {
                LaunchGame();
            }
        }

        private static void LaunchGame()
        {
            if (EditorApplication.isCompiling || EditorApplication.isPlaying || EditorApplication.isPaused)
            {
                return;
            }
            
            DeployAllScenes();

            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.name == "GameFramework")
            {
                //直接启动
                EditorApplication.isPlaying = true;
            }
            else
            {
                PlayerPrefs.SetString(NeedRestoreSceneSaveKey, activeScene.path);
                EditorSceneManager.OpenScene("Assets/GameFramework.unity");
                EditorApplication.isPlaying = true;
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                var needRestoreSceneAssetPath = PlayerPrefs.GetString(NeedRestoreSceneSaveKey);
                if (!string.IsNullOrEmpty(needRestoreSceneAssetPath))
                {
                    EditorSceneManager.OpenScene(needRestoreSceneAssetPath);
                    PlayerPrefs.DeleteKey(NeedRestoreSceneSaveKey);
                }
            }
            else if (state == PlayModeStateChange.EnteredPlayMode)
            {
                // stayAtSceneViewWhenPlayGame = PlayerPrefs.GetInt(StayAtSceneViewSaveKey) == 1;
                // if (stayAtSceneViewWhenPlayGame)
                // {
                //     EditorWindow.FocusWindowIfItsOpen(typeof(SceneView));
                //     Debug.Log("focus scene view");
                // }
            }
        }
        
        private static void DeployAllScenes()
        {
            var sceneNames = new HashSet<string> {"Assets/GameFramework.unity"};
            var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] {"Assets/Game/Scene"});
            foreach (var sceneGuid in sceneGuids)
            {
                var sceneName = AssetDatabase.GUIDToAssetPath(sceneGuid);
                sceneNames.Add(sceneName);
            }

            var scenes = new List<EditorBuildSettingsScene>();
            foreach (var sceneName in sceneNames)
            {
                scenes.Add(new EditorBuildSettingsScene(sceneName, true));
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}