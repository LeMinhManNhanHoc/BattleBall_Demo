using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DefenderController : MonoBehaviour
{
    [SerializeField] private float colorDarkerAmount;
    [SerializeField] private GameObject detectRange;
    [SerializeField] private Animator animator;

    [Header("Set color")]
    [SerializeField] private SkinnedMeshRenderer body;
    [SerializeField] private SkinnedMeshRenderer joint;
    [SerializeField] private Material playerbodyMat;
    [SerializeField] private Material playerJointMat;
    [SerializeField] private Material enemybodyMat;
    [SerializeField] private Material enemyJointMat;
    [SerializeField] private Material bodyMat;
    [SerializeField] private Material jointMat;

    [HideInInspector] public int ID;

    //private Material defenderMaterial;
    private int targetID = -1;
    private Transform target;
    private bool hasBall = false;
    private bool canMove = false;
    private NavMeshAgent agent;
    private Vector3 originalPosition;
    private bool isOnCoolDown = false;

    //private Color coolDownColor;
    //private Color activeColor;

    private Color coolDownColorBody;
    private Color coolDownColorJoint;
    private Color activeColorBody;
    private Color activeColorJoint;

    // Start is called before the first frame update
    void Start()
    {
        //defenderMaterial = this.GetComponent<Renderer>().material;

        //coolDownColor = new Color(defenderMaterial.color.r - colorDarkerAmount,
        //                          defenderMaterial.color.g - colorDarkerAmount,
        //                          defenderMaterial.color.b - colorDarkerAmount);

        //activeColor = new Color(defenderMaterial.color.r,
        //                        defenderMaterial.color.g,
        //                        defenderMaterial.color.b);

        SetTeamColor();

        agent = this.GetComponent<NavMeshAgent>();
        agent.speed = GameManager.Instance.defenderNormalSpeed;

        if (!GameManager.Instance.isLocalPVP && GameController.Instance.isPlayerAttack)
        {
            originalPosition = transform.position;
            agent.Warp(originalPosition);
        }
        StartCoroutine(WaitCoolDown(GameManager.Instance.defenderSpawnTime));
    }

    public void MoveToTarget()
    {
        if (canMove)
        {
            AttackerController targetAttacker = GameController.Instance.GetAttackerByID(targetID);

            if (isOnCoolDown)
            {
                SetTarget(null);
            }
            else
            {
                if (targetAttacker != null)
                {
                    SetTarget(targetAttacker.transform);
                }
            }

            if (target != null)
            {
                agent.speed = GameManager.Instance.defenderNormalSpeed;
                agent.SetDestination(target.position);
            }
            else
            {
                agent.speed = GameManager.Instance.defenderReturnSpeed;
                agent.SetDestination(originalPosition);
            }
        }

        if (animator != null && agent != null)
            animator.SetFloat("Speed", agent.velocity.magnitude);

        detectRange.SetActive(!IsCoolingDown());
    }

    public void StopMoving(bool stop)
    {
        if(stop)
            agent.velocity = Vector3.zero;
        agent.isStopped = stop;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public IEnumerator WaitCoolDown(float time)
    {
        bodyMat.color = coolDownColorBody;
        jointMat.color = coolDownColorJoint;
        SetTarget(null);
        isOnCoolDown = true;
        StopMoving(true);
        canMove = false;

        yield return new WaitForSeconds(time);

        bodyMat.color = activeColorBody;
        jointMat.color = activeColorJoint;
        isOnCoolDown = false;
        StopMoving(false);
        canMove = true;
    }

    public IEnumerator WaitCoolDownAndReturn(float time)
    {
        bodyMat.color = coolDownColorBody;
        jointMat.color = coolDownColorJoint;
        isOnCoolDown = true;
        SetTargetID(-1);
        canMove = true;

        yield return new WaitForSeconds(time);

        bodyMat.color = activeColorBody;
        jointMat.color = activeColorJoint;
        isOnCoolDown = false;
        StopMoving(false);
        canMove = true;
    }

    public void IsCaught()
    {
        //StartCoroutine(WaitCoolDown(GameController.Instance.defenderReactiveTime));
        StartCoroutine(WaitCoolDownAndReturn(GameManager.Instance.defenderReactiveTime));
    }

    public void OnGetBall()
    {
        hasBall = true;
    }

    public void SetTargetID(int id)
    {
        targetID = id;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Attacker")
        {
            AttackerController currentAttacker = collision.gameObject.GetComponent<AttackerController>();

            if (currentAttacker.HasBall() || currentAttacker.IsCoolingDown())
            {
                IsCaught();
                currentAttacker.IsCaught();
            }
        }
    }

    public bool IsCoolingDown()
    {
        return isOnCoolDown;
    }

    //public void SetTeamColor(Material mat)
    //{
    //    MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
    //    if (meshRenderer != null)
    //    {
    //        meshRenderer.material = mat;
    //    }
    //}

    public void SetTeamColor()
    {
        if(GameController.Instance.isPlayerAttack)
        {
            body.material = enemybodyMat;
            joint.material = enemyJointMat;
        }
        else
        {
            body.material = playerbodyMat;
            joint.material = playerJointMat;
        }

        bodyMat = body.material;
        jointMat = joint.material;

        coolDownColorBody = new Color(bodyMat.color.r - colorDarkerAmount,
                                      bodyMat.color.g - colorDarkerAmount,
                                      bodyMat.color.b - colorDarkerAmount);

        coolDownColorJoint = new Color(jointMat.color.r - colorDarkerAmount,
                                      jointMat.color.g - colorDarkerAmount,
                                      jointMat.color.b - colorDarkerAmount);

        activeColorBody = new Color(bodyMat.color.r,
                                bodyMat.color.g,
                                bodyMat.color.b);

        activeColorJoint = new Color(jointMat.color.r,
                                jointMat.color.g,
                                jointMat.color.b);
    }
}