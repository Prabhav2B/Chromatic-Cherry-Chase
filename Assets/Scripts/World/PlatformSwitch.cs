using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSwitch : MonoBehaviour
{
    private PlayerManager playerManager;
    private SpriteRenderer[] platformSprites;
    private Collider2D myCollider;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        platformSprites = this.GetComponentsInChildren<SpriteRenderer>();
        myCollider = this.GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (playerManager.PowerActive)
            ActivatePlatform(true);
        else
            ActivatePlatform(false);
    }

    private void ActivatePlatform(bool b0)
    {
        foreach (var sprite in platformSprites)
        {
            sprite.color = b0 ? new Color(0.32f, 0.77f, 0.98f, 0.8f) : new Color(0.98f, 0.32f, 0.32f, 0.8f);
        }

        myCollider.enabled = b0;
    }
}
