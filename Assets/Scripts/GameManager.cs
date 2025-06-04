using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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
    PlayerManager playerManager = PlayerManager.Instance;
    if (!playerManager)
    {
      Debug.Log(nameof(PlayerManager) + " is null");
      return;
    }

    InitPlayers(playerManager);

    if (GameContext.GameMode == GameMode.TwoPlayer)
    {
      PlayerIndex playerIndex = EnumExtension.GetRandomEnum<PlayerIndex>();
      GameContext.CurrentPlayer = playerIndex;
      playerManager.Players[playerIndex].StartTurn();
    }
    else
    {
      GameContext.CurrentPlayer = PlayerIndex.Player2;
      playerManager.Players[PlayerIndex.Player2].StartTurn();
    }
  }

  private void InitPlayers(PlayerManager playerManager)
  {
    if (gameContext.GameMode == GameMode.OnePlayer)
    {
      playerManager.Players[PlayerIndex.Player1].Init(true);
      playerManager.Players[PlayerIndex.Player2].Init(false);
    }
    else
    {
      playerManager.Players[PlayerIndex.Player1].Init(false);
      playerManager.Players[PlayerIndex.Player2].Init(false);
    }
  }
}

[Serializable]
public class GameContext
{
    public GameMode GameMode;
    public Difficulty Difficulty;
    public PlayerIndex CurrentPlayer;

    [InlineEditor]
    public GameConfig GameConfig;

}

public enum GameMode
{
    OnePlayer,
    TwoPlayer
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

public enum PlayerIndex
{
    Player1,
    Player2
}
