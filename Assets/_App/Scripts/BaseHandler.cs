using System;
using UnityEngine;

[Serializable]
public class BaseHandler : IDisposable
{
    [SerializeField] private BaseMain baseMain;
    
    public event Action DeadEvent; 
    
    public void Initialize()
    {
        baseMain.DeadEvent += OnDead;
        baseMain.Initialize();
        baseMain.gameObject.SetActive(true);
    }
    
    private void OnDead()
    {
        DeadEvent?.Invoke();
    }
    

    public void Dispose()
    {
        baseMain.DeadEvent -= OnDead;
    }
   
}
