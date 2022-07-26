using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class GameManager : GenericSingleton<GameManager>
{
    [Header("Game Configs")]
    public int numberOfMatch;
    public int maxTimer;
    public int maxEnergy;
    public float energyFillRate;
    public float minFov;
    public float maxFov;
    public float ballSpeed;
    public float aiSpawnAttackerRate;
    public float aiSpawnDefenderRate;
    public bool enableARMode = false;

    [Header("Attacker Configs")]
    public int attackerCost;
    public float attackerSpawnTime;
    public float attackerReactiveTime;
    public float attackerNormalSpeed;
    public float attackerCarryingSpeed;

    [Header("Defender Configs")]
    public int defenderCost;
    public float defenderSpawnTime;
    public float defenderReactiveTime;
    public float defenderNormalSpeed;
    public float defenderReturnSpeed;
    public float detectionRange;

    [HideInInspector] public bool isLocalPVP;

    private void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
    }
}
