using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using Cinemachine;
using UnityEngine.Serialization;

public class CharController : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 10f, maxGravitySpeed = 10f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 10f, maxAirAcceleration = 1f;
    [SerializeField, Range(0f, 90f)] private float maxGroundAngle = 25f;
    [SerializeField, Range(-50f, 50f)] private float maxGravityAcceleration = -9.8f;
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 5.0f;
    [SerializeField, Range(0f, 100f)] private float dashVelocity = 10f;
    [FormerlySerializedAs("discount")] [SerializeField, Range(0f, 1f)]private float dashVelocityDiscount = 0.95f;
    [SerializeField, Range(0, 5)] private int maxAirJumps = 1;
    [FormerlySerializedAs("maxAirDash")] [SerializeField, Range(0, 5)] private int maxDash = 1;


    [SerializeField] private bool fastFallActive = false;

    private Rigidbody2D rb;
    private Vector2 velocity;
    private Vector2 displacement;
    private Vector2 acceleration;
    private Vector2 dashDir;
    private Vector2 contactNormal;
    private float moveVal;
    private float desiredMovementVelocity;
    private float desiredGravityVelocity;
    private float jumpVelocity;
    private bool desiredJump;
    private bool desiredDash;
    private bool isDashing;
    //private bool jumpReleased; -> currently unused;
    private bool onGround;
    private bool fastFall;
    private int jumpPhase;
    private int dashPhase;
    private int stepsSinceLastJump, stepsSinceLastDash;
    private float minGroundDotProduct;
    private CinemachineImpulseSource _impulseSource;

    public bool JumpMaxed => (jumpPhase > maxAirJumps || (!onGround && maxAirJumps == 0));

    public float MaxSpeed => maxSpeed;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        OnValidate();
    }

    private void Start()
    {
        _impulseSource = FindObjectOfType<CinemachineImpulseSource>();
        
        //jumpReleased = true;
    }

    public void Move(Vector2 moveVector)
    {
        dashDir = moveVector;
        this.moveVal = Mathf.Round(moveVector.x);
    }

    public void JumpInitiate()
    {
        //jumpReleased = false;
        Jump();
    }

    private void Jump()
    {
        desiredJump = true;
    }

    public void JumpEnd()
    {
        //jumpReleased = true;
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
            
            if ((maxAirJumps > 0 && jumpPhase <= maxAirJumps))
            {
                if (jumpPhase == 0 && !onGround) {
                    jumpPhase = 1;
                }
                stepsSinceLastJump = 0;
                jumpPhase++;
                jumpVelocity = Mathf.Sqrt(-2f * maxGravityAcceleration * jumpHeight);
                //float alignedVelocity = Vector2.Dot(velocity.normalized, contactNormal); // not completely sure about this

                
                if (!Mathf.Approximately(contactNormal.y, 1f) && velocity.y > 0) //check if on a upward slope
                {
                    velocity.y += ((jumpVelocity)) ;
                }
                else
                {
                    velocity.y = (jumpVelocity+ 1.5f) ;
                }

                
            }
        }


        if (fastFall && fastFallActive)
        {
            if (velocity.y >= jumpVelocity / 3f)
                velocity.y += (-1f * jumpVelocity) / 4f;

            fastFall = false;
        }

        AdjustVelocity();

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

        if (dashPhase < maxDash && desiredDash)
        {
            
            //Might need to separate concerns 
            desiredDash = false;
            Vector2 dir = dashDir.normalized;


            if (!Mathf.Approximately(dashDir.sqrMagnitude, 0f))
            {
                stepsSinceLastDash = 0;
                dashPhase++;
                velocity.x = dashVelocity * dir.x;
                velocity.y = dashVelocity * dir.y;
                StopAllCoroutines();
                StartCoroutine(DashWait(velocity));
                return;
            }
        }

        rb.velocity = velocity;

        ClearState();
    }
    private void UpdateState()
    {
        velocity = rb.velocity;
        stepsSinceLastJump++;
        stepsSinceLastDash++;
        
        if (!onGround)
        {
            contactNormal = Vector2.up;
            return;
        }

        contactNormal.Normalize();
        if (stepsSinceLastJump > 1) {
            jumpPhase = 0;
            
        }

        if (stepsSinceLastDash > 1)
        {
            dashPhase = 0;
        }

        
    }
    void ClearState () 
    {
        onGround = false;
        contactNormal = Vector2.zero;
    }
    
    private IEnumerator DashWait(Vector2 velocity)
    {
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
            rb.velocity = dashVel * dashVelocityDiscount;
            dashVel = rb.velocity;
        }

        isDashing = false;
    }

    private void OnValidate()
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


            if (!(normal.y >= minGroundDotProduct)) continue;
            onGround = true;
            contactNormal += normal;
        }
    }
    
    Vector2 ProjectOnContactPlane (Vector2 vector) {
        return vector - contactNormal * Vector2.Dot(vector, contactNormal);
    }

    void AdjustVelocity()
    {
        Vector2 xAxis = ProjectOnContactPlane(Vector2.right).normalized;

        float currentX = Vector2.Dot(velocity, xAxis);


        float acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;

        if (Mathf.Approximately(desiredMovementVelocity, 0f) &&
            !onGround) //come to stop fast when control is released mid air
            maxSpeedChange = maxAcceleration * Time.fixedDeltaTime;

        float newX = Mathf.MoveTowards(currentX, desiredMovementVelocity, maxSpeedChange);

        velocity += xAxis * (newX - currentX);

        float slopeFactor = Mathf.Abs(Vector2.Angle(contactNormal, Vector2.up))/90f;
        
        
        if (velocity.y < -0.1 && onGround) // little hack to adjust speed on slopes
        {
            velocity += velocity.normalized * (slopeFactor * 2.4f);
        }
        else if (velocity.y > 0.1 && onGround)
        {
            velocity -= velocity.normalized * (slopeFactor * 1.6f);
        }

    }
}