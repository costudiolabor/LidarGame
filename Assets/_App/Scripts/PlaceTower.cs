using UnityEngine;

public class PlaceTower : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private EnemyHandler _enemyHandler;
    private TowerMain _towerPrefab;
    private bool _isCreate;
    private TowerMain _towerMain;

    public void Initialize(EnemyHandler enemyHandler,  TowerMain towerPrefab)
    {
        _enemyHandler = enemyHandler;
        _towerPrefab = towerPrefab;
        CreateTower();
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

    private void CreateTower()
    {
        _towerMain = Instantiate(_towerPrefab, transform);
        _towerMain.SetEnemyHandler(_enemyHandler);
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
