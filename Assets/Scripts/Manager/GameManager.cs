using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sych.ShareAssets.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
  [SerializeField] GameContext gameContext;
  public GameContext GameContext => gameContext;


  protected override void InitAfterAwake()
  {
  }

  void Update()
  {
    if (GameContext.GamePhase == GamePhase.GameStart)
    {
      GameContext.GameTime += Time.deltaTime;
    }
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

    playerManager.InitPlayers();

    if (GameContext.GameMode == GameMode.TwoPlayer)
    {
      PlayerIndex playerIndex = EnumExtension.GetRandomEnum<PlayerIndex>();
      GameContext.CurrentPlayer = playerIndex;
      playerManager.Players[GameContext.CurrentPlayer].StartTurn();
    }
    else
    {
      GameContext.CurrentPlayer = PlayerIndex.Player2;
      playerManager.Players[PlayerIndex.Player2].StartTurn();
    }

    GameContext.GamePhase  = GamePhase.GameStart;

    GameContext.GameTime = 0;
  }

  public void EndGame(PlayerIndex playerLose)
  {
    var playerManager = PlayerManager.Instance;

    CharacterController losePlayer = playerManager.Players[playerLose];
    CharacterController winPlayer = playerManager.GetEnemyCharacter(playerLose);

    losePlayer.SetAnimation(CharacterController.loseAnimation, true);
    winPlayer.SetAnimation(CharacterController.winAnimation, true);

    GameContext.GamePhase  = GamePhase.GameEnd;

    ResultUIManager.Instance.ShowResultUI(winPlayer.PlayerIndex);
  }






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

public enum GamePhase
{
  Idle,
  GameStart,
  GameEnd
}
