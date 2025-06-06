using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Special Items/Heal")]
public class Heal : SpecialItem
{

    public override async void Use(PlayerIndex playerIndex)
    {
        CharacterController characterController = PlayerManager.Instance.Players[playerIndex];
        var config = GameManager.Instance.GameContext.GameConfig.Config;
        characterController.Heal(config["Heal"].HP);
        characterController.SetThrowAmount(0);
        Debug.Log("heal");
        await UniTask.WaitForSeconds(2f);

        characterController.EndTurn();

    }


}
