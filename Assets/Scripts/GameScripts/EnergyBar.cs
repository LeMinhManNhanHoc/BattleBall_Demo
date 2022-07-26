using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBar : MonoBehaviour
{
    [HideInInspector] public int energyCount = 0;

    [Header("References")]
    public GameObject energyPelletPrefab;
    public Transform pelletHolder;

    private List<EnergyPelletController> energyPelletList = new List<EnergyPelletController>();
    private float currentEnergyPercent = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        //maxEnergyPelletCount = energyPelletList.Count;
        CreateEnergyPellet();
    }

    private void CreateEnergyPellet()
    {
        for (int i = 0; i < GameManager.Instance.maxEnergy; i++)
        {
            energyPelletList.Add(Instantiate(energyPelletPrefab, pelletHolder).GetComponent<EnergyPelletController>());
            energyPelletList[i].gameObject.name = $"Energy_{i}";
        }
    }

    public void StartUpdate(float speed)
    {
        if (currentEnergyPercent < 1.0f)
        {
            currentEnergyPercent += speed * Time.fixedDeltaTime;
        }
        else
        {
            currentEnergyPercent = 1.0f;
        }

        if(energyCount < energyPelletList.Count)
        {
            if(energyPelletList[energyCount].isFull)
            {
                energyCount++;
                currentEnergyPercent = 0.0f;
            }
            else
            {
                energyPelletList[energyCount].SetPelletPercent(currentEnergyPercent);
            }
        }
    }

    public void UseEnergy(int energyUsed)
    {
        energyCount -= energyUsed;

        for (int i = 0; i < energyUsed; i++)
        {
            energyPelletList[i].ResetPellet();
        }

        for (int i = energyUsed; i < energyPelletList.Count; i++)
        {
            energyPelletList[i - energyUsed].SetPelletPercent(energyPelletList[i].GetCurrentPercent());
            energyPelletList[i].ResetPellet();
        }
    }

    public void ResetEnergyBar()
    {
        energyCount = 0;

        for (int i = 0; i < energyPelletList.Count; i++)
        {
            energyPelletList[i].ResetPellet();
        }
    }
}
