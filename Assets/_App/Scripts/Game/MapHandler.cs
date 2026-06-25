using UnityEngine;

public class MapHandler : MonoBehaviour
{
  [SerializeField] private BaseHandler baseHandler;
  [SerializeField] private EnemyHandler enemyHandler;
  [SerializeField] private TowersHandler towersHandler;

  private CanvasHandler _canvasHandler;
  public void Initialize(CanvasHandler canvasHandler, ConfigGame configGame, ConfigEnemy configEnemy, ConfigTower configTower)
  {
    _canvasHandler = canvasHandler;
    _canvasHandler.ExitEvent += OnExit;
    _canvasHandler.AgainEvent += OnAgain;
    _canvasHandler.NextEvent += OnNext;
    
    baseHandler.DeadEvent += OnDead;
    
    enemyHandler.WinEvent += OnWin;
    enemyHandler.ChangeEnemyEvent += OnChangeEnemy;

    enemyHandler.Initialize(configGame, configEnemy, this);
    towersHandler.Initialize(enemyHandler, configGame, configTower);
    
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
