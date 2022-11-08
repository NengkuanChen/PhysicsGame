using System;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Editor.UI
{
    public static class ReplaceTextWithTextMeshProUGUI
    {
        [MenuItem("Tools/UI/将UI中的text替换为TextMeshProUGUI")]
        public static void ReplaceTextToTextMeshProUGUI()
        {
            var currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (currentPrefabStage == null)
            {
                Debug.LogError("需要在UI预制编辑界面中使用");
                return;
            }

            var uiRootGameObject = currentPrefabStage.prefabContentsRoot;
            var textComponents = uiRootGameObject.GetComponentsInChildren<Text>();
            foreach (var textComponent in textComponents)
            {
                Debug.Log($"替换text组件： {textComponent.gameObject.name}");
                var fontSize = textComponent.fontSize;
                var text = textComponent.text;
                var textColor = textComponent.color;
                var textGameObject = textComponent.gameObject;
                var bestFit = textComponent.resizeTextForBestFit;
                var sizeDelta = textComponent.rectTransform.sizeDelta;
                var textAlignment = textComponent.alignment;
                var fontName = textComponent.font.name;

                Color? outlineColor = null;
                Color? shadowColor = null;
                var outline = textComponent.GetComponent<Outline>();
                if (outline != null)
                {
                    outlineColor = outline.effectColor;
                    Object.DestroyImmediate(outline);
                }

                var shadow = textComponent.GetComponent<Shadow>();
                if (shadow != null)
                {
                    shadowColor = shadow.effectColor;
                    Object.DestroyImmediate(shadow);
                }

                Object.DestroyImmediate(textComponent);

                var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"Assets/Game/UI/Font/{fontName}.asset");
                var textMeshProUGUI = textGameObject.GetOrAddComponent<TextMeshProUGUI>();
                textMeshProUGUI.font = fontAsset;
                textMeshProUGUI.fontSize = fontSize;
                textMeshProUGUI.enableAutoSizing = bestFit;
                textMeshProUGUI.autoSizeTextContainer = false;
                textMeshProUGUI.text = text;
                textMeshProUGUI.color = textColor;
                textMeshProUGUI.rectTransform.sizeDelta = sizeDelta;
                textMeshProUGUI.fontSizeMax = 200;
                textMeshProUGUI.fontSizeMin = 1;
                TextAlignmentOptions proTextAlignment;
                switch (textAlignment)
                {
                    case TextAnchor.UpperLeft:
                        proTextAlignment = TextAlignmentOptions.TopLeft;
                        break;
                    case TextAnchor.UpperCenter:
                        proTextAlignment = TextAlignmentOptions.Center;
                        break;
                    case TextAnchor.UpperRight:
                        proTextAlignment = TextAlignmentOptions.TopRight;
                        break;
                    case TextAnchor.MiddleLeft:
                        proTextAlignment = TextAlignmentOptions.MidlineLeft;
                        break;
                    case TextAnchor.MiddleCenter:
                        proTextAlignment = TextAlignmentOptions.Midline;
                        break;
                    case TextAnchor.MiddleRight:
                        proTextAlignment = TextAlignmentOptions.MidlineRight;
                        break;
                    case TextAnchor.LowerLeft:
                        proTextAlignment = TextAlignmentOptions.BottomLeft;
                        break;
                    case TextAnchor.LowerCenter:
                        proTextAlignment = TextAlignmentOptions.Bottom;
                        break;
                    case TextAnchor.LowerRight:
                        proTextAlignment = TextAlignmentOptions.BottomRight;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                textMeshProUGUI.alignment = proTextAlignment;
                textMeshProUGUI.raycastTarget = false;

                //load or create material
                var fontMaterial = LoadOrCreateFontMaterial(fontAsset, outlineColor, shadowColor);
                textMeshProUGUI.fontSharedMaterial = fontMaterial;
            }

            AssetDatabase.Refresh();
            EditorSceneManager.MarkSceneDirty(currentPrefabStage.scene);
        }

        private static Material LoadOrCreateFontMaterial(TMP_FontAsset fontAsset,
                                                         Color? outlineColor,
                                                         Color? shadowColor)
        {
            if (outlineColor == null && shadowColor == null)
            {
                return fontAsset.material;
            }

            const string materialFolder = "Assets/Game/UI/Font/Materials";
            var requiresMaterialName = fontAsset.name;
            if (outlineColor != null)
            {
                requiresMaterialName += $"_outline_{ColorUtility.ToHtmlStringRGBA(outlineColor.Value)}";
            }

            if (shadowColor != null)
            {
                requiresMaterialName += $"_shadow_{ColorUtility.ToHtmlStringRGBA(shadowColor.Value)}";
            }

            var fontMaterialAssetPath = $"{materialFolder}/{requiresMaterialName}.mat";
            var fontMaterial = AssetDatabase.LoadAssetAtPath<Material>(fontMaterialAssetPath);
            if (fontMaterial == null)
            {
                Debug.Log($"create font material {fontMaterialAssetPath}");
                fontMaterial = new Material(fontAsset.material);
                if (outlineColor != null)
                {
                    fontMaterial.EnableKeyword("OUTLINE_ON");
                    fontMaterial.SetColor("_OutlineColor", outlineColor.Value);
                    fontMaterial.SetFloat("_OutlineWidth", .2f);
                }

                if (shadowColor != null)
                {
                    fontMaterial.EnableKeyword("UNDERLAY_ON");
                    fontMaterial.SetColor("_UnderlayColor", shadowColor.Value);
                    fontMaterial.SetFloat("_UnderlayOffsetX", .4f);
                    fontMaterial.SetFloat("_UnderlayOffsetY", -.4f);
                }

                AssetDatabase.CreateAsset(fontMaterial, fontMaterialAssetPath);
            }

            return fontMaterial;
        }
    }
}