using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;

public class CharController : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 10f, maxAirAcceleration = 1f;
    [SerializeField, Range(-50f, 50f)] private float maxGravityAcceleration = -9.8f;
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 5.0f;
    [SerializeField, Range(0, 5)] int maxAirJumps = 1;

    private Rigidbody2D rb;
    private Vector2 velocity;
    private Vector2 displacement;
    private Vector2 acceleration;
    private Vector2 moveVector;
    private float desiredMovementVelocity;
    private float desiredGravityVelocity;
    private bool desiredJump;
    private bool onGround;
    private int jumpPhase;

    public float MaxSpeed => maxSpeed;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 moveVector)
    {
        this.moveVector = moveVector;
    }

    public void Jump()
    {
        desiredJump = true;
    }

    private void Update()
    {
        desiredMovementVelocity =  moveVector.x * maxSpeed;
        desiredGravityVelocity =  maxSpeed;
    }

    private void FixedUpdate()
    {
        UpdateState();

        if (desiredJump)
        {
            desiredJump = false;
            if (onGround || jumpPhase < maxAirJumps)
            {
                jumpPhase++;
                float jumpVelocity = Mathf.Sqrt(-2f * maxGravityAcceleration * jumpHeight);

                if (velocity.y > 0f)
                {
                    jumpVelocity = Mathf.Max(jumpVelocity - velocity.y, 0f);
                }
                else if (velocity.y < -0.1f)
                {
                    jumpVelocity = Mathf.Max(jumpVelocity - velocity.y, 0f);
                }

                velocity.y += jumpVelocity;
            }
        }

        float acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        float maxChangeSpeed = acceleration * Time.fixedDeltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredMovementVelocity, maxChangeSpeed);

        float maxGravityChange;
        
        if (!onGround && velocity.y < 0f)
        {
            maxGravityChange = maxGravityAcceleration * Time.deltaTime * 2.0f;
        }
        else
        {
            maxGravityChange = maxGravityAcceleration * Time.deltaTime;
        }
        velocity.y = Mathf.MoveTowards(velocity.y, desiredGravityVelocity, maxGravityChange);
        

        rb.velocity = velocity;

        onGround = false;
    }

    private void UpdateState()
    {
        velocity = rb.velocity;

        if (onGround)
        {
            jumpPhase = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        EvaluateCollision(other);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        EvaluateCollision(other);
    }

    private void EvaluateCollision(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.contacts[i].normal;
            onGround |= normal.y >= 0.9f;
        }
    }
}