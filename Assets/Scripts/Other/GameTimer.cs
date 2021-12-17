using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class GameTimer : SingleInstance<GameTimer>
{
    [SerializeField] private Image timerImage;

    [SerializeField] private TimeLimits timeLimits;

    private ScreenFadeManager _screenFadeManager;
    private PlayerManager _playerManager;


    private float _totalTime;
    private float _currentTime;
    private bool _stopTimer;
    private EndCardManager _endCardManager;

    public float TotalTime => _totalTime;
    public float MinutesLeft => Mathf.Floor(_currentTime / 60f);
    public float SecondsLeft => Mathf.Floor(_currentTime % 60f);

    public UnityAction onTimerExpired;
    public UnityAction onTimerTick;


    protected override void Awake()
    {
        base.Awake();
        _screenFadeManager = FindObjectOfType<ScreenFadeManager>();
        _playerManager = FindObjectOfType<PlayerManager>();
        _endCardManager = FindObjectOfType<EndCardManager>();
    }

    private void Start()
    {
        var randomTimeLimit = timeLimits.timeLimits[Random.Range(0, timeLimits.timeLimits.Count)];

        
        _endCardManager.LinkText = randomTimeLimit.surveyURL;

        _totalTime = randomTimeLimit.minutes * 60f + randomTimeLimit.seconds;
        

        var openURL = gameObject.AddComponent<OpenURL>();
        
#if !UNITY_EDITOR && UNITY_WEBGL
        onTimerExpired += () => openURL.OpenIt(randomTimeLimit.surveyURL);
#else
        onTimerExpired += () => Application.OpenURL(randomTimeLimit.surveyURL);
#endif

        //_totalTime = 15;
        _currentTime = _totalTime;
        StartTimer();
        onTimerTick?.Invoke();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _levelResetHandler.onLevelReload += ResetTimer;
        onTimerExpired += _endCardManager.Activate;
        onTimerExpired += _screenFadeManager.FadeOut;
        onTimerExpired += _playerManager.Deactivate;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _levelResetHandler.onLevelReload -= ResetTimer;
        
        onTimerExpired -= _endCardManager.Activate;
        onTimerExpired -= _screenFadeManager.FadeOut;
        onTimerExpired -= _playerManager.Deactivate;
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

    public void StopTimer()
    {
        _stopTimer = true;
    }

    IEnumerator Tick()
    {
        while (_currentTime > 0 && !_stopTimer)
        {
            yield return new WaitForSecondsRealtime(1.0f);
            _currentTime--;
            onTimerTick?.Invoke();
            timerImage.fillAmount = _currentTime / _totalTime;
        }

        if (!_stopTimer)
            onTimerExpired?.Invoke();
    }
}