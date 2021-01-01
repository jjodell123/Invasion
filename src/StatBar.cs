using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    // Sets the max value for the stat bar, and automatically puts the current
    // value to max.
    public void SetMaxValue(float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;
    }

    // Adds "change" to the slider value.
    public void UpdateValue(float change)
    {
        
        slider.value = slider.value + change;
    }
}
