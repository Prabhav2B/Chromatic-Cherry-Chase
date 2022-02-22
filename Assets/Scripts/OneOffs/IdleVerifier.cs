using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class IdleVerifier : MonoBehaviour
{
    private GameTimer _gameTimer;
    private LevelResetHandler _levelResetHandler;
    private int playerLayer;

    private GameObject idleUI;

    private void Awake()
    {
        idleUI = GetComponentInChildren<Canvas>(true).gameObject;

        playerLayer = LayerMask.NameToLayer("Player");
        _gameTimer = FindObjectOfType<GameTimer>();
        _levelResetHandler = FindObjectOfType<LevelResetHandler>();

        var trigger = GetComponent<BoxCollider2D>();
        trigger.isTrigger = true;
    }

    private void OnEnable()
    {
        idleUI.SetActive(false);
        _gameTimer.onIdleDetected += IdleMessage;
        _levelResetHandler.onLevelReload += ResetIdleMessage;
    }

    private void OnDisable()
    {
        _gameTimer.onIdleDetected -= IdleMessage;
        _levelResetHandler.onLevelReload -= ResetIdleMessage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            _gameTimer.Idle = false;
        }
    }

    private void IdleMessage()
    {
        idleUI.SetActive(true);
    }
    
    private void ResetIdleMessage()
    {
        idleUI.SetActive(false);
    }
}