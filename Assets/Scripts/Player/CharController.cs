using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using Cinemachine;

public class CharController : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 10f, maxGravitySpeed = 10f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 10f, maxAirAcceleration = 1f;
    [SerializeField, Range(0f, 90f)] private float maxGroundAngle = 25f;
    [SerializeField, Range(-50f, 50f)] private float maxGravityAcceleration = -9.8f;
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 5.0f;
    [SerializeField, Range(0f, 100f)] private float dashVelocity = 10f;
    [SerializeField, Range(0f, 1f)]private float discount = 0.95f;
    [SerializeField, Range(0, 5)] private int maxAirJumps = 1;

    [SerializeField] private bool fastFallActive = false;

    private Rigidbody2D rb;
    private Vector2 velocity;
    private Vector2 displacement;
    private Vector2 acceleration;
    private float moveVal;
    private Vector2 dashDir;
    private float desiredMovementVelocity;
    private float desiredGravityVelocity;
    private float jumpVelocity;
    private bool desiredJump;
    private bool desiredDash;
    private bool isDashing;
    private bool jumpReleased;
    private bool onGround;
    private bool fastFall;
    private int jumpPhase;
    private float minGroundDotProduct;
    private float naturalUpwardVelocity;
    private CinemachineImpulseSource _impulseSource;

    public bool JumpMaxed { get; private set; }

    public float MaxSpeed => maxSpeed;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        OnValidate();
    }

    private void Start()
    {
        _impulseSource = FindObjectOfType<CinemachineImpulseSource>();
        
        jumpReleased = true;
    }

    public void Move(Vector2 moveVector)
    {
        dashDir = moveVector;
        this.moveVal = Mathf.Round(moveVector.x);
    }

    public void JumpInitiate()
    {
        jumpReleased = false;
        Jump();
    }

    private void Jump()
    {
        desiredJump = true;
    }

    public void JumpEnd()
    {
        jumpReleased = true;
        fastFall = true;
    }

    public void DashInitiate()
    {
        desiredDash = true;
    }

    public void DashEnd()
    {
        //Nothing to be done at Dash End _yet_
        //throw new NotImplementedException();
    }

    private void Update()
    {
        desiredMovementVelocity = moveVal * maxSpeed;
        desiredGravityVelocity = -maxGravitySpeed;
    }

    private void FixedUpdate()
    {
        if (isDashing)
            return;


        UpdateState();

        if (desiredJump)
        {
            desiredJump = false;
            if (jumpPhase < maxAirJumps || onGround)
            {
                jumpPhase++;
                jumpVelocity = Mathf.Sqrt(-2f * maxGravityAcceleration * jumpHeight);

                if (velocity.y > 0f)
                {
                    jumpVelocity = Mathf.Max(jumpVelocity - velocity.y, 0f);
                }
                else if (velocity.y < -0.001f)
                {
                    jumpVelocity = jumpVelocity - velocity.y;
                }

                velocity.y += jumpVelocity + 1.5f;
            }

            if (jumpPhase >= maxAirJumps)
            {
                JumpMaxed = true;
            }
        }


        if (fastFall && fastFallActive)
        {
            if (velocity.y >= jumpVelocity / 3f)
                velocity.y += (-1f * jumpVelocity) / 4f;

            fastFall = false;
        }

        float acceleration = onGround ? maxAcceleration : maxAirAcceleration;

        var maxChangeSpeed = acceleration * Time.fixedDeltaTime;
        if (Mathf.Approximately(desiredMovementVelocity, 0f) && !onGround)
            maxChangeSpeed = maxAcceleration * Time.fixedDeltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredMovementVelocity, maxChangeSpeed);

        float maxGravityChange;

        if (!onGround && velocity.y < 0f)
        {
            maxGravityChange = -maxGravityAcceleration * Time.fixedDeltaTime * 1.5f;
        }
        else
        {
            maxGravityChange = -maxGravityAcceleration * Time.fixedDeltaTime;
        }

        velocity.y = Mathf.MoveTowards(velocity.y, desiredGravityVelocity, maxGravityChange);

        if (desiredDash)
        {
            
            //Might need to separate concerns 
            desiredDash = false;
            Vector2 dir = dashDir.normalized;


            if (!Mathf.Approximately(dashDir.sqrMagnitude, 0f))
            {
                velocity.x = dashVelocity * dir.x;
                velocity.y = dashVelocity * dir.y;
                StopAllCoroutines();
                StartCoroutine(DashWait(velocity));
                return;
            }
        }

        rb.velocity = velocity;

        onGround = false;
    }

    IEnumerator DashWait(Vector2 velocity)
    {
        //     
        //     DOVirtual.Float(14, 0, .8f, RigidbodyDrag);
        //
        //     dashParticle.Play();
        //     rb.gravityScale = 0;
        //     GetComponent<BetterJumping>().enabled = false;
        //     
        //
        isDashing = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(.05f);
        
        _impulseSource.GenerateImpulse();
        float timeElapsed = 0.0f;
        Vector2 dashVel = velocity;

        while (true)
        {
            if (timeElapsed >= 0.3f)
                break;
            yield return new WaitForFixedUpdate();
            timeElapsed += Time.fixedDeltaTime;
            rb.velocity = dashVel * discount;
            dashVel = rb.velocity;
        }

        isDashing = false;
    }

    private void UpdateState()
    {
        velocity = rb.velocity;

        if (!onGround) return;
        jumpPhase = 0;
        naturalUpwardVelocity = 0f;
        JumpMaxed = false;
    }

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
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
            onGround |= normal.y >= minGroundDotProduct;
        }
    }
}