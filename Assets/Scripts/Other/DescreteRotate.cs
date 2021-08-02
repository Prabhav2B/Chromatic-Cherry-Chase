using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Rotates whatever transform it is attached to by Specified Degrees,
/// in Specified Time, in Specified Intervals
/// </summary>
public class DescreteRotate : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Rotator());
    }

    private IEnumerator Rotator()
    {
        var accumulator = 0f;
        while (true)
        {
            transform.DORotate(new Vector3(0, 0, accumulator + 45f), .2f);
            accumulator += 45f;
            yield return new WaitForSeconds(1f);
        }
    }
}