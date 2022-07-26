using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawn : MonoBehaviour
{
    private float currentTime;
    public void UpdateLogic()
    {
        if (GameController.Instance.isGameOver || GameController.Instance.isMatcheOver)
        {
            currentTime = 0.0f;
            return;
        }

        currentTime += Time.deltaTime;
        if (currentTime > 1.0f)
        {
            if (GameController.Instance.isPlayerAttack)
            {
                if (GameController.Instance.enemyEnergyBar.energyCount >= GameManager.Instance.defenderCost && ShouldDefend())
                {
                    AttackerController target = GameController.Instance.GetHasBallAttacker();
                    if (target == null)
                    {
                        float x = Random.Range(4, -4);
                        float z = Random.Range(1, 8);
                        Vector3 position = new Vector3(x, 1f, z);

                        GameController.Instance.SpawnDefender(position);
                    }
                    else
                    {
                        float x = target.transform.localPosition.x;
                        float z = target.transform.localPosition.y + 1f;
                        Vector3 position = new Vector3(x, 1f, z);

                        GameController.Instance.SpawnDefender(position);
                    }
                }
            }
            else
            {
                if (GameController.Instance.enemyEnergyBar.energyCount >= GameManager.Instance.attackerCost && ShouldAttack())
                {
                    int x = Random.Range(4, -4);
                    int z = Random.Range(1, 8);
                    Vector3 position = new Vector3(x, 1f, z);

                    GameController.Instance.SpawnAttacker(position);
                }
            }

            currentTime = 0.0f;
        }
    }

    public bool ShouldAttack()
    {
        float[] attackChance = { GameManager.Instance.aiSpawnAttackerRate, 1f - GameManager.Instance.aiSpawnAttackerRate};

        if (attackChance[Choose(attackChance)] == GameManager.Instance.aiSpawnAttackerRate)
        {
            return true;
        }
        return false;
    }

    public bool ShouldDefend()
    {
        float[] defendChance = { GameManager.Instance.aiSpawnDefenderRate, 1f - GameManager.Instance.aiSpawnDefenderRate };
        
        if (defendChance[Choose(defendChance)] == GameManager.Instance.aiSpawnDefenderRate)
        {
            return true;
        }
        return false;
    }

    private int Choose(float[] probs)
    {

        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }

}
