using System;
using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
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
    private bool _jumpBuffer, _coyoteJump;
    private bool _isDashing;
    private bool _onGround, _onSteep, _onDownwardSlope;
    private bool _steepProximity, _pushingSteep;
    private bool _fastFall;
    private bool _facingRight, _isStill;
    private int _jumpPhase, _dashPhase, _slopeDir, _jumpCount;
    private int _stepsSinceLastJump, _stepsSinceLastDash, _stepsSinceJumpBuffer, _stepsSinceCoyoteFlag;
    private float _minGroundDotProduct;
    private CinemachineImpulseSource _impulseSource;
    private Camera _mainCam;

    private const int JumpBufferFrames = 8;
    private const int CoyoteFlagFrames = 10;

    public bool JumpMaxed => (_jumpPhase > maxAirJumps || (!_onGround && maxAirJumps == 0));
    public bool DashMaxed => _dashPhase >= maxDash;
    public bool IsStill => _isStill;
    public bool FacingRight => _facingRight;
    public bool OnSteep => _onSteep;
    public bool OnGround => _onGround;
    public bool OnDownwardSlope => _onDownwardSlope;
    public bool IsDashing => _isDashing;


    public Vector2 DashDir
    {
        set => _dashDir = value;
    }

    public Vector2 ExternalVelocity
    {
        set => _externalVelocity = value;
    }

    public Action OnDash;
    public Action OnJump;
    public Action OnDoubleJump;

    private void Awake()
    {
        _rb = this.GetComponent<Rigidbody2D>();
        _mainCam = Camera.main;
        OnValidate();
    }

    private void Start()
    {
        _impulseSource = FindObjectOfType<CinemachineImpulseSource>();
        if (Camera.main is { })
        {
            var impulseListener = Camera.main.GetComponent<CinemachineIndependentImpulseListener>();
            if (impulseListener == null)
            {
                var independentImpulseListener =
                    Camera.main.gameObject.AddComponent<CinemachineIndependentImpulseListener>();
                independentImpulseListener.m_Gain = 0.4f;
                independentImpulseListener.m_ChannelMask = 1 << 0;
            }
        }

        _facingRight = true;
        _slopeDir = 0;
        _jumpCount = 0;
    }

    public void Move(Vector2 moveVector)
    {
        //_dashDir = moveVector;
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
        DashDir = ComputeDashDirection();
        _desiredDash = true;
    }

    private Vector2 ComputeDashDirection()
    {
        var mousePosition = Vector3.Scale(_mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue()), new Vector3(1f, 1f, 0f));
        var vectorFromCharacterToMouse = mousePosition - transform.position;

        var angleCounterClockwise = SignedAngleBetween(Vector3.left, vectorFromCharacterToMouse, Vector3.forward);
        var sector = (int) (angleCounterClockwise / 45f);
        var offset = angleCounterClockwise % 45f;

        sector = offset > (45 / 2f) ? (sector + 1) % 8 : sector;

        Vector2 pointerDir = Vector2.zero;
        
        switch (sector)         
        {
            case 0:
                pointerDir = Vector2.right;
                break;
            case 1:
                pointerDir = new Vector2(1, 1);
                break;
            case 2:
                pointerDir = Vector2.up;
                break;
            case 3:
                pointerDir = new Vector2(-1, 1);
                break;
            case 4:
                pointerDir = Vector2.left;
                break;
            case 5:
                pointerDir = new Vector2(-1, -1);
                break;
            case 6:
                pointerDir = Vector2.down;
                break;
            case 7:
                pointerDir = new Vector2(1, -1);
                break;
            default:
                pointerDir = Vector2.zero;
                break;
        }

        return pointerDir;
    }
    
    private float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n){
        // angle in [0,180]
        float angle = Vector3.Angle(a,b);
        float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));

        // angle in [-179,180]
        float signed_angle = angle * sign;

        // angle in [0,360] (not used but included here for completeness)
        float angle360 =  (signed_angle + 180) % 360;

        return angle360;
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
        else if(!_isDashing)
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
            
            _pushingSteep = _jumpCount < 1 && (int) _moveVal == _slopeDir;
           
            
            
            if (_jumpPhase > maxAirJumps && !_onSteep &&
                !CheckForSteepProximity()) //check for steep proximity here itself
            {
                if (!_jumpBuffer)
                {
                    _jumpBuffer = true;
                    _stepsSinceJumpBuffer = 0;
                }
            }
            else if(_jumpPhase <= maxAirJumps || (_onSteep && !_pushingSteep))
            {
                _jumpBuffer = false;
                Jump();
            }
            else
            {
                if (!_jumpBuffer)
                {
                    _jumpBuffer = true;
                    _stepsSinceJumpBuffer = 0;
                }
            }
        }

        if (_fastFall && fastFallActive && !_jumpBuffer)
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
        _stepsSinceCoyoteFlag++;


        if (_stepsSinceJumpBuffer > JumpBufferFrames && _jumpBuffer)
        {
            _jumpBuffer = false;
        }

        if (_stepsSinceCoyoteFlag > CoyoteFlagFrames)
        {
            _coyoteJump = false;
        }

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


        if (_velocity.y < -0.1 && _onGround && !_coyoteJump) // little hack to adjust speed on slopes
        {
            _velocity += _velocity.normalized * (slopeFactor * 1.5f);
            _onDownwardSlope = true;
        }
        else if (_velocity.y > 0.1 && _onGround && !_coyoteJump)
        {
            _velocity -= _velocity.normalized * slopeFactor;
            _onDownwardSlope = false;
        }
        else
        {
            _onDownwardSlope = false;
        }
    }

    private void Jump()
    {
        if (_coyoteJump)
        {
            _coyoteJump = false;
        }

        _stepsSinceLastJump = 0;
        _steepProximity = !_onSteep && CheckForSteepProximity(); //remember to set steep normal too

        if ((_onSteep || _steepProximity) && !_onGround && !_pushingSteep) //Wall Jump
        {
            _jumpCount++;
            OnJump?.Invoke();
            WallJump();
            return;
        }

        if (_jumpPhase == 0 && !_onGround)
        {
            _jumpPhase = 1;
        }

        _jumpPhase++;
        _jumpCount++;
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
        
        switch (_jumpPhase)
        {
            case 1:
                OnJump?.Invoke();
                break;
            case 2:
                OnDoubleJump?.Invoke();
                break;
        }


    }

    private void WallJump()
    {
        _velocity.x = (_steepNormal.normalized).x * 16.5f; //remove .normalized for reeeeeee
        _velocity.y = 8.25f;
        if (!Mathf.Approximately(_moveVal, 0f))
        {
            _rb.velocity = _velocity;
        }
    }

    private bool CheckForSteepProximity()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(this.transform.position, Vector2.right, 1f, 1 << 0);
        var hit2DCollider = hit2D.collider;
        if (hit2DCollider != null)
        {
            _steepNormal = hit2D.normal;
            //_slopeDir = transform.position.x > hit2DCollider.ClosestPoint(transform.position).x ? -1 : 1;
            return true;
        }

        hit2D = Physics2D.Raycast(this.transform.position, Vector2.left, 1f, 1 << 0);
        if (hit2DCollider != null)
        {
            _steepNormal = hit2D.normal;
            //_slopeDir = transform.position.x > hit2DCollider.ClosestPoint(transform.position).x ? -1 : 1;

            return true;
        }

        return false;
    }

    public void ClearState()
    {
        _onGround = _coyoteJump;
        _onSteep = _desiredDash = _steepProximity = false;
        _contactNormal = _steepNormal = Vector2.zero;
        _slopeDir = 0;
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
        OnDash?.Invoke();
        
        _isDashing = true;
        
        if (Mathf.Approximately(_dashDir.sqrMagnitude, 0f)) //needs to be recorded before setting velocity to zero
        {
            _dashDir = _facingRight ? Vector2.right : Vector2.left;
        }

        _dashDir.Normalize();

        _rb.velocity = Vector2.zero;
        _isStill = false;
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
        _coyoteJump = false; //careful about this here
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        EvaluateCollision(other);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        EvaluateCollision(other);
        _coyoteJump = !_onGround && !_onSteep && _jumpPhase == 0;
        _stepsSinceCoyoteFlag = 0;
    }

    private void EvaluateCollision(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.contacts[i].normal;


            if ((normal.y >= _minGroundDotProduct))
            {
                _onGround = true;
                _jumpCount = 0;
                _contactNormal += normal;
            }
            else if (normal.y > -0.01f)
            {
                _onSteep = true;
                _steepNormal += normal;
                _slopeDir = transform.position.x > collision.contacts[i].point.x ? -1 : 1;
            }
        }

        if (_onGround && _onSteep) // a hack
        {
            _onSteep = false;
            _slopeDir = 0;
        }
    }

    Vector2 ProjectOnContactPlane(Vector2 vector)
    {
        //return vector - contactNormal * Vector2.Dot(vector, contactNormal);
        return vector;
    }

    //Uncomment to Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.left * 1f);
        Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.right * 1f);
    }
}