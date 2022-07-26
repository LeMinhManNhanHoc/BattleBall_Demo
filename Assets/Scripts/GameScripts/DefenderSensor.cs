using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderSensor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DefenderController defenderController;
    [SerializeField] private GameObject detectRange;

    private void Start()
    {
        transform.localScale = Vector3.one * (GameController.FIELD_WIDTH * GameManager.Instance.detectionRange / 100f);
        detectRange.transform.localScale = transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Attacker")
        {
            AttackerController currentAttacker = other.GetComponent<AttackerController>();
            if(currentAttacker.HasBall() && !defenderController.IsCoolingDown())
            {
                defenderController.SetTargetID(currentAttacker.ID);
            }
        }
    }
}
