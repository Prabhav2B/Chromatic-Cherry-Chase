using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class CollectibleCounter : SingleInstance<CollectibleCounter>
{
    public UnityAction onCollectible;

    private float _collectibleCount;

    public float CollectibleCount => _collectibleCount;


    protected override void OnEnable()
    {
        base.OnEnable();
        onCollectible += UpdateCollectibleCount;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        onCollectible -= UpdateCollectibleCount;
    }

    void UpdateCollectibleCount()
    {
        _collectibleCount++;
    }

    
}
