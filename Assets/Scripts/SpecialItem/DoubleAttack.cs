using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoubleAttack", menuName = "Special Items/Double Attack")]
public class DoubleAttack : SpecialItem
{

    public override void UseAsync(PlayerIndex playerIndex)
    {
        CharacterController characterController = PlayerManager.Instance.Players[playerIndex];
        var config = GameManager.Instance.GameContext.GameConfig.Config;
        characterController.SetThrowAmount(config["Double Attack"].Amount);

    }


}
