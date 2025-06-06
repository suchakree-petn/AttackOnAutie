using System;
using System.Collections;
using System.Collections.Generic;
using Google.Apis;
using Sirenix.Utilities;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{

    [SerializeField] Dictionary<PlayerIndex, CharacterController> players = new();
    public Dictionary<PlayerIndex, CharacterController> Players => players;

    public static Action<PlayerIndex> OnPlayerStartTurn;
    public static Action<PlayerIndex> OnPlayerEndTurn;

    protected override void InitAfterAwake()
    {
    }
    private void Start()
    {
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
    }

    private void OnPlayerEndTurnHandler(PlayerIndex index)
    {
        PlayerIndex nextPlayer = index switch
        {
            PlayerIndex.Player1 => PlayerIndex.Player2,
            PlayerIndex.Player2 => PlayerIndex.Player1,
            _ => PlayerIndex.Player1
        };

        if (GameManager.Instance.GameContext.GamePhase == GamePhase.GameEnd)
        {
            Players[nextPlayer].EndTurn();
        }
        else
        {
            Players[nextPlayer].StartTurn();

        }
    }

    public CharacterController GetEnemyCharacter(PlayerIndex playerIndex)
    {
        return playerIndex == PlayerIndex.Player1 ? players[PlayerIndex.Player2] : players[PlayerIndex.Player1];
    }
}
