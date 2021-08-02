using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSwitch : MonoBehaviour
{
    [SerializeField] private PlatformType myType;

    private PlayerManager playerManager;
    private SpriteRenderer[] platformSprites;
    private Collider2D myCollider;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        platformSprites = this.GetComponentsInChildren<SpriteRenderer>();
        myCollider = this.GetComponent<Collider2D>();

        SetProperties(myType);
    }

    
    private void Update()
    {
        if (!(playerManager.PowerActive ^ myType == PlatformType.red))
            ActivatePlatform(true);
        else
            ActivatePlatform(false);
    }

    private void SetProperties(PlatformType type)
    {
        foreach (var sprite in platformSprites)
        {
            sprite.color = type == PlatformType.blue ? new Color(0.32f, 0.77f, 0.98f, 0.8f) : new Color(0.98f, 0.32f, 0.32f, 0.8f);
        }
    }

    private void ActivatePlatform(bool b0)
    {
        myCollider.enabled = b0;
    }

    public enum PlatformType
    {
        red,
        blue
    }
}