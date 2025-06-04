using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;

using UnityEngine;

public class CharacterController : MonoBehaviour, ITurn
{
    [SerializeField] PlayerIndex playerIndex;
    public PlayerIndex PlayerIndex => playerIndex;

    [FoldoutGroup("Reference"), Required]
    [SerializeField] ThrowController throwController;

    public bool IsInTurn => GameManager.Instance.GameContext.CurrentPlayer == PlayerIndex;
    private GameContext gameContext;

    void Start()
    {
        gameContext = GameManager.Instance.GameContext;

        throwController.OnCollided += async () =>
        {
            await UniTask.WaitForSeconds(2f);
            EndTurn();
        };
    }


    void Update()
    {
        if (IsInTurn && !throwController.IsThrew)
        {
            if (Input.GetMouseButton(0))
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

    public void TakeDamage(int damage)
    {

    }

    public void StartTurn()
    {
        Debug.Log("Start turn player: " + playerIndex.ToString());
        gameContext.CurrentPlayer = playerIndex;

        throwController.ResetThow();
    }

    public void EndTurn()
    {
        throwController.HideChargeGauge();
        PlayerManager.OnPlayerEndTurn?.Invoke(playerIndex);

    }
}

public enum ThrowType
{
    Normal,
    Power
}
