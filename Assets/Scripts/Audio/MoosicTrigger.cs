using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoosicTrigger : MonoBehaviour
{
    private MoosicManager _moosicManager;
    
    void Start()
    {
        _moosicManager = FindObjectOfType<MoosicManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _moosicManager.PlayMoosic();
    }
}
