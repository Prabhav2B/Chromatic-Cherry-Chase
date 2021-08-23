using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinished : MonoBehaviour
{
    private ScreenFadeManager _screenFadeManager;
    private PlayerManager _playerManager;
    private GameTimer _gameTimer;

    private void Awake()
    {
        _screenFadeManager = FindObjectOfType<ScreenFadeManager>();
        _playerManager = FindObjectOfType<PlayerManager>();
        _gameTimer = FindObjectOfType<GameTimer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _gameTimer.StopTimer();
        StartCoroutine(EndGame());
        GetComponent<CircleCollider2D>().enabled = false;
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(8f);
        _gameTimer.onTimerExpired();
    }
}
