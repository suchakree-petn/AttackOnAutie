using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerThrow", menuName = "Special Items/Power Throw")]
public class PowerThrow : SpecialItem
{
    [SerializeField] ThrowType throwType = ThrowType.Power;



    public override void UseAsync(PlayerIndex playerIndex)
    {
        CharacterController characterController = PlayerManager.Instance.Players[playerIndex];

        characterController.SetThrowType(throwType);
    }

    public override void Discard(PlayerIndex playerIndex)
    {
        CharacterController characterController = PlayerManager.Instance.Players[playerIndex];

        characterController.SetThrowType(ThrowType.Normal);
    }
}
