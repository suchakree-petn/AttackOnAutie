using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{

    [SerializeField] Dictionary<PlayerIndex, CharacterController> players = new();
    public Dictionary<PlayerIndex, CharacterController> Players => players;

    public static Action<PlayerIndex> OnPlayerEndTurn;

    protected override void InitAfterAwake()
    {
    }
    private void Start()
    {
        OnPlayerEndTurn += OnPlayerEndTurnHandler;
    }

    private void OnPlayerEndTurnHandler(PlayerIndex index)
    {
        PlayerIndex nextPlayer = index switch
        {
            PlayerIndex.Player1 => PlayerIndex.Player2,
            PlayerIndex.Player2 => PlayerIndex.Player1,
            _ => PlayerIndex.Player1
        };

        Players[nextPlayer].StartTurn();
    }
}
