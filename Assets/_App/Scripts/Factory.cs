using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class Factory {
    
    public T Get<T>(T original, Transform parent) where T : Object {
        return (T)Object.Instantiate((Object)original, parent);
    }
}