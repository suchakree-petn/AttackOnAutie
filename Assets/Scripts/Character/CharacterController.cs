using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using Sirenix.OdinInspector;

using UnityEngine;

public class CharacterController : MonoBehaviour, ITurn
{
    [SerializeField] PlayerIndex playerIndex;
    public PlayerIndex PlayerIndex => playerIndex;

    [SerializeField] float maxHp;
    [SerializeField] float currentHp;

    [FoldoutGroup("Reference"), Required]
    [SerializeField] ThrowController throwController;

    [FoldoutGroup("Reference"), Required]
    [SerializeField] EntityHealth_UI entityHealth_UI;

    [SerializeField] List<Collider2D> hitBoxColliders;



    public bool IsInTurn => GameManager.Instance.GameContext.CurrentPlayer == PlayerIndex;
    private GameContext gameContext;

    public bool IsMouseOverCharacter;

    void Start()
    {
        gameContext = GameManager.Instance.GameContext;

        throwController.OnCollided += async () =>
        {
            if (throwController.ThrowAmount <= 0)
            {
                await UniTask.WaitForSeconds(2f);
                EndTurn();
            }
        };
    }


    void Update()
    {
        if (IsInTurn && !throwController.IsThrew && throwController.ThrowAmount > 0)
        {
            if (Input.GetMouseButton(0) && IsMouseOverCharacter || throwController.IsCharging)
            {
                throwController.ShowChargeGauge();
                throwController.ChargeThrow();
            }

            if (Input.GetMouseButtonUp(0) && throwController.IsCharging)
            {
                throwController.Throw();
            }
        }


    }

    public void Init(bool isAI)
    {
        if (isAI)
        {
            Dictionary<string, Config> config = gameContext.GameConfig.Config;

            maxHp = gameContext.Difficulty switch
            {
                Difficulty.Easy => config["Enemy HP(easy)"].HP,
                Difficulty.Normal => config["Enemy HP(normal)"].HP,
                Difficulty.Hard => config["Enemy HP(hard)"].HP,
                _ => config["Enemy HP(easy)"].HP,
            };
        }
        else
        {
            maxHp = gameContext.GameConfig.Config["Player HP"].HP;

        }
        currentHp = maxHp;

        UpdateHPBar();

        hitBoxColliders.ForEach(col => col.enabled = true);
    }

    void UpdateHPBar()
    {
        entityHealth_UI.SetHpBar(currentHp / maxHp);
    }



    public void TakeDamage(bool isCrit, ThrowType throwType)
    {
        var config = gameContext.GameConfig.Config;

        int damage;
        if (throwType == ThrowType.Power)
        {
            damage = config["Power Throw"].Damage;
        }
        else
        {
            if (isCrit)
            {
                damage = config["Normal Attack"].Damage;
            }
            else
            {
                damage = config["Small Attack"].Damage;
            }
        }
        Debug.Log("TakeDamage: " + damage);
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        UpdateHPBar();

    }

    public void StartTurn()
    {
        Debug.Log("Start turn player: " + playerIndex.ToString());
        gameContext.CurrentPlayer = playerIndex;

        hitBoxColliders.ForEach(col => col.enabled = false);
        throwController.ResetThow();
        PlayerManager.OnPlayerStartTurn?.Invoke(playerIndex);

    }

    public void EndTurn()
    {
        hitBoxColliders.ForEach(col => col.enabled = true);
        throwController.HideChargeGauge();
        PlayerManager.OnPlayerEndTurn?.Invoke(playerIndex);

    }

    public void SetThrowType(ThrowType throwType)
    {
        throwController.ThrowType = throwType;
    }

    public void SetThrowAmount(int amount)
    {
        throwController.ThrowAmount = amount;
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        UpdateHPBar();
    }
}

public enum ThrowType
{
    Normal,
    Power
}
