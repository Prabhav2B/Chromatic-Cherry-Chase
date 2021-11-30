using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashTimerCanvas : MonoBehaviour
{
    private Image _dashTimerImage;
    private float _dashFillAmount;
    
    
    public float DashFillAmount {
        set => _dashFillAmount = Mathf.Clamp01(value);
    }

    // Start is called before the first frame update
    void Start()
    {
        _dashTimerImage = GetComponentInChildren<Image>();
        Deactivate();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_dashTimerImage.enabled)
            return;
            
        _dashTimerImage.fillAmount = _dashFillAmount;
    }

    public void Activate()
    {
        _dashTimerImage.enabled = true;
    }
    
    public void Deactivate()
    {
        _dashTimerImage.enabled = false;
    }
}
