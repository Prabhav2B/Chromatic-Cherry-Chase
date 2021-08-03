using System;
using System.Collections;
using System.Collections.Generic;
using CharTween;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ControlSchemeDependentText : MonoBehaviour
{
    [SerializeField] private ControlSchemeTextList _text; 
    TMP_Text _UItext;
    private TutorialUIUpdate _updater;
    private void Awake()
    {
        _UItext = this.GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        _updater = GetComponentInParent<TutorialUIUpdate>(); 
        _updater.onControlSchemeChange += UpdateText;
    }
    
    private void OnDisable()
    {
        _updater.onControlSchemeChange -= UpdateText;
    }

    private void UpdateText( PlayerManager.ControlScheme controlScheme)
    {
        _UItext.text = controlScheme == PlayerManager.ControlScheme.KeyboardAndMouse
            ? _text.controlScheme1Text
            : _text.controlScheme2Text;
    }

    
}
