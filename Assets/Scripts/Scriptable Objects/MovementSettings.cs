using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementPreset", menuName = "Setting/Movement")]
public class MovementSettings : ScriptableObject
{
    [Range(.1f, 10f)]
    public float movementSpeed = 10;

    [Range(.1f, 10f)]
    public float dragFactor = 1;

    [Range(0.0f, 5.0f)]
    public float timeToReachFullSpeed = .5f;

    [Range(0.0f, 5.0f)]
    public float timeToFullyStop = .5f;

    [Range(0.0f, 5.0f)]
    public float gravityScale = .5f;
}
