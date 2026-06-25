using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class EnemyHandler : IDisposable
{
   [SerializeField] private Transform[] spawnsEnemy;
   [SerializeField] private Transform targetEnemy;
   [SerializeField] private Transform parent;
   [SerializeField] private float delaySeconds = 3.0f;
   [SerializeField] private int maxEnemy = 20;

   private List<Enemy> _enemies = new();
   private bool _isDisposed;
   private int _currentEnemy;
   private ConfigGame _configGame;
   private Enemy _prefabEnemy;
   private PoolObjects<Enemy> _pool;
   private WaitForSeconds _waitForSeconds;
   private MonoBehaviour _mono;

   public List<Enemy> Enemies => _enemies;
   public event Action WinEvent;
   public event Action<int> ChangeEnemyEvent;

   public void Initialize(ConfigGame configGame, ConfigEnemy configEnemy, MonoBehaviour mono)
   {
      _configGame = configGame;
      _configGame.UpdateSettingEvent += OnUpdateSetting;
      _prefabEnemy = configEnemy.PrefabEnemy;
      _mono = mono;

      _pool = new PoolObjects<Enemy>(_prefabEnemy,20, true, parent);
      _waitForSeconds = new WaitForSeconds(delaySeconds);
      
   }

   public void Enable()
   {
      _currentEnemy = maxEnemy;
      ChangeEnemyEvent?.Invoke(_currentEnemy);
      _mono.StartCoroutine(TimerSpawn());
   } 
   

   private void OnUpdateSetting(SettingGame configGame)
   {
      delaySeconds = configGame.delaySeconds;
   }

   private IEnumerator TimerSpawn()
   {
      while (true)
      {
         SpawnEnemy();
         yield return _waitForSeconds;
      }
   }
   
   private void SpawnEnemy()
   {
      int index = Random.Range(0, spawnsEnemy.Length);
      Enemy enemy = _pool.GetFreeElement();
      enemy.transform.SetParent(parent);
      enemy.transform.position = spawnsEnemy[index].position;
      enemy.transform.eulerAngles = spawnsEnemy[index].eulerAngles;
      enemy.Target = targetEnemy;
      enemy.DestroyEvent += OnEnemyDestroyed;
      enemy.Initialize();
      enemy.gameObject.SetActive(true);
      _enemies.Add(enemy);
   }
   
   public void Stop()
   {
      StopEnemies();
      _mono.StopAllCoroutines();
   }

   private void StopEnemies()
   {
      foreach (var enemy in _enemies) {
         if (enemy) enemy.Disable();
      }
   }
   
   private void OnEnemyDestroyed(Enemy enemy, bool isBase)
   {
      _enemies.Remove(enemy);
      if (isBase == true) return;
      _currentEnemy--;
      ChangeEnemyEvent?.Invoke(_currentEnemy);
      if (_currentEnemy < 1)
      {
         Stop();
         WinEvent?.Invoke();
      }
   }
   
   public void Dispose()
   {
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