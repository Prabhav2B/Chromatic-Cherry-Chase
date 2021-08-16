using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelResetHandler : MonoBehaviour
{
    public Action onLevelStart;
    public Action onLevelEnd;
    public Action onLevelReload;
    
    private static bool _instantiated;
    
    void Awake()
    {
        //Debug.Assert(!_instantiated, this.gameObject);
        if (_instantiated)
        {
            Destroy(this);
        }

        _instantiated = true;
        DontDestroyOnLoad(gameObject);
    }

    public void ExecuteLevelReload()
    {
        onLevelReload?.Invoke();
    }
    
    public void ExecuteLevelEnd()
    {
        onLevelEnd?.Invoke();
    }


}