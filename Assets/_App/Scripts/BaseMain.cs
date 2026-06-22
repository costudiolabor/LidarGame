using System;
using TMPro;
using UnityEngine;

public class BaseMain : MonoBehaviour
{
    [SerializeField] private AudioSource audioSourceDead;
    [SerializeField] private AudioSource audioSourceReload;
    [SerializeField] private ParticleSystem vfx;
    [SerializeField] private int health = 10;
    [SerializeField] private int countReloadBullet = 10;
    [SerializeField] private TMP_Text textHealth;
    [SerializeField] private float delayDead = 1.0f;

    private bool _isDeath;
    public event Action DeadEvent; 

    public void Initialize()
    {
        ShowHealth();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        ShowHealth();
        if (health > 0) return;
        Dead();
    }

    private void ShowHealth()
    {
        textHealth.text = health.ToString();
    }
    
    private void Dead()
    {
        if (_isDeath) return;
        vfx.Play();
        audioSourceDead.Play();
        _isDeath = true;
        Invoke(nameof(DelayDead), delayDead);
    }

    private void DelayDead()
    {
        DeadEvent?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (health < 1) return;
            if (player.IsLoad) return;
            audioSourceReload.Play();
            player.UpdateBullet(countReloadBullet);
            player.IsLoad = true;
        }
    }
    
    
}
