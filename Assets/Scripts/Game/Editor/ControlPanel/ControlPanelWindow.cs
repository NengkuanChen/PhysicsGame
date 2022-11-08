using Game.Editor.Build;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.ControlPanel
{
    public class ControlPanelWindow : OdinEditorWindow
    {
        [MenuItem("Tools/工程控制面板")]
        private static void ShowWindow()
        {
            var window = GetWindow<ControlPanelWindow>();
            window.titleContent = new GUIContent("控制面板");
            window.minSize = new Vector2(600, 400);
            window.Show();
        }

        [SerializeField, LabelText("开启打点相关代码"), OnValueChanged("OnReportCodeEnableChanged")]
        private bool reportCodeEnable;
        [SerializeField, LabelText("开启广告相关代码"), OnValueChanged("OnAdvertisementCodeEnable")]
        private bool advertisementCodeEnable;

        /**********************************
         ***** OCF = OnlyCarFramework *****
         **********************************/
        
        private const string ReportCodeSymbol = "OCF_REPORT_ENABLE";
        private const string AdvertisementCodeSymbol = "OCF_ADVERTISEMENT_ENABLE";

        protected override void OnEnable()
        {
            base.OnEnable();

            reportCodeEnable = BuildSymbolHandler.IsSymbolEnabled(ReportCodeSymbol);
            advertisementCodeEnable = BuildSymbolHandler.IsSymbolEnabled(AdvertisementCodeSymbol);
        }

        private void OnReportCodeEnableChanged()
        {
            if (reportCodeEnable)
            {
                BuildSymbolHandler.EnableSymbol(ReportCodeSymbol);
            }
            else
            {
                BuildSymbolHandler.DisableSymbol(ReportCodeSymbol);
            }
        }

        private void OnAdvertisementCodeEnable()
        {
            if (advertisementCodeEnable)
            {
                BuildSymbolHandler.EnableSymbol(AdvertisementCodeSymbol);
            }
            else
            {
                BuildSymbolHandler.DisableSymbol(AdvertisementCodeSymbol);
            }
        }
    }
}