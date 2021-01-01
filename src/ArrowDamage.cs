using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDamage : MonoBehaviour
{
    [SerializeField]
    private float maxDamage = 20f;
    private float percentDrawBack = 1f;
    
    // Sets the amount of damage the arrow does as a percent of maxDamage.
    // percent should be between 0 and 1, inclusive.
    public void SetDrawBackPercent(float percent)
    {
        percentDrawBack = Mathf.Clamp(percent, 0f, 1f);
    }

    public float GetDamage()
    {
        return maxDamage * percentDrawBack;
    }

    public float GetPercentDrawBack()
    {
        return percentDrawBack;
    }
}
