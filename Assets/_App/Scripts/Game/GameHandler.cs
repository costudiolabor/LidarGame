using UnityEngine;

public class GameHandler : MonoBehaviour
{
  [SerializeField] private BaseHandler baseHandler;
  [SerializeField] private EnemyHandler enemyHandler;
  [SerializeField] private TowersHandler towersHandler;

  private CanvasHandler _canvasHandler;
  private ConfigGame _configGame;
  public void Initialize(CanvasHandler canvasHandler, ConfigGame configGame, OSCHandler oscHandler)
  {
    _configGame = configGame;

    _canvasHandler = canvasHandler;
    _canvasHandler.ExitEvent += OnExit;
    _canvasHandler.AgainEvent += OnAgain;
    _canvasHandler.NextEvent += OnNext;
    
    baseHandler.DeadEvent += OnDead;
    
    enemyHandler.WinEvent += OnWin;
    enemyHandler.ChangeEnemyEvent += OnChangeEnemy;

    enemyHandler.Initialize(_configGame);
    towersHandler.Initialize(enemyHandler, _configGame);
    
    baseHandler.Enable();
    enemyHandler.Enable();
  }

  private void OnChangeEnemy(int value)
  {
    _canvasHandler.UpdateEnemy(value);
  }

  private void OnExit()
  {
    RestartGame();
  }

  private void OnAgain()
  {
    RestartGame();
  }

  private void OnNext()
  {
    RestartGame();
  }
  
  private void OnDead()
  {
    _canvasHandler.ShowLose();
    enemyHandler.Stop();
    towersHandler.Stop();
  } 
  
  private void OnWin()
  {
    _canvasHandler.ShowWin();
    enemyHandler.Stop();
    towersHandler.Stop();
  }

  private void RestartGame()
  {
     Destroy(this.gameObject);
  }

  public void OnDestroy()
  {
    _canvasHandler.ExitEvent -= OnExit;
    _canvasHandler.AgainEvent -= OnAgain;
    _canvasHandler.NextEvent -= OnNext;
    
    baseHandler.DeadEvent -= OnDead;

    baseHandler.Dispose();
    enemyHandler.Dispose();

  }
  
}
