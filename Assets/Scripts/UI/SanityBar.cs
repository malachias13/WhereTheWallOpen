using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ManaFlux
{
    public class SanityBar : MonoBehaviour
    {
        public Slider slider;

        public void SetMaxSanity(float maxSanity)
        {
            slider.maxValue = maxSanity;
            slider.value = maxSanity;
        }
        public void SetCurrentSanity(float currentSanity)
        {
            slider.value = currentSanity;
        }
    }
}