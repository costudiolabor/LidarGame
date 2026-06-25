using UnityEngine;

public class PlaceTower : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private EnemyHandler _enemyHandler;
    private TowerMain _towerPrefab;
    private bool _isCreate;
    private TowerMain _towerMain;
    private ConfigGame _configGame;
    private Factory _factory;

    public void Initialize(EnemyHandler enemyHandler, ConfigTower configTower, ConfigGame configGame)
    {
        _enemyHandler = enemyHandler;
        _towerPrefab = configTower.PrefabTowerMain;
        _configGame = configGame;
        _factory = new Factory();
        CreateTower(configTower);
        _configGame.UpdateSettingEvent += OnUpdateSetting;
    }

    private void OnUpdateSetting(SettingGame configGame)
    {
        float delay = configGame.delayFire;
        _towerMain.SetDelayFire(delay);
    }
    
    public void IsPlay(bool state)
    {
        _towerMain.IsPlay = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            ActivateTower();
        }
    }

    private void CreateTower(ConfigTower configTower)
    {
        _towerMain = _factory.Get(_towerPrefab, transform);
        _towerMain.Initialize(_enemyHandler, configTower);
        _towerMain.gameObject.SetActive(false);
        Debug.Log("CreateTower");
        _isCreate = true;
       
    }

    private void ActivateTower()
    {
        if (_isCreate == false) return;
        Debug.Log("ActivateTower");
        if (_towerPrefab == null) return;
        _towerMain.gameObject.SetActive(true);
        audioSource.Play();
    }

}
