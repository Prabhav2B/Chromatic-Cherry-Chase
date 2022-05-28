using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoosicManager : MonoBehaviour
{
    [SerializeField] private AudioClip introClip;
    [SerializeField] private AudioClip loopClip;

    private AudioSource _audioSource;
    private GameTimer _gameTimer;
    private LevelResetHandler _levelResetHandler;

    private bool _moosicStopped;

    private void Awake()
    {
        _gameTimer = FindObjectOfType<GameTimer>();
        _levelResetHandler = FindObjectOfType<LevelResetHandler>();
    }

    private void OnEnable()
    {
        _levelResetHandler.onLevelReload += StopMoosic;
        _gameTimer.onTimerExpired += StopMoosic;
    }

    private void OnDisable()
    {
        if (_gameTimer != null)
            _gameTimer.onTimerExpired -= StopMoosic;

        if (_levelResetHandler != null)
            _levelResetHandler.onLevelReload -= StopMoosic;
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = introClip;
        _audioSource.playOnAwake = false;
    }

    public void PlayMoosic()
    {
        if(_audioSource.isPlaying) return;
        
        _audioSource.clip = introClip;
        _audioSource.Play();
        StartCoroutine(WaitForSongEnd());
    }
    
    IEnumerator WaitForSongEnd()
    {
        yield return new WaitUntil(() => !_audioSource.isPlaying);
        TransitionToLoop();
    }

    private void TransitionToLoop()
    {
        if(_moosicStopped) return;
        
        _audioSource.clip = loopClip;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    private void StopMoosic()
    {
        _moosicStopped = true;
        StopCoroutine(WaitForSongEnd());
        _audioSource.Stop();
    }
}
