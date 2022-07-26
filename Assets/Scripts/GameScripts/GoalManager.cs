using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] private bool isPlayerGoal;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Attacker")
        {
            if ((!isPlayerGoal && GameController.Instance.isPlayerAttack) ||
                (isPlayerGoal && !GameController.Instance.isPlayerAttack))
            {
                AttackerController currentAttacker = other.gameObject.GetComponent<AttackerController>();
                if (currentAttacker.HasBall() && !GameController.Instance.isMatcheOver && !GameController.Instance.isGameOver)
                {
                    GameController.Instance.OnAttackerReachGoal();
                }
                else
                {
                    if (!GameController.Instance.enterPenalty)
                    {
                        GameObject effect = Instantiate(GameController.Instance.destroyEffect, transform);
                        effect.transform.position = other.transform.position;
                        GameController.Instance.DestroyAttacker(currentAttacker);
                    }
                }
            }
        }
    }
}
