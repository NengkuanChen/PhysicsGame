﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
#if UNITY_2021_1_OR_NEWER
using UnityEngine.UIElements;

#endif

namespace UnityToolbarExtender
{
    [InitializeOnLoad]
    public static class ToolbarExtender
    {
        static int m_toolCount;
        static GUIStyle m_commandStyle = null;

        public static readonly List<Action> LeftToolbarGUI = new List<Action>();
        public static readonly List<Action> RightToolbarGUI = new List<Action>();

    #if UNITY_2021_1_OR_NEWER
        static ScriptableObject currentTooBar;
        static Type TooBarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        static VisualElement leftTooBarParent;
        static VisualElement rightTooBarParent;
        static int currentTooBarInstanceID;
    #endif

        static ToolbarExtender()
        {
        #if UNITY_2021_1_OR_NEWER
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        #else
            Type toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");

        #if UNITY_2019_1_OR_NEWER
            string fieldName = "k_ToolCount";
        #else
			string fieldName = "s_ShownToolIcons";
        #endif

            FieldInfo toolIcons = toolbarType.GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        #if UNITY_2019_3_OR_NEWER
            m_toolCount = toolIcons != null ? ((int) toolIcons.GetValue(null)) : 8;
        #elif UNITY_2019_1_OR_NEWER
			m_toolCount = toolIcons != null ? ((int) toolIcons.GetValue(null)) : 7;
        #elif UNITY_2018_1_OR_NEWER
			m_toolCount = toolIcons != null ? ((Array) toolIcons.GetValue(null)).Length : 6;
        #else
			m_toolCount = toolIcons != null ? ((Array) toolIcons.GetValue(null)).Length : 5;
        #endif

            ToolbarCallback.OnToolbarGUI -= OnGUI;
            ToolbarCallback.OnToolbarGUI += OnGUI;
        #endif
        }

        private static void OnUpdate()
        {
            if (currentTooBar == null)
            {
                var toolbars = Resources.FindObjectsOfTypeAll(TooBarType);
                currentTooBar = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;
            }

            var reCreated = false;
            if (currentTooBar != null && currentTooBar.GetInstanceID() != currentTooBarInstanceID)
            {
                if (leftTooBarParent != null)
                {
                    leftTooBarParent.RemoveFromHierarchy();
                    leftTooBarParent = null;
                }

                if (rightTooBarParent != null)
                {
                    rightTooBarParent.RemoveFromHierarchy();
                    rightTooBarParent = null;
                }

                currentTooBarInstanceID = currentTooBar.GetInstanceID();
                reCreated = true;
            }

            if (currentTooBar != null && reCreated)
            {
                var root = currentTooBar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                if (root != null)
                {
                    var rawRoot = root.GetValue(currentTooBar);
                    if (rawRoot != null)
                    {
                        var mRoot = rawRoot as VisualElement;

                        //left
                        var toolbarZoneLeftAlign = mRoot.Q("ToolbarZoneLeftAlign");
                        leftTooBarParent = new VisualElement()
                        {
                            style =
                            {
                                flexGrow = 1,
                                flexDirection = FlexDirection.Row,
                            }
                        };
                        leftTooBarParent.Add(new VisualElement()
                        {
                            style =
                            {
                                flexGrow = 1,
                            }
                        });
                        leftTooBarParent.Add(new IMGUIContainer(() =>
                        {
                            if (LeftToolbarGUI != null)
                            {
                                foreach (var action in LeftToolbarGUI)
                                {
                                    action.Invoke();
                                }
                            }
                        }));
                        toolbarZoneLeftAlign.Add(leftTooBarParent);

                        //right
                        var toolbarZoneRightAlign = mRoot.Q("ToolbarZoneRightAlign");
                        rightTooBarParent = new VisualElement()
                        {
                            style =
                            {
                                flexGrow = 1,
                                flexDirection = FlexDirection.Row,
                            }
                        };
                        rightTooBarParent.Add(new IMGUIContainer(() =>
                        {
                            if (RightToolbarGUI != null)
                            {
                                foreach (var action in RightToolbarGUI)
                                {
                                    action.Invoke();
                                }
                            }
                        }));
                        rightTooBarParent.Add(new VisualElement()
                        {
                            style =
                            {
                                flexGrow = 1,
                            }
                        });
                        
                        toolbarZoneRightAlign.Add(rightTooBarParent);
                    }
                }
            }
        }

    #if UNITY_2019_3_OR_NEWER
        public const float space = 8;
    #else
		public const float space = 10;
    #endif
        public const float largeSpace = 20;
        public const float buttonWidth = 32;
        public const float dropdownWidth = 80;
    #if UNITY_2019_1_OR_NEWER
        public const float playPauseStopWidth = 140;
    #else
		public const float playPauseStopWidth = 100;
    #endif

        static void OnGUI()
        {
            // Create two containers, left and right
            // Screen is whole toolbar

            if (m_commandStyle == null)
            {
                m_commandStyle = new GUIStyle("CommandLeft");
            }

            var screenWidth = EditorGUIUtility.currentViewWidth;

            // Following calculations match code reflected from Toolbar.OldOnGUI()
            float playButtonsPosition = Mathf.RoundToInt((screenWidth - playPauseStopWidth) / 2);

            Rect leftRect = new Rect(0, 0, screenWidth, Screen.height);
            leftRect.xMin += space;                     // Spacing left
            leftRect.xMin += buttonWidth * m_toolCount; // Tool buttons
        #if UNITY_2019_3_OR_NEWER
            leftRect.xMin += space; // Spacing between tools and pivot
        #else
			leftRect.xMin += largeSpace; // Spacing between tools and pivot
        #endif
            leftRect.xMin += 64 * 2; // Pivot buttons
            leftRect.xMax = playButtonsPosition;

            Rect rightRect = new Rect(0, 0, screenWidth, Screen.height);
            rightRect.xMin = playButtonsPosition;
            rightRect.xMin += m_commandStyle.fixedWidth * 3; // Play buttons
            rightRect.xMax = screenWidth;
            rightRect.xMax -= space;         // Spacing right
            rightRect.xMax -= dropdownWidth; // Layout
            rightRect.xMax -= space;         // Spacing between layout and layers
            rightRect.xMax -= dropdownWidth; // Layers
        #if UNITY_2019_3_OR_NEWER
            rightRect.xMax -= space; // Spacing between layers and account
        #else
			rightRect.xMax -= largeSpace; // Spacing between layers and account
        #endif
            rightRect.xMax -= dropdownWidth; // Account
            rightRect.xMax -= space;         // Spacing between account and cloud
            rightRect.xMax -= buttonWidth;   // Cloud
            rightRect.xMax -= space;         // Spacing between cloud and collab
            rightRect.xMax -= 78;            // Colab

            // Add spacing around existing controls
            leftRect.xMin += space;
            leftRect.xMax -= space;
            rightRect.xMin += space;
            rightRect.xMax -= space;

            // Add top and bottom margins
        #if UNITY_2019_3_OR_NEWER
            leftRect.y = 4;
            leftRect.height = 22;
            rightRect.y = 4;
            rightRect.height = 22;
        #else
			leftRect.y = 5;
			leftRect.height = 24;
			rightRect.y = 5;
			rightRect.height = 24;
        #endif

            if (leftRect.width > 0)
            {
                GUILayout.BeginArea(leftRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in LeftToolbarGUI)
                {
                    handler();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            if (rightRect.width > 0)
            {
                GUILayout.BeginArea(rightRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in RightToolbarGUI)
                {
                    handler();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }
    }
}