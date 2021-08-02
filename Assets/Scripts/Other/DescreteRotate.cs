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
    [Tooltip("How much should one rotation be in degrees")]
    [SerializeField, Range(0f, 360f)] private float rotationDegrees = 45f;
    [Tooltip("How long should it take to rotate?")]
    [SerializeField] private float duration = 0.2f;
    [Tooltip("How long between rotations?")]
    [SerializeField] private float intervalDuration = 1f;
    private void Start()
    {
        StartCoroutine(Rotator());
    }

    private IEnumerator Rotator()
    {
        var accumulator = 0f;
        while (true)
        {
            transform.DORotate(new Vector3(0, 0, accumulator + rotationDegrees), duration);
            accumulator += rotationDegrees;
            yield return new WaitForSeconds(intervalDuration);
        }
    }
}