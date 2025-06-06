using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] PlayerIndex playerIndex;
    [SerializeField, Required] Button powerThrowButton, doubleAttackButton, healButton;
    [SerializeField, Required] Image currentPlayerArrow, warningTimerFill, warningTimerIcon;
    [SerializeField, Required] Transform useSpecialItemBox;
    [SerializeField, Required] TextMeshProUGUI specialItemNameText;
    [SerializeField, Required] GameObject warningTimerGroup;
    GameContext gameContext;
    CharacterController characterController;


    void Awake()
    {
        DisableSpecialItemButton();
        HideCurrentPlayerArrow();
        HideWarningTimer();
    }

    void Start()
    {
        gameContext = GameManager.Instance.GameContext;
        characterController = PlayerManager.Instance.Players[playerIndex];
        powerThrowButton.onClick.AddListener(OnUsePowerThrow);
        doubleAttackButton.onClick.AddListener(OnUseDoubleAttack);
        healButton.onClick.AddListener(OnUseHeal);

        PlayerManager.OnPlayerStartTurn += OnPlayerStartTurnHandler;
        PlayerManager.OnPlayerEndTurn += OnPlayerEndTurnHandler;
        characterController.OnStartChage += OnStartChargeHandler;



    }

    void Update()
    {
        if (characterController.IsInTurn && !characterController.IsThrew && !characterController.IsCharging && gameContext.GamePhase == GamePhase.GameStart)
        {
            if (characterController.TimeToWarning <= 0 && characterController.TimeToThink > 0)
                ShowWarningTimer();
            else
                HideWarningTimer();
        }
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
        HideWarningTimer();
    }

    public void OnUsePowerThrow()
    {
        if (GameManager.Instance.GameContext.CurrentPlayer != playerIndex) return;
        if (!powerThrowButton.gameObject.activeInHierarchy) return;

        SpecialItemManager.Instance.UsePowerThow(playerIndex);

        powerThrowButton.gameObject.SetActive(false);
        DisableSpecialItemButton();
        ShowUseSpecialItem("Power Throw");
    }

    public void OnUseDoubleAttack()
    {
        if (GameManager.Instance.GameContext.CurrentPlayer != playerIndex) return;
        if (!doubleAttackButton.gameObject.activeInHierarchy) return;

        SpecialItemManager.Instance.UseDoubleAttack(playerIndex);

        doubleAttackButton.gameObject.SetActive(false);
        DisableSpecialItemButton();
        ShowUseSpecialItem("Double Attack");

    }

    public void OnUseHeal()
    {
        if (GameManager.Instance.GameContext.CurrentPlayer != playerIndex) return;
        if (!healButton.gameObject.activeInHierarchy) return;

        SpecialItemManager.Instance.UseHeal(playerIndex);

        healButton.gameObject.SetActive(false);
        DisableSpecialItemButton();
        ShowUseSpecialItem("Heal");

    }

    private void OnPlayerEndTurnHandler(PlayerIndex index)
    {
        if (index == playerIndex)
        {
            DisableSpecialItemButton();
            HideCurrentPlayerArrow();
            HideWarningTimer();
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

    private void ShowUseSpecialItem(string name)
    {
        specialItemNameText.SetText(name);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(useSpecialItemBox.DOLocalMoveY(7.3f, 0.5f));
        sequence.AppendInterval(2f);
        sequence.Append(useSpecialItemBox.DOLocalMoveY(-4.3f, 0.5f));
        sequence.Play();
    }

    Sequence warningTimerSequence;
    private void ShowWarningTimer()
    {
        warningTimerGroup.SetActive(true);
        float thinkTime = gameContext.GameConfig.Config["Time to think"].Sec;
        float warningTime = gameContext.GameConfig.Config["Time to Warning"].Sec;
        warningTimerFill.fillAmount = characterController.TimeToThink / (thinkTime - warningTime);

        if (warningTimerSequence == null)
        {
            warningTimerSequence = DOTween.Sequence();
            warningTimerIcon.transform.localRotation = Quaternion.Euler(0, 0, -10);
            warningTimerSequence.Append(warningTimerIcon.transform.DORotate(new Vector3(0, 0, 10), 0.5f)).SetEase(Ease.Linear);
            warningTimerSequence.Append(warningTimerIcon.transform.DORotate(new Vector3(0, 0, -10), 0.5f)).SetEase(Ease.Linear);
            warningTimerSequence.SetLoops(-1);
        }
    }

    private void HideWarningTimer()
    {
        warningTimerGroup.SetActive(false);
    }
}
