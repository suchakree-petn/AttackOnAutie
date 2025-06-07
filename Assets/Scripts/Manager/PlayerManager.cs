using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] Dictionary<PlayerIndex, CharacterController> players = new();
    public Dictionary<PlayerIndex, CharacterController> Players => players;
    [SerializeField, Required] PlayerUIController aiUIController;

    public static Action<PlayerIndex> OnPlayerStartTurn;
    public static Action<PlayerIndex> OnPlayerEndTurn;


    GameContext gameContext;

    protected override void InitAfterAwake()
    {
    }
    private void Start()
    {
        gameContext = GameManager.Instance.GameContext;

        OnPlayerStartTurn += OnPlayerStartTurnHandler;
        OnPlayerEndTurn += OnPlayerEndTurnHandler;

        GameManager.Instance.StartGame();
    }

    void OnDestroy()
    {
        OnPlayerStartTurn -= OnPlayerStartTurnHandler;
        OnPlayerEndTurn -= OnPlayerEndTurnHandler;
    }

    private void OnPlayerStartTurnHandler(PlayerIndex index)
    {
        WindManager.Instance.RandomWind();

        if (gameContext.GameMode == GameMode.OnePlayer && index == PlayerIndex.Player1)
        {
            AIDecision();
        }
    }

    private void OnPlayerEndTurnHandler(PlayerIndex index)
    {
        PlayerIndex nextPlayer = index switch
        {
            PlayerIndex.Player1 => PlayerIndex.Player2,
            PlayerIndex.Player2 => PlayerIndex.Player1,
            _ => PlayerIndex.Player1
        };

        if (gameContext.GamePhase == GamePhase.GameEnd)
        {
            Players[nextPlayer].EndTurn();
        }
        else
        {
            Players[nextPlayer].StartTurn();
        }
    }
    public void InitPlayers()
    {
        if (gameContext.GameMode == GameMode.OnePlayer)
        {
            Players[PlayerIndex.Player1].Init(true);
            Players[PlayerIndex.Player2].Init(false);
        }
        else
        {
            Players[PlayerIndex.Player1].Init(false);
            Players[PlayerIndex.Player2].Init(false);
        }
    }
    private async void AIDecision()
    {
        await UniTask.WaitForSeconds(1);
        Difficulty difficulty = gameContext.Difficulty;
        string nameKey;
        float windStrength = WindManager.Instance.WindStrength;
        int windDirection = WindManager.Instance.WindDirection;
        switch (difficulty)
        {
            case Difficulty.Easy:
                nameKey = "Enemy HP(easy)";
                break;
            case Difficulty.Normal:
                nameKey = "Enemy HP(normal)";
                AIDecideUseSpecialItem(Difficulty.Normal, windStrength, windDirection);
                break;
            case Difficulty.Hard:
                nameKey = "Enemy HP(hard)";
                AIDecideUseSpecialItem(Difficulty.Hard, windStrength, windDirection);
                break;
            default:
                nameKey = "Enemy HP(easy)";
                break;
        }
        Config config = gameContext.GameConfig.Config[nameKey];
        float missChance = config.MissedChance;
        if (Random.value <= missChance / 100)
        {
            MissedThrow();
        }
        else
        {
            GuaranteeThrow();
        }
    }

    private void AIDecideUseSpecialItem(Difficulty difficulty, float windStrength, int windDirection)
    {
        switch (difficulty)
        {
            case Difficulty.Normal:
                if (windStrength <= 0.3f)
                    aiUIController.OnUseDoubleAttack();
                break;
            case Difficulty.Hard:
                CharacterController ai = Players[PlayerIndex.Player1];
                float tempCurrentHp = ai.CurrentHp;
                if (ai.CurrentHp / ai.MaxHp < 0.5f)
                    aiUIController.OnUseHeal();
                else if (tempCurrentHp == ai.CurrentHp)
                {
                    if (windStrength <= 0.3f)
                        aiUIController.OnUseDoubleAttack();

                    else if (windStrength > 0.75f && windDirection != 0)
                        aiUIController.OnUsePowerThrow();
                }
                break;
        }
    }

    private void MissedThrow()
    {
        Debug.Log("Miss throw");
        CharacterController aiChar = Players[PlayerIndex.Player1];
        float chargeValue = Random.value < 0.7f ? Random.Range(-7, 11) : Random.Range(22, 27);
        aiChar.ChargeAndThrow(chargeValue);
    }

    private void GuaranteeThrow()
    {
        Debug.Log("Guarantee throw");
        CharacterController aiChar = Players[PlayerIndex.Player1];
        float chargeValue = Random.Range(12.5f, 15.5f);
        aiChar.ChargeAndThrow(chargeValue);
    }

    public CharacterController GetEnemyCharacter(PlayerIndex playerIndex)
    {
        return playerIndex == PlayerIndex.Player1 ? players[PlayerIndex.Player2] : players[PlayerIndex.Player1];
    }
}
