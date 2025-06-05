using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] PlayerIndex playerIndex;
    [SerializeField, Required] Button powerThrowButton, doubleAttackButton, healButton;
    GameContext gameContext;

    void Awake()
    {
        DisableSpecialItemButton();
    }

    void Start()
    {
        gameContext = GameManager.Instance.GameContext;
        powerThrowButton.onClick.AddListener(OnUsePowerThrow);
        doubleAttackButton.onClick.AddListener(OnUseDoubleAttack);
        healButton.onClick.AddListener(OnUseHeal);

        PlayerManager.OnPlayerStartTurn += OnPlayerStartTurnHandler;
        PlayerManager.OnPlayerEndTurn += OnPlayerEndTurnHandler;


    }

    void OnDestroy()
    {
        PlayerManager.OnPlayerStartTurn -= OnPlayerStartTurnHandler;
        PlayerManager.OnPlayerEndTurn -= OnPlayerEndTurnHandler;
    }

    private void OnUsePowerThrow()
    {
        if (GameManager.Instance.GameContext.CurrentPlayer != playerIndex) return;

        SpecialItemManager.Instance.UsePowerThow(playerIndex);

        powerThrowButton.gameObject.SetActive(false);
        DisableSpecialItemButton();

    }

    private void OnUseDoubleAttack()
    {
        if (GameManager.Instance.GameContext.CurrentPlayer != playerIndex) return;

        SpecialItemManager.Instance.UseDoubleAttack(playerIndex);

        doubleAttackButton.gameObject.SetActive(false);
        DisableSpecialItemButton();

    }

    private void OnUseHeal()
    {
        if (GameManager.Instance.GameContext.CurrentPlayer != playerIndex) return;

        SpecialItemManager.Instance.UseHeal(playerIndex);

        healButton.gameObject.SetActive(false);
        DisableSpecialItemButton();


    }

    private void OnPlayerEndTurnHandler(PlayerIndex index)
    {
        if (index == playerIndex)
        {
            DisableSpecialItemButton();
        }
    }

    private void OnPlayerStartTurnHandler(PlayerIndex index)
    {
        if (index == playerIndex)
        {
            EnableSpecialItemButton();
        }
    }


    public void DisableSpecialItemButton()
    {
        powerThrowButton.interactable = false;
        doubleAttackButton.interactable = false;
        healButton.interactable = false;
    }

    public void EnableSpecialItemButton()
    {
        powerThrowButton.interactable = true;
        doubleAttackButton.interactable = true;
        healButton.interactable = true;
    }
}
