using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private ParticleSystem vfx;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject view;
    [SerializeField] private Collider colliderObject;
    [SerializeField] private float delayDead = 1.0f;
    [SerializeField] private int damage = 1;
    [SerializeField] private int hearth = 10;
    [SerializeField] private float smoothSpeed = 5f;

    private WaitForSeconds _waitForSeconds;
    
    public Transform Target {get; set;}
    public event Action<Enemy, bool> DestroyEvent;

    private void OnEnable()
    {
        view.SetActive(true);
        colliderObject.enabled = true;
    }

    public void Initialize()
    {
        _waitForSeconds = new WaitForSeconds(delayDead);
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Target.position,smoothSpeed * Time.deltaTime);
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
        DestroyEvent?.Invoke(this, isBase);
        yield return _waitForSeconds;
        Disable();
    }
    
}