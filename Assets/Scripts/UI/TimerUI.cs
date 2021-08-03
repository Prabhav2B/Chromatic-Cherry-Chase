using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    private GameTimer _gameTimer;
    void Start()
    {
        _gameTimer = FindObjectOfType<GameTimer>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(_gameTimer.MinutesLeft + " : " + _gameTimer.SecondsLeft);
    }
}
