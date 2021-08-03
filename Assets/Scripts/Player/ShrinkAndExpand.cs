using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Looped shrinking tweener animation 
/// </summary>
public class ShrinkAndExpand : MonoBehaviour
{
    private Transform _halo;

    private void Awake()
    {
        _halo = this.GetComponent<Transform>();
    }

    private void Start()
    {
        SquishStart();
    }
    
void SquishStart()
    {
        _halo.DOScale(new Vector3(.75f, .75f, .75f), .55f)
            .OnComplete(SquishRelease);
    }

    void SquishRelease()
    {
        _halo.DOScale(new Vector3(1.25f, 1.25f, 1.25f), .55f).OnComplete(SquishStart);;
    }

    public void StopTween()
    {
        _halo.DOKill();
    }
}
