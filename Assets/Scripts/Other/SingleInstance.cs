using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleInstance<T> : MonoBehaviour
{
    private static bool _instantiated;
    protected LevelResetHandler _levelResetHandler;
    protected virtual void Awake()
    {
        //Debug.Assert(!_instantiated, this.gameObject);
        if (_instantiated)
        {
            Destroy(gameObject);
            return;
        }

        _instantiated = true;

       
        var instances = FindObjectsOfType<LevelResetHandler>(false);
        foreach (var instance in instances)
        {
            if (!instance.IsStaticInstance) continue;
            _levelResetHandler = instance;
            break;
        }
        _levelResetHandler.enabled = true;
        //_levelResetHandler.onLevelEnd += DestroyInstance;
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
        //Destroy(this);
    }
}