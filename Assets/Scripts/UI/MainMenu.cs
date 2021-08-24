using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] Image transitionImage;
    [SerializeField] private float fadeInDuration = 0.5f;
    private bool _menuActive;

    public void TriggerMainMenu()
    {
        _menuActive = !_menuActive;

        if (_menuActive)
        {
            var tScale = Time.timeScale;
            DOTween.To(() => tScale, x => Time.timeScale = x, 0f, 0.4f);
            transitionImage.DOFade(.8f, fadeInDuration).SetUpdate(true).OnComplete(ActiveMainMenu);
        }
        else
        {
            var tScale = Time.timeScale;
            DOTween.To(() => tScale, x => Time.timeScale = x, 1f, 0.4f);
            DeactiveMainMenu();
            transitionImage.DOFade(0f, fadeInDuration).SetUpdate(true);
        }
    }

    private void ActiveMainMenu()
    {
        mainMenuUI.SetActive(true);
    }

    private void DeactiveMainMenu()
    {
        mainMenuUI.SetActive(false);
    }
}