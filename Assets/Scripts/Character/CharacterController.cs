using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

public class CharacterController : MonoBehaviour, ITurn
{
    public const string smallHitAnimation = "Moody Friendly";
    public const string normalHitAnimation = "Moody UnFriendly";
    public const string notHitAnimation = "Happy Friendly";
    public const string winAnimation = "Cheer Friendly";
    public const string loseAnimation = "Moody UnFriendly";
    public const string idleAnimation = "Idle UnFriendly 1";

    [SerializeField] PlayerIndex playerIndex;
    public PlayerIndex PlayerIndex => playerIndex;

    [SerializeField] float maxHp;
    public float MaxHp => maxHp;

    [SerializeField] float currentHp;
    public float CurrentHp => currentHp;

    [FoldoutGroup("Reference"), Required]
    [SerializeField] ThrowController throwController;

    [FoldoutGroup("Reference"), Required]
    [SerializeField] EntityHealth_UI entityHealth_UI;

    [SerializeField] List<Collider2D> hitBoxColliders;

    public bool IsInTurn => GameManager.Instance.GameContext.CurrentPlayer == PlayerIndex;
    private GameContext gameContext;

    public bool IsMouseOverCharacter;
    public bool IsCharging => throwController.IsCharging;
    public bool IsThrew => throwController.IsThrew;
    private SkeletonAnimation skeletonAnimation;
    public bool IsTakeDamageThisTurn { get; private set; } = false;
    public event Action OnStartChage;

    public float TimeToThink { get; private set; }
    public float TimeToWarning { get; private set; }

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



        throwController.OnStartChage += () =>
        {
            OnStartChage?.Invoke();
        };

    }


    async void Update()
    {
        if (gameContext.GameMode != GameMode.OnePlayer || playerIndex == PlayerIndex.Player2)
            InputHandler();

        if (IsInTurn && !IsThrew && !IsCharging && gameContext.GamePhase == GamePhase.GameStart)
        {
            TimeToThink -= Time.deltaTime;
            TimeToWarning -= Time.deltaTime;

            if (TimeToThink <= 0)
            {
                SetThrowAmount(0);
                await UniTask.WaitForSeconds(2f);
                EndTurn();
            }
        }
    }

    private void InputHandler()
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

        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
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
            SetAnimation(normalHitAnimation);
            AddAnimation(idleAnimation, true);
        }
        else
        {
            if (isCrit)
            {
                damage = config["Normal Attack"].Damage;
                SetAnimation(normalHitAnimation);
                AddAnimation(idleAnimation, true);
            }
            else
            {
                damage = config["Small Attack"].Damage;
                SetAnimation(smallHitAnimation);
                AddAnimation(idleAnimation, true);

            }
        }
        Debug.Log("TakeDamage: " + damage);
        IsTakeDamageThisTurn = true;
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        UpdateHPBar();

        if (currentHp <= 0)
        {
            GameManager.Instance.EndGame(PlayerIndex);
        }
    }

    public void StartTurn()
    {
        Debug.Log("Start turn player: " + playerIndex.ToString());
        gameContext.CurrentPlayer = playerIndex;

        hitBoxColliders.ForEach(col => col.enabled = false);
        throwController.ResetThow();
        IsTakeDamageThisTurn = false;
        var config = gameContext.GameConfig.Config;
        TimeToThink = config["Time to think"].Sec;
        TimeToWarning = config["Time to Warning"].Sec;
        PlayerManager.OnPlayerStartTurn?.Invoke(playerIndex);

    }

    public void EndTurn()
    {
        if (gameContext.CurrentPlayer != playerIndex) return;

        hitBoxColliders.ForEach(col => col.enabled = true);
        throwController.HideChargeGauge();

        CharacterController enemyCharacter = PlayerManager.Instance.GetEnemyCharacter(playerIndex);
        if (!enemyCharacter.IsTakeDamageThisTurn && IsThrew)
        {
            enemyCharacter.SetAnimation(notHitAnimation);
            enemyCharacter.AddAnimation(idleAnimation, true);
        }
        throwController.ThrowType = ThrowType.Normal;
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

    public void ChargeAndThrow(float chargeValue)
    {
        if (throwController.ThrowAmount > 0)
            throwController.ChargeAndThrow(chargeValue);
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        UpdateHPBar();
    }

    public void SetAnimation(string animationName, bool isLoop = false)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, animationName, isLoop);
    }

    public void AddAnimation(string animationName, bool isLoop = false, float delay = 0)
    {
        skeletonAnimation.AnimationState.AddAnimation(0, animationName, isLoop, delay);
    }
}

public enum ThrowType
{
    Normal,
    Power
}
