using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace AmazingAssets.AdvancedDissolve.ExampleScripts
{
    public class UIInput : MonoBehaviour
    {
        public AdvancedDissolveGeometricCutoutController geometricCutoutController;

        public Text fpsText;
        float deltaTime = 0.0f;
        bool displayFPS;

        public GameObject menu;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //FPS
            if (Input.GetKeyDown(KeyCode.F))
                displayFPS = !displayFPS;

            //VSync
            if (Input.GetKeyDown(KeyCode.V))
                QualitySettings.vSyncCount = (QualitySettings.vSyncCount == 0) ? 1 : 0;

            //Noise
            if (Input.GetKeyDown(KeyCode.N))
                geometricCutoutController.noise = (geometricCutoutController.noise > 0) ? 0 : 1;

            //Invert
            if (Input.GetKeyDown(KeyCode.I))
                geometricCutoutController.invert = !geometricCutoutController.invert;

            //Show/Hide Menu
            if (Input.GetKeyDown(KeyCode.H))
                menu.SetActive(!menu.activeSelf);


            UpdateFPS();
        }


        void UpdateFPS()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;


            if (displayFPS)
            {
                float fps = 1.0f / deltaTime;
                fpsText.text = (int)fps + " fps";
            }
            else
                fpsText.text = string.Empty;
        }
    }
}