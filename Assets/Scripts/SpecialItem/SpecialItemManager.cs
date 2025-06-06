using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SpecialItemManager : Singleton<SpecialItemManager>
{
  [InlineEditor]
  [SerializeField, Required] SpecialItem powerThrowItem, doubleAttackItem, healItem;
  GameContext gameContext;

  protected override void InitAfterAwake()
  {
  }
  void Start()
  {
    gameContext = GameManager.Instance.GameContext;
  }

 

  public void UsePowerThow(PlayerIndex playerIndex)
  {
    powerThrowItem.Use(playerIndex);
  }

  public void UseDoubleAttack(PlayerIndex playerIndex)
  {
    doubleAttackItem.Use(playerIndex);
  }

  public void UseHeal(PlayerIndex playerIndex)
  {
    healItem.Use(playerIndex);

  }

}
