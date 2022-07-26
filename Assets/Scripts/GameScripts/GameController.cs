using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour//GenericSingleton<GameManager>
{
    public static float FIELD_WIDTH = 10;
    private static GameController instance;
    public static GameController Instance
    {
        get
        {
            return instance;
        }
    }

    [Header("References")]
    public Transform defaultPlayerTarget;
    public Transform defaultEnemyTarget;
    public GameObject debug;
    public EnergyBar enemyEnergyBar;
    public EnergyBar playerEnergyBar;
    public TimeManager timer;
    public CameraManager cameraManager;
    public CameraManager arCameraManager;
    public Transform spawnHolder;
    public GameObject attackerPrefab;
    public GameObject defenderPrefab;
    public MazeRender mazeRender;
    public Material playerMat;
    public Material enemyMat;
    public GameObject endPopup;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI scoreText;
    public AISpawn aiSpawn;
    public GameObject pauseMenu;
    public GameObject destroyEffect;

    [Header("Prefabs")]
    public GameObject ballPrefab;

    [Header("Sound")]
    public AudioClip clickEffect;
    public AudioClip cheerEffect;
    public AudioClip passEffect;

    public AudioSource source;

    [HideInInspector] public bool isPlayerAttack = false;
    [HideInInspector] public bool isMatcheOver = false;
    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public bool pauseGame = false;
    [HideInInspector] public bool enterPenalty = false;

    private GameObject ball = null;
    private List<AttackerController> attackerList = new List<AttackerController>();
    private List<DefenderController> defenderList = new List<DefenderController>();
    private int playerScore = 0;
    private int enemyScore = 0;
    private int currentMatchCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        StartGame();
    }

    public void StartGame()
    {
        pauseGame = false;
        isPlayerAttack = false;

        arCameraManager.gameObject.SetActive(GameManager.Instance.enableARMode);
        cameraManager.gameObject.SetActive(!GameManager.Instance.enableARMode);

        StartCoroutine(ResetMatch());
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseGame)
        {
            for (int i = 0; i < attackerList.Count; i++)
            {
                attackerList[i].StopMoving(true);
            }

            for (int i = 0; i < defenderList.Count; i++)
            {
                defenderList[i].StopMoving(true);
            }

            return;
        }
        else
        {
            for (int i = 0; i < attackerList.Count; i++)
            {
                attackerList[i].StopMoving(false);
            }

            for (int i = 0; i < defenderList.Count; i++)
            {
                defenderList[i].StopMoving(false);
            }
        }

        if (!isMatcheOver && !isGameOver)
        {
            if (!GameManager.Instance.enableARMode)
            {
                cameraManager.UpdateCamera();
            }
            else
            {
                arCameraManager.UpdateCamera();
            }

            UpdateAttacker();

            UpdateDefender();

            if(!GameManager.Instance.isLocalPVP)
            {
                aiSpawn.UpdateLogic();
            }
        }

        arCameraManager.gameObject.SetActive(GameManager.Instance.enableARMode);
        cameraManager.gameObject.SetActive(!GameManager.Instance.enableARMode);
    }

    private void FixedUpdate()
    {
        if (!isMatcheOver && !isGameOver)
        {
            enemyEnergyBar.StartUpdate(GameManager.Instance.energyFillRate);
            playerEnergyBar.StartUpdate(GameManager.Instance.energyFillRate);
        }
    }

    private void UpdateAttacker()
    {
        for (int i = 0; i < attackerList.Count; i++)
        {
            if (attackerList[i] != null)
                attackerList[i].MoveToTarget();
        }
    }

    private void UpdateDefender()
    {
        for (int i = 0; i < defenderList.Count; i++)
        {
            if (defenderList[i] != null)
                defenderList[i].MoveToTarget();
        }
    }

    public void SpawnAttacker(Vector3 position)
    {
        bool canPlayerAttack = (isPlayerAttack && (playerEnergyBar.energyCount - GameManager.Instance.attackerCost) >= 0) || enterPenalty;
        bool canEnemyAttack = (!isPlayerAttack && (enemyEnergyBar.energyCount - GameManager.Instance.attackerCost) >= 0) || enterPenalty;

        if (canPlayerAttack || canEnemyAttack)
        {
            source.PlayOneShot(clickEffect);
            GameObject go = Instantiate(attackerPrefab, position, new Quaternion());
            go.transform.parent = spawnHolder;
            go.transform.localPosition = position;
            //go.AddComponent<AttackerController>();

            AttackerController attackController = go.GetComponent<AttackerController>();
            attackController.ID = attackerList.Count;

            if (!enterPenalty)
            {
                if (canPlayerAttack) playerEnergyBar.UseEnergy(GameManager.Instance.attackerCost);
                if (canEnemyAttack) enemyEnergyBar.UseEnergy(GameManager.Instance.attackerCost);
            }

            if (!isPlayerAttack)
            {
                //attackController.SetTeamColor(enemyMat);
                attackController.gameObject.transform.eulerAngles = new Vector3(0f, 180f, 0f);
            }
            //else
            //{
            //    attackController.SetTeamColor(playerMat);
            //}

            if (ball != null && ball.GetComponent<BallController>().IsAvailable())
            {
                attackController.SetTarget(ball.transform);
            }
            else
            {
                if (isPlayerAttack)
                {
                    attackController.SetTarget(defaultPlayerTarget);
                }
                else
                {
                    attackController.SetTarget(defaultEnemyTarget);
                }
            }

            attackerList.Add(attackController);
        }
    }

    public void SpawnDefender(Vector3 position)
    {
        bool canPlayerDefend = !isPlayerAttack && (playerEnergyBar.energyCount - GameManager.Instance.defenderCost) >= 0;
        bool canEnemyDefend = isPlayerAttack && (enemyEnergyBar.energyCount - GameManager.Instance.defenderCost) >= 0;

        if (canPlayerDefend || canEnemyDefend)
        {
            source.PlayOneShot(clickEffect);
            GameObject go = Instantiate(defenderPrefab, position, new Quaternion());
            go.transform.parent = spawnHolder;
            go.transform.localPosition = position;
            //go.AddComponent<DefenderController>();

            DefenderController defenderController = go.GetComponent<DefenderController>();
            defenderController.ID = attackerList.Count;

            if (isPlayerAttack)
            {
                //defenderController.SetTeamColor(enemyMat);
                defenderController.gameObject.transform.eulerAngles = new Vector3(0f, 180f, 0f);
            }
            //else
            //{
            //    defenderController.SetTeamColor(playerMat);
            //}

            if (canPlayerDefend) playerEnergyBar.UseEnergy(GameManager.Instance.defenderCost);
            if (canEnemyDefend) enemyEnergyBar.UseEnergy(GameManager.Instance.defenderCost);

            defenderList.Add(defenderController);
        }
    }

    public void SpawnBall()
    {
        if(ball != null)
        {
            Destroy(ball);
        }

        ball = Instantiate(ballPrefab, spawnHolder);

        if (enterPenalty)
        {
            ball.transform.localPosition = mazeRender.ReturnRandomPosition() + (Vector3.up * 0.25f);
        }
        else
        {
            if (isPlayerAttack)
            {
                int x = Random.Range(4, -4);
                int z = Random.Range(-1, -8);
                ball.transform.localPosition = new Vector3(x, 0.25f, z);
            }
            else
            {
                int x = Random.Range(4, -4);
                int z = Random.Range(1, 8);
                ball.transform.localPosition = new Vector3(x, 0.25f, z);
            }
        }

        for (int i = 0; i < attackerList.Count; i++)
        {
            attackerList[i].SetTarget(ball.transform);
        }
    }

    public void OnBallTaken()
    {
        if (isPlayerAttack)
        {
            for (int i = 0; i < attackerList.Count; i++)
            {
                attackerList[i].SetTarget(defaultPlayerTarget);
            }
        }
        else
        {
            for (int i = 0; i < attackerList.Count; i++)
            {
                attackerList[i].SetTarget(defaultEnemyTarget);
            }
        }
    }

    public void OnAttackerReachGoal()
    {
        if (!isMatcheOver && !isGameOver)
        {
            if (isPlayerAttack)
            {
                playerScore++;
            }
            else
            {
                enemyScore++;
            }
            OnEndMatch();
        }
    }

    public void OnAttackPassFail()
    {
        if (!isMatcheOver && !isGameOver)
        {
            if (isPlayerAttack)
            {
                enemyScore++;
            }
            else
            {
                playerScore++;
            }
            OnEndMatch();
        }
    }

    public void OnTimeOut()
    {
        if (!isMatcheOver && !isGameOver)
        {
            if (enterPenalty)
            {
                enemyScore++;
                OnEndGame();
                return;
            }
            OnEndMatch();
        }
    }

    //public void OnEnterPenalty()
    //{
    //    enterPenalty = true;

    //    mazeRender.DrawMaze();

    //    SpawnAttacker(mazeRender.ReturnPostionByIndex(2,0) + Vector3.up);
    //}

    public void OnEndGame()
    {
        source.PlayOneShot(cheerEffect);
        isMatcheOver = true;
        isGameOver = true;
        StopAllCoroutines();
        StartCoroutine(ResetGame());
    }

    public void OnEndMatch()
    {
        source.PlayOneShot(cheerEffect);
        isMatcheOver = true;
        StopAllCoroutines();
        StartCoroutine(ResetMatch());
    }

    IEnumerator ResetMatch()
    {
        if(currentMatchCount > 0)
        {
            endPopup.SetActive(true);
        }

        if(currentMatchCount < GameManager.Instance.numberOfMatch)
        {
            roundText.text = $"ROUND {currentMatchCount} ENDED";
            currentMatchCount++;
        }
        else
        {
            if(playerScore > enemyScore)
            {
                roundText.text = $"<color=blue> BLUE <color=white> WIN !";
            }
            else if (playerScore < enemyScore)
            {
                roundText.text = $"<color=red> RED <color=white> WIN !";
            }
            else
            {
                roundText.text = $"DRAW!";
                enterPenalty = true;
            }
        }

        scoreText.text = $"<color=red> {enemyScore} <color=white> - <color=blue> {playerScore}";

        for (int i = 0; i < attackerList.Count; i++)
        {
            attackerList[i].StopMoving(true);
        }

        for (int i = 0; i < defenderList.Count; i++)
        {
            defenderList[i].StopMoving(true);
        }

        yield return new WaitForSeconds(1.0f);

        endPopup.SetActive(false);

        isPlayerAttack = !isPlayerAttack;

        if (ball != null)
        {
            Destroy(ball);
        }

        for (int i = spawnHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(spawnHolder.GetChild(i).gameObject);
        }

        attackerList.Clear();
        defenderList.Clear();

        playerEnergyBar.ResetEnergyBar();
        enemyEnergyBar.ResetEnergyBar();

        StartCoroutine(timer.StartCountDown(GameManager.Instance.maxTimer));

        isMatcheOver = false;

        if(enterPenalty)
        {
            mazeRender.DrawMaze();

            SpawnBall();

            if (isPlayerAttack)
            {
                SpawnAttacker(mazeRender.ReturnPostionByIndex(2, 0) + Vector3.up);
            }
            else
            {
                SpawnAttacker(mazeRender.ReturnPostionByIndex(2, 9) + Vector3.up);
            }
        }
        else
        {
            SpawnBall();
        }
    }

    IEnumerator ResetGame()
    {
        if (currentMatchCount > 0)
        {
            endPopup.SetActive(true);
        }

        if (currentMatchCount < GameManager.Instance.numberOfMatch)
        {
            roundText.text = $"ROUND {currentMatchCount} ENDED";
            currentMatchCount++;
        }
        else
        {
            if (playerScore > enemyScore)
            {
                roundText.text = $"<color=red> RED <color=white> WIN !";
            }
            else if (playerScore < enemyScore)
            {
                roundText.text = $"<color=red> BLUE <color=white> WIN !";
            }
            else
            {
                roundText.text = $"DRAW!";
                enterPenalty = true;
            }
        }

        scoreText.text = $"<color=red> {enemyScore} <color=white> - <color=blue> {playerScore}";

        for (int i = 0; i < attackerList.Count; i++)
        {
            attackerList[i].StopMoving(true);
        }

        for (int i = 0; i < defenderList.Count; i++)
        {
            defenderList[i].StopMoving(true);
        }

        yield return new WaitForSeconds(1.0f);

        enemyScore = 0;
        playerScore = 0;
        enterPenalty = false;
        isMatcheOver = false;
        isGameOver = false;
        isPlayerAttack = false;

        if (ball != null)
        {
            Destroy(ball);
        }

        for (int i = spawnHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(spawnHolder.GetChild(i).gameObject);
        }

        attackerList.Clear();
        defenderList.Clear();

        playerEnergyBar.ResetEnergyBar();
        enemyEnergyBar.ResetEnergyBar();

        StartCoroutine(timer.StartCountDown(GameManager.Instance.maxTimer));

        if (enterPenalty)
        {
            mazeRender.DestroyMaze();
        }

        SceneManager.LoadScene(0);
    }

    public AttackerController GetAttackerByID(int ID)
    {
        //if (ID >= attackerList.Count || ID == -1) return null;
        return attackerList.Find(x => x.ID == ID);
    }

    public DefenderController GetDefenderByID(int ID)
    {
        //if (ID >= defenderList.Count || ID == -1) return null;

        //return defenderList[ID];
        return defenderList.Find(x => x.ID == ID);
    }

    public AttackerController GetNearestAttacker(int currentAttackerID)
    {
        AttackerController currentAttacker = attackerList.Find(x => x.ID == currentAttackerID);

        float distance = float.MaxValue;
        AttackerController nearestAttacker = null;
        for (int i = 0; i < attackerList.Count; i++)
        {
            if(attackerList[i] != null && attackerList[i].ID != currentAttacker.ID)
            {
                float tempDistance = Mathf.Abs(Vector3.Distance(currentAttacker.transform.position, attackerList[i].transform.position));
                if(tempDistance < distance && !attackerList[i].IsCoolingDown())
                {
                    distance = tempDistance;
                    nearestAttacker = attackerList[i];
                }
            }
        }

        return nearestAttacker;
    }

    public void PassToTarget(Transform target)
    {
        source.PlayOneShot(passEffect);
        BallController currentBall = ball.GetComponent<BallController>();
        currentBall.IsHeld(false);
        currentBall.SetBallTarget(target);
    }

    public void DestroyAttacker(AttackerController attacker)
    {
        attackerList.Remove(attackerList.Find(x => x.ID == attacker.ID));
        Destroy(attacker.gameObject);
    }

    public void DestroyDefender(DefenderController defender)
    {
        attackerList.Remove(attackerList.Find(x => x.ID == defender.ID));
        Destroy(defender.gameObject);
    }

    //public void OnARButtonClick()
    //{
    //    if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
    //    {
    //        Permission.RequestUserPermission(Permission.Camera);
    //    }
    //    else
    //    {
    //        GameManager.Instance.enableARMode = !GameManager.Instance.enableARMode;

    //        arCameraManager.gameObject.SetActive(GameManager.Instance.enableARMode);
    //        cameraManager.gameObject.SetActive(!GameManager.Instance.enableARMode);
    //    }
    //}

    public AttackerController GetHasBallAttacker()
    {
        return attackerList.Find(x => x.HasBall());
    }

    public void OnPauseClicked()
    {
        pauseMenu.SetActive(true);
        pauseGame = true;
    }

    public void OnResumeClicked()
    {
        pauseMenu.SetActive(false);
        pauseGame = false;
    }

    public void OnMainMenuClicked()
    {
        pauseMenu.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
