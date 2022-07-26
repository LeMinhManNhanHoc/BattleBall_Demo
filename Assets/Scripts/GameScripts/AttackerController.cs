using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackerController : MonoBehaviour
{
    [SerializeField] private GameObject hightlightObject;
    [SerializeField] private float colorDarkerAmount;
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

    private Material attackerMaterial;
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
        //attackerMaterial = this.GetComponent<Renderer>().material;

        //coolDownColor = new Color(attackerMaterial.color.r - colorDarkerAmount,
        //                          attackerMaterial.color.g - colorDarkerAmount,
        //                          attackerMaterial.color.b - colorDarkerAmount);

        //activeColor = new Color(attackerMaterial.color.r,
        //                        attackerMaterial.color.g,
        //                        attackerMaterial.color.b);

        SetTeamColor();

        agent = this.GetComponent<NavMeshAgent>();
        agent.speed = GameManager.Instance.attackerNormalSpeed;

        if (!GameManager.Instance.isLocalPVP && !GameController.Instance.isPlayerAttack)
        {
            originalPosition = transform.position;
            agent.Warp(originalPosition);
        }

        StartCoroutine(WaitCoolDown(GameManager.Instance.attackerSpawnTime));
    }

    public void MoveToTarget()
    {
        if (canMove)
        {
            agent.SetDestination(target.position);
        }

        if (animator != null && agent != null)
            animator.SetFloat("Speed", agent.velocity.magnitude);

        hightlightObject.SetActive(HasBall());
    }

    public void StopMoving(bool stop)
    {
        if(stop)
            agent.velocity = Vector3.zero;

        agent.isStopped = stop;
    }

    public bool HasBall()
    {
        return hasBall;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public IEnumerator WaitCoolDown(float time)
    {
        bodyMat.color = coolDownColorBody;
        jointMat.color = coolDownColorJoint;
        hasBall = false;
        StopMoving(true);
        canMove = false;
        isOnCoolDown = true;

        yield return new WaitForSeconds(time);

        bodyMat.color = activeColorBody;
        jointMat.color = activeColorJoint;
        isOnCoolDown = false;
        StopMoving(false);
        canMove = true;
    }

    public void IsCaught()
    {
        AttackerController passToTarget = GameController.Instance.GetNearestAttacker(ID);
        if (passToTarget == null)
        {
            if (!GameController.Instance.isMatcheOver && !GameController.Instance.isGameOver)
                GameController.Instance.OnAttackPassFail();
            return;
        }
        else
        {
            GameController.Instance.PassToTarget(passToTarget.transform);
        }

        StartCoroutine(WaitCoolDown(GameManager.Instance.attackerReactiveTime));

        if (GameController.Instance.isPlayerAttack)
        {
            target = GameController.Instance.defaultPlayerTarget;
        }
        else
        {
            target = GameController.Instance.defaultEnemyTarget;
        }
        //canMove = true;
    }

    public void OnGetBall()
    {
        hasBall = true;
        agent.speed = GameManager.Instance.attackerCarryingSpeed;
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
        if (!GameController.Instance.isPlayerAttack)
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
