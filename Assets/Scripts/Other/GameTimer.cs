using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class GameTimer : MonoBehaviour
{
    [SerializeField, Range(0, 100)] private int timeInMinutes = 1;
    [SerializeField, Range(0, 59)] private int timeInSeconds = 30;

    private float _totalTime;

    public float TotalTime => _totalTime;
    public float MinutesLeft => Mathf.Floor(_totalTime / 60f);
    public float SecondsLeft => Mathf.Floor(_totalTime % 60f);

    public UnityAction onTimerExpired;
    public UnityAction onTimerTick;

    private static bool _instantiated;

    void Awake()
    {
        Debug.Assert(!_instantiated, this.gameObject);
        if (_instantiated)
        {
            Destroy(this);
        }
        _instantiated = true;
    }

    private void Start()
    {
        _totalTime = timeInMinutes * 60f + timeInSeconds;
        StartTimer();
        onTimerTick?.Invoke();
    }

    void StartTimer()
    {
        StopCoroutine(Tick());
        StartCoroutine(Tick());
    }

    IEnumerator Tick()
    {
        while (_totalTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            _totalTime--;
            onTimerTick?.Invoke();
        }

        onTimerExpired?.Invoke();
        Debug.Log("Time Up!");
    }
}
