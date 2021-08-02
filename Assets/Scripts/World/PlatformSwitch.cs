using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSwitch : MonoBehaviour
{
    [SerializeField] private PlatformType myType;

    private PlayerManager _playerManager;
    private SpriteRenderer[] _platformSprites;
    private Collider2D _myCollider;

    private void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _platformSprites = this.GetComponentsInChildren<SpriteRenderer>();
        _myCollider = this.GetComponent<Collider2D>();

        SetProperties(myType);
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
            sprite.color = type == PlatformType.Blue ? new Color(0.32f, 0.77f, 0.98f, 0.8f) : new Color(0.98f, 0.32f, 0.32f, 0.8f);
        }
    }

    private void ActivatePlatform(bool b0)
    {
        _myCollider.enabled = b0;
    }

    private enum PlatformType
    {
        Red,
        Blue
    }
}