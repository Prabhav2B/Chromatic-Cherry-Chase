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

    private void Start()
    {
        var levelResetHandler = FindObjectOfType<LevelResetHandler>();
        levelResetHandler.onLevelReload += ResetCounter;
    }

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

    private void ResetCounter()
    {
        _collectibleCount = 0;
    }
}
