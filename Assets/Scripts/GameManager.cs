using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameContext gameContext;
    public GameContext GameContext => gameContext;




    protected override void InitAfterAwake()
    {
    }


    private void Start()
    {
    }


    [Button]
    public void StartGame()
    {
        if (!PlayerManager.Instance)
        {
            Debug.Log(nameof(PlayerManager) + " is null");
            return;
        }


        if (GameContext.GameMode == GameMode.TwoPlayer)
        {
            PlayerIndex playerIndex = EnumExtension.GetRandomEnum<PlayerIndex>();
            GameContext.CurrentPlayer = playerIndex;
            PlayerManager.Instance.Players[playerIndex].StartTurn();
        }
        else
        {
            GameContext.CurrentPlayer = PlayerIndex.Player2;
            PlayerManager.Instance.Players[PlayerIndex.Player2].StartTurn();
        }
    }




}

[Serializable]
public class GameContext
{
    public GameMode GameMode;
    public PlayerIndex CurrentPlayer;

    [InlineEditor]
    public GameConfig GameConfig;

}

public enum GameMode
{
    OnePlayer,
    TwoPlayer
}

public enum PlayerIndex
{
    Player1,
    Player2
}
