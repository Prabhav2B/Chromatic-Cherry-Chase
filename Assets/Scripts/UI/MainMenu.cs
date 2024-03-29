using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private Image transitionImage;
    [SerializeField] private float fadeInDuration = 0.5f;

    private PlayerManager _playerManager;
    
    private bool _menuActive;

    public bool Paused => _menuActive;

    private void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
    }

    public void TriggerMainMenu()
    {
        
        
        if (!_menuActive)
        {
            _menuActive = !_menuActive;

            DOTween.Kill(transitionImage);
            AudioListener.pause = true;
            var tScale = Time.timeScale;
            DOTween.To(() => tScale, x => Time.timeScale = x, 0f, 0.5f).SetUpdate((true));
            transitionImage.DOFade(.95f, fadeInDuration).SetUpdate(true).OnComplete(ActivateMainMenu);
            _playerManager.Deactivate();

        }
        else
        {
            _menuActive = !_menuActive;
            
            DOTween.Kill(transitionImage);
            AudioListener.pause = false;
            var tScale = Time.timeScale;
            DOTween.To(() => tScale, x => Time.timeScale = x, 1f, 0.5f).SetUpdate(true);
            DeactivateMainMenu();
            transitionImage.DOFade(0f, fadeInDuration).SetUpdate(true);
            _playerManager.Activate();
        }
        
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    private void ActivateMainMenu()
    {
        mainMenuUI.SetActive(true);
    }

    private void DeactivateMainMenu()
    {
        mainMenuUI.SetActive(false);
    }
}