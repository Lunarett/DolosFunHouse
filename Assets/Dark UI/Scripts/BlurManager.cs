﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.Dark
{
    public class BlurManager : MonoBehaviour
    {
        [Header("RESOURCES")]
        public Material blurMaterial;

        [Header("SETTINGS")]
        [Range(0.0f, 10)] public float blurValue = 5.0f;
        [Range(0.1f, 50)] public float animationSpeed = 25;
        public string customProperty = "_Size";

        float currentBlurValue;

        void Start()
        {
            if(customProperty == null)
            {
                customProperty = "_Size";
            }

            blurMaterial.SetFloat(customProperty, 0);
        }

        IEnumerator BlurIn()
        {
            currentBlurValue = blurMaterial.GetFloat(customProperty);

            while (currentBlurValue <= blurValue)
            {
                currentBlurValue += Time.deltaTime * animationSpeed;

                if (currentBlurValue >= blurValue)
                {
                    currentBlurValue = blurValue;
                }

                blurMaterial.SetFloat(customProperty, currentBlurValue);
                yield return null;
            }
            StopCoroutine("BlurIn");
        }

        IEnumerator BlurOut()
        {
            currentBlurValue = blurMaterial.GetFloat(customProperty);

            while (currentBlurValue >= 0)
            {
                currentBlurValue -= Time.deltaTime * animationSpeed;

                if (currentBlurValue <= 0)
                {
                    currentBlurValue = 0;
                }

                blurMaterial.SetFloat(customProperty, currentBlurValue);
                yield return null;
            }
            StopCoroutine("BlurOut");
        }

        public void BlurInAnim()
        {
            StopCoroutine("BlurOut");
            StartCoroutine("BlurIn");
        }

        public void BlurOutAnim()
        {
            StopCoroutine("BlurIn");
            StartCoroutine("BlurOut");
        }

        public void SetBlurValue(float cbv)
        {
            blurValue = cbv;
        }
    }
}