using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionToMainLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var _levelResetHandler = FindObjectOfType<LevelResetHandler>();
        var _screenFadeManager = FindObjectOfType<ScreenFadeManager>();
        
        
        ScreenFadeManager.PostFadeOut fadeOutAction = _levelResetHandler.ExecuteLevelEnd;
        SceneTransitionManagement s = FindObjectOfType<SceneTransitionManagement>();
        fadeOutAction += s.LoadNextLevel;
        _screenFadeManager.FadeOut(fadeOutAction);
        _levelResetHandler.onLevelReload = null;
        fadeOutAction = null;
    }
}
