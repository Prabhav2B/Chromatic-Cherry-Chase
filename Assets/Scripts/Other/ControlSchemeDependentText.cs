using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlSchemeDependentText : MonoBehaviour
{
    [SerializeField] private ControlSchemeTextList _text; 
    private TMP_Text _UItext;

    private void Awake()
    {
        _UItext = this.GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        this.GetComponentInParent<TutorialUIUpdate>().onControlSchemeChange += UpdateText;
    }
    
    private void OnDisable()
    {
        this.GetComponentInParent<TutorialUIUpdate>().onControlSchemeChange -= UpdateText;
    }

    private void UpdateText( PlayerManager.ControlScheme controlScheme)
    {
        _UItext.text = controlScheme == PlayerManager.ControlScheme.KeyboardAndMouse
            ? _text.controlScheme1Text
            : _text.controlScheme2Text;
    }
}
