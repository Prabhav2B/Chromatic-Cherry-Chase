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

        var minutes = _gameTimer.MinutesLeft < 10 ? "0" + _gameTimer.MinutesLeft : _gameTimer.MinutesLeft.ToString(CultureInfo.InvariantCulture); 
        var seconds = _gameTimer.SecondsLeft < 10 ? "0" + _gameTimer.SecondsLeft : _gameTimer.SecondsLeft.ToString(CultureInfo.InvariantCulture); 
        
        
        _timeText.text = minutes + ":" + seconds;
    }

    private void OnDisable()
    {
        _gameTimer.onTimerTick -= UpdateTimeUI;
    }

    // Update is called once per frame
    void UpdateTimeUI()
    {
        var minutes = _gameTimer.MinutesLeft < 10 ? "0" + _gameTimer.MinutesLeft : _gameTimer.MinutesLeft.ToString(CultureInfo.InvariantCulture); 
        var seconds = _gameTimer.SecondsLeft < 10 ? "0" + _gameTimer.SecondsLeft : _gameTimer.SecondsLeft.ToString(CultureInfo.InvariantCulture); 
        
        _timeText.text = minutes + ":" + seconds;
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
