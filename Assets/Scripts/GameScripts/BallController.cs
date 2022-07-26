using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private bool isHeld = false;
    private Transform target;

    public bool IsAvailable()
    {
        return !isHeld;
    }

    public void IsHeld(bool isHeld)
    {
        this.isHeld = isHeld;
    }

    public void Update()
    {
        if(isHeld)
        {
            if((target != null))
                this.transform.position = target.position + (Vector3.down * 0.75f);
        }
        else if (target != null)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.position + (Vector3.down * 0.75f), GameManager.Instance.ballSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Attacker" && IsAvailable())
        {
            target = other.transform;
            isHeld = true;

            AttackerController currentAttacker = other.gameObject.GetComponent<AttackerController>();
            currentAttacker.OnGetBall();
            GameController.Instance.OnBallTaken();
        }
    }

    public void SetBallTarget(Transform target)
    {
        this.target = target;
    }
}
