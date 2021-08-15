using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelResetHandler : SingleInstance<LevelResetHandler>
{
    public Action onLevelStart;
    public Action onLevelEnd;
    private Action onLevelReload;
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    protected override void OnEnable()
    {
        //DOES NOTHING ON PURPOSE
        //THIS ONE PERSISTS AND MANAGES OVERALL GAME ACTIVITY
    }
    
    protected override void OnDisable()
    {
        //DOES NOTHING ON PURPOSE
        //THIS ONE PERSISTS AND MANAGES OVERALL GAME ACTIVITY
    }

    public void ExecuteLevelReload()
    {
        onLevelReload?.Invoke();
    }


}