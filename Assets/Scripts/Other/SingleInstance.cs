using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleInstance<T> : MonoBehaviour
{
    private static bool _instantiated;
    protected LevelResetHandler _levelResetHandler;
    void Awake()
    {
        Debug.Assert(!_instantiated, this.gameObject);
        if (_instantiated)
        {
            Destroy(this);
        }

        _instantiated = true;

        _levelResetHandler = FindObjectOfType<LevelResetHandler>();
    }

    protected virtual void OnEnable()
    {
        _levelResetHandler.onLevelEnd += DestroyInstance;
    }

    protected virtual void OnDisable()
    {
        _levelResetHandler.onLevelEnd -= DestroyInstance;
    }

    private void DestroyInstance()
    {
        _instantiated = false;
        this.enabled = false;
        Destroy(this);
    }
}