using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PlatformSwitch : MonoBehaviour
{
    [SerializeField] private Sprite blockPlatformSprite;
    [SerializeField] private Sprite capsulePlatformSprite;
    
    [SerializeField] private PlatformType myType;
    [SerializeField] private bool flips = false;
    [SerializeField] private float flipInterval = 0.0f;
    [SerializeField] private float flipIntervalOffset = 0.0f;

    private PlayerManager _playerManager;
    private SpriteRenderer[] _platformSprites;
    private Collider2D _myCollider;
    private Image _timerUI;

    private float _timeAccumulator = 0f;

    private void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _platformSprites = this.GetComponentsInChildren<SpriteRenderer>();
        _myCollider = this.GetComponent<Collider2D>();
        _timerUI = GetComponentInChildren<Image>();

        
    }

    private void Start()
    {
        _timeAccumulator = flipIntervalOffset;
        SetProperties(myType);

        if (!flips)
        {
            _timerUI.enabled = false;
            return;
        }
        _timerUI.enabled = true;
        StartCoroutine(FlipTimer());
    }

    private void Update()
    {
        if (!(_playerManager.PowerActive ^ myType == PlatformType.Red))
            ActivatePlatform(true);
        else
            ActivatePlatform(false);
    }

    private void SetProperties(PlatformType type)
    {
        foreach (var sprite in _platformSprites)
        {
            sprite.color = type == PlatformType.Blue
                ? new Color(0.29f, 0.55f, 0.98f, 0.8f)
                : new Color(0.98f, 0.76f, 0.28f, 0.8f);
            
            sprite.sprite = type == PlatformType.Blue
                ? capsulePlatformSprite
                : blockPlatformSprite;
        }

        if (flips)
        {
            _timerUI.color = type == PlatformType.Blue
                ? new Color(0.29f, 0.55f, 0.98f, 0.8f)
                : new Color(0.98f, 0.76f, 0.28f, 0.8f);
        }
    }

    private void ActivatePlatform(bool b0)
    {
        _myCollider.enabled = b0;
    }

    private void FlipPlatformerType()
    {
        myType = myType == PlatformType.Blue ? PlatformType.Red : PlatformType.Blue;
        SetProperties(myType);
        SquishStart();
    }

    private IEnumerator FlipTimer()
    {
        while (true)
        {
            if (_timeAccumulator > flipInterval)
            {
                _timeAccumulator = 0f;
                FlipPlatformerType();
            }
            
            _timerUI.fillAmount = Mathf.Clamp01((flipInterval - _timeAccumulator) / flipInterval);
            
            _timeAccumulator += Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }
    }
    
    void SquishStart()
    {
        transform.DOScale(new Vector3(.75f, 1.25f, 1.0f), .15f)
            .OnComplete(SquishRelease);
    }

    void SquishRelease()
    {
        transform.DOScale(new Vector3(1f, 1f, 1.0f), .15f);
    }

    private enum PlatformType
    {
        Red,
        Blue
    }
}