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
            player.OnStartChage += HideCurrentPlayerArrow;
        });



    }

    void OnDestroy()
    {
        PlayerManager.OnPlayerStartTurn -= OnPlayerStartTurnHandler;
        PlayerManager.OnPlayerEndTurn -= OnPlayerEndTurnHandler;

        if (PlayerManager.Instance )
        {
            PlayerManager.Instance.Players.Values.ForEach(player =>
            {
                player.OnStartChage -= HideCurrentPlayerArrow;
            });
        }
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
            HideCurrentPlayerArrow();

        }
    }

    private void OnPlayerStartTurnHandler(PlayerIndex index)
    {
        if (index == playerIndex)
        {
            EnableSpecialItemButton();
            ShowCurrentPlayerArrow();

        }
    }

    public void ShowCurrentPlayerArrow()
    {
        currentPlayerArrow.gameObject.SetActive(true);

        currentPlayerArrow.transform.DOKill();
        Sequence sequence = DOTween.Sequence();
        float initialY = currentPlayerArrow.transform.position.y;
        sequence.Append(currentPlayerArrow.transform.DOMoveY(initialY + 1, 1f));
        sequence.Append(currentPlayerArrow.transform.DOMoveY(initialY, 1f));
        sequence.SetLoops(-1);
        sequence.Play();
    }

    public void HideCurrentPlayerArrow()
    {
        currentPlayerArrow.gameObject.SetActive(false);
        currentPlayerArrow.transform.DOKill();
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
