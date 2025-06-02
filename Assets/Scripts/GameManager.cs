using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [ShowInInspector]
    public GameContext GameContext { get; private set; }
    protected override void InitAfterAwake()
    {
        GameContext = new();
    }
}

[Serializable]
public class GameContext
{
    public GameMode GameMode;

}

public enum GameMode
{
    OnePlayer,
    TwoPlayer
}
