using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

public class ControlSchemeDependentImage : MonoBehaviour
{
    [SerializeField] private ControlSchemeImageList _text; 
        private SVGImage _UISVG;
        private TutorialUIUpdate _updater;
    
        private void Awake()
        {
            _UISVG = this.GetComponentInChildren<SVGImage>();
        }
    
        private void OnEnable()
        {
            _updater = GetComponentInParent<TutorialUIUpdate>(); 
            _updater.onControlSchemeChange += UpdateImage;
        }
        
        private void OnDisable()
        {
            _updater.onControlSchemeChange -= UpdateImage;
        }
    
        private void UpdateImage( PlayerManager.ControlScheme controlScheme)
        {
            _UISVG.sprite = controlScheme == PlayerManager.ControlScheme.KeyboardAndMouse
                ? _text.controlScheme1Image
                : _text.controlScheme2Image;
        }
}
