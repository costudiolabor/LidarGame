using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TowerMain : MonoBehaviour
{
    [SerializeField] private EnemyHandler enemyHandler;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform gun;
    [SerializeField] private Transform parentBullet;
    [SerializeField] private float delayFire = 1.0f;
    [SerializeField] private float speedBullet;
    [SerializeField] private AudioSource audioSourceFire;
    [SerializeField] private AudioSource audioSourceReload;
    [SerializeField] private TMP_Text textBullet;

    [SerializeField] private int countBullets = 20;
    
    private WaitForSeconds _delay;
    private bool _isTarget;

    private Transform _currentTarget;

    public bool IsPlay { get; set; } = true;
    private void OnEnable()
    {
        delayFire += Random.Range(-0.1f, 0.1f);
        _delay = new WaitForSeconds(delayFire);
        StartCoroutine(TimerFire());
        ShowBullet();
    }

    public void SetDelayFire(float delay) => delayFire = delay; 

    private void ShowBullet()
    {
        textBullet.text = countBullets.ToString();
    }
    
    public void SetEnemyHandler(EnemyHandler enemyHandler) => this.enemyHandler = enemyHandler; 
    private void Update()
    {
        _currentTarget = GetNearestTarget(transform.position);
        if (_currentTarget != null)
        {
            gun.LookAt(_currentTarget);
            _isTarget = true;
        }
        else _isTarget = false;
    }
    
    private Transform GetNearestTarget(Vector3 referencePoint)
    {
        Transform closestTarget = null;
        int count = enemyHandler.Enemies.Count;
        float minDistanceSqr = Mathf.Infinity; 
        for (int i = 0; i < count; i++)
        {
            if (enemyHandler.Enemies[i] == null) 
                continue;
            float distanceSqr = (enemyHandler.Enemies[i].transform.position - referencePoint).sqrMagnitude;
            if (distanceSqr < minDistanceSqr)
            {
                minDistanceSqr = distanceSqr;
                closestTarget = enemyHandler.Enemies[i].transform;
            }
        }

        return closestTarget;
    }

    private IEnumerator TimerFire()
    {
        while (IsPlay)
        {
            if (_isTarget)
            {
                if (countBullets > 0) Fire();
            }
            yield return _delay;
        }
    }
    
    private void Fire()
    {
        Bullet bullet = GetBullet();
        Vector3 shootDirection = parentBullet.forward;
        bullet.Initialize(shootDirection, speedBullet);
        audioSourceFire.Play();
        ShowBullet();
    }
    
    private Bullet GetBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, parentBullet);
        countBullets--;
        return bullet;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (player.IsLoad == true)
            {
                audioSourceReload.Play();
                countBullets += player.GetBullet();
                player.UpdateBullet(0);
                player.IsLoad = false;
                ShowBullet();
            }
        }
    }
    
}
