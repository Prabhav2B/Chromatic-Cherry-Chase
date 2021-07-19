using System;
using System.Collections;
using UnityEngine;
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
    [SerializeField, Range(0f, 1f)] private float dashVelocityDiscount = 0.95f;
    [SerializeField, Range(0, 5)] private int maxAirJumps = 1;
    [SerializeField, Range(0, 5)] private int maxDash = 1;
    [SerializeField] private bool fastFallActive;

    private Rigidbody2D _rb;
    private Vector2 _velocity, _externalVelocity;
    private Vector2 _dashDir;
    private Vector2 _contactNormal, _steepNormal;
    private float _moveVal;
    private float _desiredMovementVelocity, _desiredGravityVelocity;
    private float _jumpVelocity;
    private bool _desiredJump, _desiredDash;
    private bool _jumpBuffer;
    private bool _isDashing;
    private bool _onGround, _onSteep;
    private bool _steepProximity;
    private bool _fastFall;
    private bool _facingRight, _isStill;
    private int _jumpPhase, _dashPhase;
    private int _stepsSinceLastJump, _stepsSinceLastDash, _stepsSinceJumpBuffer;
    private float _minGroundDotProduct;
    private CinemachineImpulseSource _impulseSource;

    public bool JumpMaxed => (_jumpPhase > maxAirJumps || (!_onGround && maxAirJumps == 0));
    public bool DashMaxed => _dashPhase >= maxDash;
    public bool IsStill => _isStill;
    public bool FacingRight => _facingRight;

    public Vector2 ExternalVelocity
    {
        set => _externalVelocity = value;
    }

    private void Awake()
    {
        _rb = this.GetComponent<Rigidbody2D>();
        OnValidate();
    }

    private void Start()
    {
        _impulseSource = FindObjectOfType<CinemachineImpulseSource>();
        _facingRight = true;
    }

    public void Move(Vector2 moveVector)
    {
        _dashDir = moveVector;
        this._moveVal = Mathf.Round(moveVector.x); // Lock this while wall jumping
    }

    public void JumpInitiate()
    {
        _desiredJump = true;
    }

    public void JumpEnd()
    {
        _fastFall = true;
    }

    public void DashInitiate()
    {
        _desiredDash = true;
    }

    public void DashEnd()
    {
        //Nothing to be done at Dash End _yet_
        //throw new NotImplementedException();
    }

    private void Update()
    {
        _isStill = false;
        _desiredMovementVelocity = _moveVal * maxSpeed;
        _desiredGravityVelocity = -maxGravitySpeed;

        if (_velocity.x > maxSpeed * 0.75f)
        {
            _facingRight = true;
        }
        else if (_rb.velocity.x < maxSpeed * -0.75f)
        {
            _facingRight = false;
        }
        else
        {
            _isStill = true;
        }
    }

    private void FixedUpdate()
    {
        if (_isDashing)
            return;

        UpdateState();

        if (_dashPhase < maxDash && _desiredDash)
        {
            //Might need to separate concerns 
            _desiredDash = false;

            _stepsSinceLastDash = 0;
            _dashPhase++;
            StopAllCoroutines();
            StartCoroutine(Dash());
            return;
        }

        if (_desiredJump || _jumpBuffer)
        {
            _desiredJump = false;

            if (_jumpPhase > maxAirJumps && !_onSteep)
            {
                if (!_jumpBuffer)
                {
                    _jumpBuffer = true;
                    _stepsSinceJumpBuffer = 0;
                }
            }
            else
            {
                ResetJumpBuffer();
                Jump();
            }

        }

        if (_fastFall && fastFallActive)
        {
            if (_velocity.y >= _jumpVelocity / 3f)
                _velocity.y += (-1f * _jumpVelocity) / 4f;

            _fastFall = false;
        }


        AdjustVelocity();

        float maxGravityChange;

        if (!_onGround && _velocity.y < 0f)
        {
            maxGravityChange = -maxGravityAcceleration * Time.fixedDeltaTime * 1.5f;
        }
        else
        {
            maxGravityChange = -maxGravityAcceleration * Time.fixedDeltaTime;
        }

        if (_onSteep)
        {
            _desiredGravityVelocity /= 2f;
        }

        _velocity.y = Mathf.MoveTowards(_velocity.y, _desiredGravityVelocity, maxGravityChange);

        _velocity = _velocity + _externalVelocity;
        _velocity.x = Mathf.Min(_velocity.x, 30f);
        _velocity.y = Mathf.Min(_velocity.y, 30f);

        _rb.velocity = _velocity;

        ClearState();
    }

    private void UpdateState()
    {
        _velocity = _rb.velocity;
        _stepsSinceLastJump++;
        _stepsSinceLastDash++;
        _stepsSinceJumpBuffer++;

        if (!_onGround)
        {
            _contactNormal = Vector2.up;
            return;
        }

        _contactNormal.Normalize();
        _steepNormal.Normalize();

        if (_stepsSinceLastJump > 1)
        {
            _jumpPhase = 0;
        }

        if (_stepsSinceLastDash > 1)
        {
            _dashPhase = 0;
        }

        if (_stepsSinceJumpBuffer > 10)
        {
            ResetJumpBuffer();
        }
    }

    private void ResetJumpBuffer()
    {
        _stepsSinceJumpBuffer = 0;
        _jumpBuffer = false;
    }

    private void Jump()
    {
        _stepsSinceLastJump = 0;
        _steepProximity = !_onSteep && CheckForSteepProximity(); //remember to set steep normal too

        if (_onSteep || _steepProximity) //Wall Jump
        {
            WallJump();
            return;
        }

        if (_jumpPhase == 0 && !_onGround)
        {
            _jumpPhase = 1;
        }

        _jumpPhase++;
        _jumpVelocity = Mathf.Sqrt(-2f * maxGravityAcceleration * jumpHeight);
        //float alignedVelocity = Vector2.Dot(velocity.normalized, contactNormal); // not completely sure about this

        if (!Mathf.Approximately(_contactNormal.y, 1f) && _velocity.y > 0) //check if on a upward slope
        {
            _velocity.y += ((_jumpVelocity));
        }
        else
        {
            _velocity.y = (_jumpVelocity + 1.5f);
        }
    }

    private void WallJump()
    {
        _velocity.x = (_steepNormal.normalized).x * 16.5f; //remove .normalized for reeeeeee
        _velocity.y = 8.5f;
        if (!Mathf.Approximately(_moveVal, 0f))
        {
            _rb.velocity = _velocity;
        }
    }

    private bool CheckForSteepProximity()
    {
        RaycastHit2D hitinfo = Physics2D.Raycast(this.transform.position, Vector2.right, .55f);
        if (hitinfo.collider != null)
        {
            _steepNormal = hitinfo.normal;
            return true;
        }

        hitinfo = Physics2D.Raycast(this.transform.position, Vector2.left, .55f);
        if (hitinfo.collider != null)
        {
            _steepNormal = hitinfo.normal;
            return true;
        }

        return false;
    }

    void ClearState()
    {
        _onGround = _onSteep = _desiredDash = false;
        _contactNormal = _steepNormal = Vector2.zero;
    }

    public void ClampVelocity()
    {
        _velocity = _rb.velocity;
        _velocity.x = Mathf.Min(_velocity.x, 10f);
        _velocity.y = Mathf.Min(_velocity.y, 10f);

        _rb.velocity = _velocity;
    }

    private IEnumerator Dash()
    {
        _isDashing = true;

        if (Mathf.Approximately(_dashDir.sqrMagnitude, 0f)) //needs to be recorded before setting velocity to zero
        {
            _dashDir = _facingRight ? Vector2.right : Vector2.left;
        }

        _dashDir.Normalize();

        _rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(.05f);

        _impulseSource.GenerateImpulse();
        float timeElapsed = 0.0f;

        Vector2 dashVel;
        dashVel.x = dashVelocity * _dashDir.x;
        dashVel.y = dashVelocity * _dashDir.y;

        while (true)
        {
            if (timeElapsed >= 0.3f)
                break;
            yield return new WaitForFixedUpdate();
            timeElapsed += Time.fixedDeltaTime;
            _rb.velocity = dashVel * dashVelocityDiscount;
            dashVel = _rb.velocity;
        }

        _isDashing = false;
    }

    private void OnValidate()
    {
        _minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
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


            if ((normal.y >= _minGroundDotProduct))
            {
                _onGround = true;
                _contactNormal += normal;
            }
            else if (normal.y > -0.01f)
            {
                _onSteep = true;
                _steepNormal += normal;
            }
        }

        if (_onGround && _onSteep) // a hack
        {
            _onSteep = false;
        }
    }

    Vector2 ProjectOnContactPlane(Vector2 vector)
    {
        //return vector - contactNormal * Vector2.Dot(vector, contactNormal);
        return vector;
    }

    void AdjustVelocity()
    {
        Vector2 xAxis = ProjectOnContactPlane(Vector2.right).normalized;

        float currentX = Vector2.Dot(_velocity, xAxis);


        float acceleration = _onGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;

        if (Mathf.Approximately(_desiredMovementVelocity, 0f) &&
            !_onGround) //come to stop fast when control is released mid air
            maxSpeedChange = maxAcceleration * Time.fixedDeltaTime;

        float newX = Mathf.MoveTowards(currentX, _desiredMovementVelocity, maxSpeedChange);

        _velocity += xAxis * (newX - currentX);

        float slopeFactor = Mathf.Abs(Vector2.Angle(_contactNormal, Vector2.up)) / 90f;


        if (_velocity.y < -0.1 && _onGround) // little hack to adjust speed on slopes
        {
            _velocity += _velocity.normalized * (slopeFactor * 1.5f);
        }
        else if (_velocity.y > 0.1 && _onGround)
        {
            _velocity -= _velocity.normalized * slopeFactor;
        }
    }
}