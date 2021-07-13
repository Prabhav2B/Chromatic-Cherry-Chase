using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyser : MonoBehaviour
{
    [SerializeField] private Vector2 geyserVelocity = new Vector2(0f, 2f);
    private PlayerManager playerManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        playerManager = other.GetComponent<PlayerManager>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (playerManager != null)
        {
            playerManager.SetExternalVelocity(playerManager.PowerActive
                ? Vector2.zero
                : geyserVelocity * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (playerManager != null)
        {
            playerManager.SetExternalVelocity(Vector2.zero);
            playerManager.GyserExit();
        }

        playerManager = null;
    }
}