using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class GameManager : Singleton<GameManager>
{
    [ShowInInspector]
    public GameContext GameContext { get; private set; }


    protected override void InitAfterAwake()
    {
    }
    

}

[Serializable]
public class GameContext
{
    public GameMode GameMode;

    [InlineEditor]
    public GameConfig GameConfig;

}

public enum GameMode
{
    OnePlayer,
    TwoPlayer
}
