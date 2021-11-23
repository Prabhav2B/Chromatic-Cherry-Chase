using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashDirectionAccessibility : MonoBehaviour
{
    private SpriteRenderer _indicatorSprite;
    private PlayerManager _playerManager;
    private CharController _characterController;
    private float zRotation;

    private void Awake()
    {
        _playerManager = GetComponentInParent<PlayerManager>();
        _indicatorSprite = GetComponentInChildren<SpriteRenderer>();
        _characterController = GetComponentInParent<CharController>();
    }

    private void Update()
    {
        Color col;
        if (!_playerManager.DashInputHeld)
        {
            col = _indicatorSprite.color;
            col.a = 0f;
            _indicatorSprite.color = col;
            return;
        }

        col = _indicatorSprite.color;
        col.a = 1f;
        _indicatorSprite.color = col;

        //float angle = Vector2.SignedAngle(new Vector2(_playerManager.ReceivedInput.x, -_playerManager.ReceivedInput.y), Vector2.left);      
        //Vector3 cross = Vector3.Cross(_playerManager.ReceivedInput, Vector3.right).normalized;
        

        int directionAsSector = _characterController.ComputeDashSector();


        float pointerDir = directionAsSector switch
        {
            0 => 0f,
            1 => 45f,
            2 => 90f,
            3 => 135f,
            4 => 180f,
            5 => 225f,
            6 => 270f,
            7 => 315f,
            _ => 0
        };

        //var targetRotation = Quaternion.Euler(new Vector3(0f, 0f, pointerDir + 180f));
        //MIGHT NOT BE THE BEST WAY TO FO IT
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 800f * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, pointerDir + 180);
    }
}