using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image filler;
    [SerializeField] private TextMeshProUGUI timer;

    private int currentTime = 0;

    public IEnumerator StartCountDown(int maxTime)
    {
        currentTime = maxTime;

        while (currentTime >= 0)
        {
            timer.text = currentTime.ToString();
            filler.fillAmount = (float)currentTime / (float)maxTime;
            yield return new WaitForSeconds(1.0f);
            currentTime--;
        }

        GameController.Instance.OnTimeOut();
    }
}
