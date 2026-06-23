using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class GameHandler : IDisposable
{
  [SerializeField] private CanvasHandler canvasHandler;
  [SerializeField] private BaseHandler baseHandler;
  [SerializeField] private EnemyHandler enemyHandler;
  [SerializeField] private TowersHandler towersHandler;

  private ConfigGame _configGame;
  public void Initialize(ConfigGame configGame)
  {
    _configGame = configGame;
    canvasHandler.StartEvent += OnStart;
    canvasHandler.ExitEvent += OnExit;
    canvasHandler.AgainEvent += OnAgain;
    canvasHandler.NextEvent += OnNext;
    baseHandler.DeadEvent += OnDead;
    enemyHandler.WinEvent += OnWin;
    enemyHandler.ChangeEnemyEvent += OnChangeEnemy;
    
    enemyHandler.Initialize(_configGame);
    towersHandler.Initialize(enemyHandler, _configGame);
  }

  private void OnChangeEnemy(int value)
  {
    canvasHandler.UpdateEnemy(value);
  }

  private void OnStart()
  {
    baseHandler.Enable();
    enemyHandler.Enable();
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
    canvasHandler.ShowLose();
    enemyHandler.Stop();
    towersHandler.Stop();
  } 
  
  private void OnWin()
  {
    canvasHandler.ShowWin();
    enemyHandler.Stop();
    towersHandler.Stop();
  }

  public void RestartGame()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  public void Dispose()
  {
    canvasHandler.StartEvent -= OnStart;
    canvasHandler.ExitEvent -= OnExit;
    canvasHandler.AgainEvent -= OnAgain;
    canvasHandler.NextEvent -= OnNext;
    baseHandler.DeadEvent -= OnDead;

    baseHandler.Dispose();
    enemyHandler.Dispose();

  }
  
}
