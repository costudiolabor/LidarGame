using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform target;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private ParticleSystem vfx;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject view;
    [SerializeField] private Collider colliderObject;
    [SerializeField] private float delayDead = 1.0f;
    [SerializeField] private int damage = 1;
    [SerializeField] private int hearth = 10;
    
    private WaitForSeconds _waitForSeconds;
    public event Action<Enemy> DestroyEvent;
    
    public void Initialize()
    {
        _waitForSeconds = new WaitForSeconds(delayDead);
        navMeshAgent.SetDestination(target.position);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        hearth -= damage;
        if (hearth < 1)
        {
            StartCoroutine(Dead());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseMain baseMain))
        {
            baseMain.TakeDamage(damage);
            bool isBase = true;
            StartCoroutine(Dead(isBase));
        }
    }

    private IEnumerator Dead(bool isBase = false)
    {
        vfx.Play();
        audioSource.Play();
        view.SetActive(false);
        colliderObject.enabled = false;
        if (isBase == false) DestroyEvent?.Invoke(this);
        DestroyEvent = null;
        yield return _waitForSeconds;
        Destroy(gameObject);
       
    }
    
}
