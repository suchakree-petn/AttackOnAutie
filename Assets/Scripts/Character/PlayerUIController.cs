using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] PlayerIndex playerIndex;
    [SerializeField, Required] Button powerThrowButton, doubleAttackButton, healButton;
    [SerializeField, Required] Image currentPlayerArrow;
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
        PlayerManager.Instance.Players.Values.ForEach(player =>
        {
            player.OnStartChage += OnStartChargeHandler;
        });



    }



    void OnDestroy()
    {
        PlayerManager.OnPlayerStartTurn -= OnPlayerStartTurnHandler;
        PlayerManager.OnPlayerEndTurn -= OnPlayerEndTurnHandler;

        if (PlayerManager.Instance)
        {
            PlayerManager.Instance.Players.Values.ForEach(player =>
            {
                player.OnStartChage -= OnStartChargeHandler;
            });
        }

        DOTween.KillAll();
    }

    private void OnStartChargeHandler()
    {
        HideCurrentPlayerArrow();
        DisableSpecialItemButton();
    }

    public void OnUsePowerThrow()
    {
        if (GameManager.Instance.GameContext.CurrentPlayer != playerIndex) return;
        if (!powerThrowButton.gameObject.activeInHierarchy) return;

        SpecialItemManager.Instance.UsePowerThow(playerIndex);

        powerThrowButton.gameObject.SetActive(false);
        DisableSpecialItemButton();

    }

    public void OnUseDoubleAttack()
    {
        if (GameManager.Instance.GameContext.CurrentPlayer != playerIndex) return;
        if (!doubleAttackButton.gameObject.activeInHierarchy) return;

        SpecialItemManager.Instance.UseDoubleAttack(playerIndex);

        doubleAttackButton.gameObject.SetActive(false);
        DisableSpecialItemButton();

    }

    public void OnUseHeal()
    {
        if (GameManager.Instance.GameContext.CurrentPlayer != playerIndex) return;
        if (!healButton.gameObject.activeInHierarchy) return;

        SpecialItemManager.Instance.UseHeal(playerIndex);

        healButton.gameObject.SetActive(false);
        DisableSpecialItemButton();


    }

    private void OnPlayerEndTurnHandler(PlayerIndex index)
    {
        if (index == playerIndex)
        {
            DisableSpecialItemButton();
            HideCurrentPlayerArrow();

        }
    }

    private void OnPlayerStartTurnHandler(PlayerIndex index)
    {
        if (index == playerIndex)
        {
            if (gameContext.GameMode == GameMode.OnePlayer && playerIndex == PlayerIndex.Player1)
                DisableSpecialItemButton();
            else
                EnableSpecialItemButton();
            ShowCurrentPlayerArrow();

        }
    }
    Sequence currentPlayerArrowSequence;
    public void ShowCurrentPlayerArrow()
    {
        currentPlayerArrow.gameObject.SetActive(true);

        if (currentPlayerArrowSequence == null)
        {
            currentPlayerArrowSequence = DOTween.Sequence();
            float initialY = currentPlayerArrow.transform.localPosition.y;
            currentPlayerArrowSequence.Append(currentPlayerArrow.transform.DOLocalMoveY(initialY + 1, 1f));
            currentPlayerArrowSequence.Append(currentPlayerArrow.transform.DOLocalMoveY(initialY, 1f));
            currentPlayerArrowSequence.SetLoops(-1);
            currentPlayerArrowSequence.Play();
        }
    }

    public void HideCurrentPlayerArrow()
    {
        currentPlayerArrow.gameObject.SetActive(false);
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
