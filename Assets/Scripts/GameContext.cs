using System;
using Sirenix.OdinInspector;

[Serializable]
public class GameContext
{
  public GameMode GameMode;
  public Difficulty Difficulty;
  public PlayerIndex CurrentPlayer;
  public GamePhase GamePhase = GamePhase.Idle;
  public float GameTime;

  [InlineEditor]
  public GameConfig GameConfig;

}
