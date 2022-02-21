using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReminderTextPopUp : MonoBehaviour
{
    private Vector3 _originalScale;
    private TMP_Text _text;
    private Image _panelImage;

    private GameTimer _gameTimer;


    private void Awake()
    {
        _gameTimer = FindObjectOfType<GameTimer>();
    }

    private void Start()
    {
        _originalScale = transform.localScale;
        _text = GetComponentInChildren<TMP_Text>();
        _panelImage = GetComponentInChildren<Image>();

        var tempColor = _panelImage.color;
        tempColor.a = 0f;
        _panelImage.color = tempColor;
        
        tempColor = _text.color;
        tempColor.a = 0f;
        _text.color = tempColor;
    }

    private void OnEnable()
    {
        _gameTimer.onTimerEvent += PopUp;
    }

    private void OnDisable()
    {
        _gameTimer.onTimerEvent -= PopUp;
    }

    private void PopUp(int time)
    {
        _text.text = time + " Minutes Left!";
        PopStart();
    }
    
    void PopStart()
    {
        _panelImage.DOFade(1f, .5f);
        _text.DOFade(.9f, .5f);
        transform.DOScale(new Vector3(1.05f, 1.05f, 1.05f), .5f).OnComplete(PopWait);
    }

    void PopWait()
    {
        Invoke(nameof(PopEnd), 1.25f);
    }

    void PopEnd()
    {
        transform.DOScale(new Vector3(1.00f, 1.00f, 1.00f), .5f);
        _panelImage.DOFade(0, .5f);
        _text.DOFade(0, .5f);
    }
}
