using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AmazingAssets.AdvancedDissolve.ExampleScripts
{
    public class UIChangeSize : MonoBehaviour
    {
        public AdvancedDissolveGeometricCutoutController geometricCutoutController;

        public Light spotLight;
        public float radius = 5;



        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.Equals))
            {
                radius += Time.deltaTime * 3;
                radius = Mathf.Clamp(radius, 1, 20);
            }

            if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
            {
                radius -= Time.deltaTime * 3;
                radius = Mathf.Clamp(radius, 1, 20);
            }


            //Update mask object (first mask object only)
            //Note, in demo scenes we have only one mask controller
            switch (geometricCutoutController.type)
            {
                case AdvancedDissolveKeywords.CutoutGeometricType.Sphere:
                    geometricCutoutController.target1Radius = radius;
                    break;
                case AdvancedDissolveKeywords.CutoutGeometricType.Cube:
                    transform.localScale = Vector3.one * radius;
                    break;
                case AdvancedDissolveKeywords.CutoutGeometricType.Capsule:
                    geometricCutoutController.target1Radius = radius;
                    break;
                case AdvancedDissolveKeywords.CutoutGeometricType.ConeSmooth:
                    {
                        spotLight.spotAngle = radius * 2;
                    }
                    break;
            }
        }
    }
}