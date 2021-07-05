using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBend : MonoBehaviour
{
    [SerializeField, Range(1f, 10f)] private float timeSlowScale = 1f;

    private float slowScaleInverse;
    private const float OriginalScale = 1.0f;

    void Start()
    {
        slowScaleInverse = 1.0f / timeSlowScale;
    }

    public void TimeBendInitiate()
    {
        Time.timeScale = slowScaleInverse;
    }

    public void TimeBendEnd()
    {
        Time.timeScale = OriginalScale;
    }
}