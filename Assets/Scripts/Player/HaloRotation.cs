using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class HaloRotation : MonoBehaviour
{
    private Transform halo;
    private Camera camera;

    private void Awake()
    {
        // camera = Camera.main;
         halo = this.GetComponent<Transform>();
    }

    private void Start()
    {
        SquishStart();
    }

    private void Update()
    {
        // Vector3 right = Vector3.Cross(camera.transform.up, transform.position - camera.transform.position);
        // Vector3 up = Vector3.Cross(transform.position - camera.transform.position, right);
        //
        // transform.rotation = Quaternion.AngleAxis(-0.5f, up) * transform.rotation;
        // transform.rotation = Quaternion.AngleAxis(1f, right) * transform.rotation;

    }
    
    void SquishStart()
    {
        halo.DOScale(new Vector3(.75f, .75f, .75f), .55f)
            .OnComplete(SquishRelease);
    }

    void SquishRelease()
    {
        halo.DOScale(new Vector3(1.25f, 1.25f, 1.25f), .55f).OnComplete(SquishStart);;
    }
}
