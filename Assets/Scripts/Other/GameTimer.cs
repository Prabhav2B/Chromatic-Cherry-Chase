using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameTimer : SingleInstance<GameTimer>
{
    [SerializeField] private Image timerImage;
    
    [SerializeField, Range(0, 100)] private int timeInMinutes = 1;
    [SerializeField, Range(0, 59)] private int timeInSeconds = 30;

    private float _totalTime;
    private float _currentTime;
    
    public float TotalTime => _totalTime;
    public float MinutesLeft => Mathf.Floor(_currentTime / 60f);
    public float SecondsLeft => Mathf.Floor(_currentTime % 60f);

    public UnityAction onTimerExpired;
    public UnityAction onTimerTick;
    

    private void Start()
    {
        var levelResetHandler = FindObjectOfType<LevelResetHandler>();
        levelResetHandler.onLevelReload += ResetTimer;
    
        _totalTime = timeInMinutes * 60f + timeInSeconds;
        _currentTime = _totalTime;
        StartTimer();
        onTimerTick?.Invoke();
    }

    void StartTimer()
    {
        StopCoroutine(Tick());
        StartCoroutine(Tick());
    }

    void ResetTimer()
    {
        _currentTime = _totalTime;
    }

    IEnumerator Tick()
    {
        while (_currentTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            _currentTime--;
            onTimerTick?.Invoke();
            timerImage.fillAmount = _currentTime / _totalTime;
        }

        onTimerExpired?.Invoke();
        Debug.Log("Time Up!");
    }
}
