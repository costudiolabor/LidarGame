using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[Serializable]
public class EnemyHandler : IDisposable
{
   [SerializeField] private Transform[] spawnsEnemy;
   [SerializeField] private Transform targetEnemy;
   [SerializeField] private Enemy prefabEnemy;
   [SerializeField] private float delaySeconds = 3.0f;
   [SerializeField] private int maxEnemy = 20;

   private List<Enemy> _enemies = new List<Enemy>();
   private CancellationTokenSource _cancellationToken;
   private bool _isDisposed;
   private int _currentEnemy;
   private ConfigGame _configGame;
   public List<Enemy> Enemies => _enemies;
   public event Action WinEvent;
   public event Action<int> ChangeEnemyEvent;
   

   public void Initialize(ConfigGame configGame)
   {
      _configGame = configGame;
      _configGame.UpdateSettingEvent += OnUpdateSetting;
   }

   public void Enable()
   {
      _cancellationToken?.Cancel();
      _cancellationToken?.Dispose();
      _cancellationToken = new CancellationTokenSource();
      TimerSpawnAsync(_cancellationToken.Token).Forget();
      _currentEnemy = maxEnemy;
      ChangeEnemyEvent?.Invoke(_currentEnemy);
   } 

   private void OnUpdateSetting(SettingGame configGame)
   {
      delaySeconds = configGame.delaySeconds;
   }
   
   private async UniTaskVoid TimerSpawnAsync(CancellationToken ct)
   {
      try
      {
         while (!ct.IsCancellationRequested && !_isDisposed)
         {
            await UniTask.Delay(
               TimeSpan.FromSeconds(delaySeconds),
               cancellationToken: ct
            );
            
            if (_isDisposed || ct.IsCancellationRequested)
               break;
               
            SpawnEnemy();
         }
      }
      catch (OperationCanceledException)
      {
         return;
      }
      catch (Exception ex)
      {
         Debug.LogError($"Error in TimerSpawnAsync: {ex.Message}");
      }
   }
   
   private void SpawnEnemy()
   {
      if (_isDisposed) return;
      if (prefabEnemy == null) return;
      if (spawnsEnemy == null || spawnsEnemy.Length == 0) return;
      int index = Random.Range(0, spawnsEnemy.Length);
      if (spawnsEnemy[index] == null)
      {
         Debug.LogWarning($"Spawn point at index {index} is null");
         return;
      }
      
      try
      {
         Enemy enemy = Object.Instantiate(prefabEnemy);
         
         if (enemy == null) return;
         
         enemy.transform.position = spawnsEnemy[index].position;
         enemy.transform.eulerAngles = spawnsEnemy[index].eulerAngles;
         enemy.Target = targetEnemy;
         
         _enemies.Add(enemy);
         enemy.DestroyEvent += OnEnemyDestroyed;
         enemy.Initialize();
       
      }
      catch (MissingReferenceException ex)
      {
         Debug.LogError($"MissingReferenceException in SpawnEnemy: {ex.Message}");
      }
   }
   
   public void Stop()
   {
      _cancellationToken?.Cancel();
      StopEnemies();
   }


   private void StopEnemies()
   {
      foreach (var enemy in _enemies) {
         if (enemy) enemy.Disable();
      }
   }
   
   private void OnEnemyDestroyed(Enemy enemy)
   {
      if (_enemies.Contains(enemy))
      {
         _enemies.Remove(enemy);
         _currentEnemy--;
         ChangeEnemyEvent?.Invoke(_currentEnemy);
         if (_currentEnemy < 1)
         {
            Stop();
            WinEvent?.Invoke();
         }
      }
   }
   
   public void Dispose()
   {
      if (_isDisposed) return;
      _isDisposed = true;
      
      _cancellationToken?.Cancel();
      _cancellationToken?.Dispose();
      _cancellationToken = null;
      
      foreach (var enemy in _enemies)
      {
         if (enemy != null)
         {
            enemy.DestroyEvent -= OnEnemyDestroyed;
         }
      }
      
      _enemies.Clear();
   }
}