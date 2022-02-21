using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ControlSchemeChangeBroadcaster : MonoBehaviour
{
    private PlayerManager _playerManager;

    private PlayerManager.ControlScheme _lastControlScheme;

    public UnityAction<PlayerManager.ControlScheme> onControlSchemeChange;

    void Start()
    {
        _playerManager = FindObjectOfType<PlayerManager>();

        _lastControlScheme = _playerManager.CurrentControlScheme;
        onControlSchemeChange?.Invoke(_lastControlScheme);
    }


    void Update()
    {
        //this can be avoided somewhere somehow right?
        if (_playerManager == null)
        {
            _playerManager = FindObjectOfType<PlayerManager>();
            return;
        }

        if (_lastControlScheme == _playerManager.CurrentControlScheme)
            return;

        _lastControlScheme = _playerManager.CurrentControlScheme;
        onControlSchemeChange?.Invoke(_lastControlScheme);
    }
}