using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    private GameTimer _gameTimer;
    private TMP_Text _timeText;
   
    
    void Start()
    {
        _timeText = GetComponent<TMP_Text>();
        _gameTimer = FindObjectOfType<GameTimer>();
        _gameTimer.onTimerTick += UpdateTimeUI;

        _timeText.text = $"{_gameTimer.MinutesLeft:00}:{_gameTimer.SecondsLeft:00}";
    }

    private void OnDisable()
    {
        _gameTimer.onTimerTick -= UpdateTimeUI;
    }

    // Update is called once per frame
    void UpdateTimeUI()
    {
        _timeText.text = $"{_gameTimer.MinutesLeft:00}:{_gameTimer.SecondsLeft:00}";
        TextPopA();
        
    }

    void TextPopA()
    {
        _timeText.transform.DOScale(new Vector3(1.05f, 1.05f, 1.05f), .15f).OnComplete(TextPopB);
    }
    
    void TextPopB()
    {
        _timeText.transform.DOScale(new Vector3(1.00f, 1.00f, 1.00f), .1f);
    }
}
