using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Serialization;

public class ControlSchemeDependentImage : MonoBehaviour
{
        [SerializeField] private ControlSchemeImageList image; 
        private SVGImage _uiSvg;
        private TutorialUIUpdate _updater;
    
        private void Awake()
        {
            _uiSvg = this.GetComponentInChildren<SVGImage>();
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
            _uiSvg.sprite = controlScheme == PlayerManager.ControlScheme.KeyboardAndMouse
                ? image.controlScheme1Image
                : image.controlScheme2Image;
        }
}
