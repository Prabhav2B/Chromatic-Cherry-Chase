using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashDirectionAccessibility : MonoBehaviour
{
    private SpriteRenderer _indicatorSprite;
    private PlayerManager _playerManager;
    private float zRotation;

    private void Awake()
    {
        _playerManager = GetComponentInParent<PlayerManager>();
        _indicatorSprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        Color col;
        if (Mathf.Approximately(_playerManager.ReceivedInput.magnitude, 0f))
        {
            col = _indicatorSprite.color;
            col.a = 0f;
            _indicatorSprite.color = col;
            return;
        }
        
        col = _indicatorSprite.color;
        col.a = 1f;
        _indicatorSprite.color = col;

        float angle = Vector2.SignedAngle(new Vector2(_playerManager.ReceivedInput.x, -_playerManager.ReceivedInput.y), Vector2.left);      
        //Vector3 cross = Vector3.Cross(_playerManager.ReceivedInput, Vector3.right).normalized;
        var targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        
        //MIGHT NOT BE THE BEST WAY TO FO IT
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 400f * Time.deltaTime);
    }
}
