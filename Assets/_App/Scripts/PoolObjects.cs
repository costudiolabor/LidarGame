using System.Collections.Generic;
using UnityEngine;
using System;

public class PoolObjects<T> where T : Component
{
    private List<T> _pool;
    private readonly bool _canExpand = false;
    private readonly Transform _parent;
    private readonly T _prefab;

    public PoolObjects(T prefab, int poolAmount, bool canExpand, Transform parent)
    {
        this._canExpand = canExpand;
        this._parent = parent;
        this._prefab = prefab;

        CreatePool(poolAmount);
    }

    private void CreatePool(int poolAmount)
    {
        _pool = new List<T>();

        for (int i = 0; i < poolAmount; i++)
            CreateElement();
    }

    private T CreateElement(bool isActiveAsDefault = false)
    {
        var createdObj = UnityEngine.Object.Instantiate(_prefab, _parent);
        createdObj.gameObject.SetActive(isActiveAsDefault);
        _pool.Add(createdObj);
        return createdObj;
    }

    public bool HasFreeElement(out T element)
    {
        foreach (var obj in _pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                element = obj;
                element.gameObject.SetActive(true);
                return true;
            }
        }

        element = null;
        return false;
    }

    public T GetFreeElement()
    {
        if (HasFreeElement(out var element))
            return element;
        if (_canExpand)
            return CreateElement(true);
        throw new Exception($"В пуле закончились {typeof(T)}");
    }

    public T[] GetElements() => _pool.ToArray() as T[];
}
