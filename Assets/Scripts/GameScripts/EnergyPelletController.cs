using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyPelletController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image filler;

    public bool isFull = false;
    public void SetPelletPercent(float percent)
    {
        if (percent < 1.0f)
        {
            filler.fillAmount = percent;
        }
        else
        {
            filler.fillAmount = 1.0f;
            isFull = true;
        }
    }

    public void ResetPellet()
    {
        filler.fillAmount = 0.0f;
        isFull = false;
    }

    public float GetCurrentPercent()
    {
        return filler.fillAmount;
    }
}
