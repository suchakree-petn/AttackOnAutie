using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SpecialItem : ScriptableObject
{
    public abstract void UseAsync(PlayerIndex playerIndex);
}
