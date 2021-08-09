using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollectibleCounter : MonoBehaviour
{
    public UnityAction onCollectible;

    private float _collectibleCount;

    public float CollectibleCount => _collectibleCount;
    
    private static bool _instantiated;

    void Awake()
    {
        Debug.Assert(!_instantiated, this.gameObject);
        if (_instantiated)
        {
            Destroy(this);
        }
        _instantiated = true;
    }
    private void Start()
    {
        onCollectible += UpdateCollectibleCount;
    }

    void UpdateCollectibleCount()
    {
        _collectibleCount++;
    }
}
