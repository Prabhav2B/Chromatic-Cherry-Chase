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
    private MainMenu _mainMenu;


    private float _totalTime;
    private float _currentTime;
    private bool _stopTimer;
    private EndCardManager _endCardManager;

    public float TotalTime => _totalTime;
    public float MinutesLeft => Mathf.Floor(_currentTime / 60f);
    public float SecondsLeft => Mathf.Floor(_currentTime % 60f);

    public bool Idle { get; set; }

    public UnityAction onTimerExpired;
    public UnityAction onTimerTick;
    public UnityAction<int> onTimerEvent;
    public UnityAction onIdleDetected;


    protected override void Awake()
    {
        base.Awake();
        _screenFadeManager = FindObjectOfType<ScreenFadeManager>();
        _playerManager = FindObjectOfType<PlayerManager>();
        _endCardManager = FindObjectOfType<EndCardManager>();
        _mainMenu = FindObjectOfType<MainMenu>();
    }

    private void Start()
    {
        Idle = true;
        
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
        //_levelResetHandler.onLevelReload += StartTimer;
        
        onTimerExpired += _endCardManager.Activate;
        onTimerExpired += _screenFadeManager.FadeOut;
        onTimerExpired += _playerManager.Deactivate;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _levelResetHandler.onLevelReload -= ResetTimer;
        //_levelResetHandler.onLevelReload -= StartTimer;
        
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
        _currentTime = TotalTime;
        StopAllCoroutines();
        StartCoroutine(Tick());
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

            if (_mainMenu.Paused)
            {
                continue;
            }

            _currentTime--;
            onTimerTick?.Invoke();
            timerImage.fillAmount = _currentTime / _totalTime;

            CheckForTimeEvent();

        }

        if (!_stopTimer)
        {
            if (Idle)
            {
                onIdleDetected?.Invoke();
            }
            else
            {
                onTimerExpired?.Invoke();
            }
        }
    }

    private void CheckForTimeEvent()
    {
        switch (Mathf.CeilToInt(_currentTime))
        {
            case 180:
            {
                onTimerEvent?.Invoke(3);
                break;
            }
            case 120:
            {
                onTimerEvent?.Invoke(2);
                break;
            }
            case 60:
            {
                onTimerEvent?.Invoke(1);
                break;
            }
            default:
            {
                break;
            }
        }
    }
}