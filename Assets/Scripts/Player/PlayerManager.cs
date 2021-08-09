using System;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.Users;

[RequireComponent(typeof(CharController))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Sprite playerCoreA;
    [SerializeField] private Sprite playerCoreABlock;
    [SerializeField] private Sprite playerCoreAJumpIndicator;
    [SerializeField] private Sprite playerCoreB;
    [SerializeField] private Sprite playerCoreBBlock;
    [SerializeField] private Sprite playerCoreBJumpIndicator;


    private static PlayerManager _playerInstance;

    public static PlayerManager PlayerInstance
    {
        get { return _playerInstance; }
    }

    private SpriteRenderer _characterSpriteRenderer;
    private ParticleSystem _particleSystem;
    private SpriteRenderer _jumpIndicatorSpriteRenderer;
    private Transform _spriteRendererTransform;
    private CharController _characterController;
    private TimeBend _timeBend;
    private Vector3 _lastCollisionPoint;
    private float _lastCollisionNormal;
    private bool _powerActive;
    private PlayerInput _input;

    protected Vector2 MoveVector;
    public Vector2 ReceivedInput { get; private set; }
    public bool PowerActive => _powerActive;

    public ControlScheme CurrentControlScheme { get; private set; }


    private void Awake()
    {
        _playerInstance = this;
        _input = GetComponent<PlayerInput>();

        _characterController = GetComponent<CharController>();
        _timeBend = GetComponent<TimeBend>();

        _characterSpriteRenderer = this.GetComponentsInChildren<SpriteRenderer>()[0];
        _jumpIndicatorSpriteRenderer = this.GetComponentsInChildren<SpriteRenderer>()[1];
        _spriteRendererTransform = _characterSpriteRenderer.transform;

        _particleSystem = this.GetComponentInChildren<ParticleSystem>();
        UpdateCurrentScheme(_input.currentControlScheme);
        
    }
    
    void OnEnable() {
        InputUser.onChange += onInputDeviceChange;
    }
 
    void OnDisable() {
        InputUser.onChange -= onInputDeviceChange;
    }

    private void onInputDeviceChange(InputUser user, InputUserChange change, InputDevice device)
    {
        if (change == InputUserChange.ControlSchemeChanged)
        {
            if (user.controlScheme != null) UpdateCurrentScheme(user.controlScheme.Value.name);
        }
    }

    private void UpdateCurrentScheme(string schemeName)
    {
        CurrentControlScheme = schemeName.Equals("Gamepad") ? ControlScheme.Gamepad : ControlScheme.KeyboardAndMouse;
    }

    private void OnMove(InputValue input)
    {
        ReceivedInput = input.Get<Vector2>();
        // PlayerInput = Vector2.ClampMagnitude(PlayerInput, 1f);
        _characterController.Move(ReceivedInput);
    }

    private void OnJump(InputValue input)
    {
        if (Math.Abs(input.Get<float>() - 1f) < 0.5f)
        {
            _characterController.JumpInitiate();
            if (!_characterController.JumpMaxed)
                SquishStart();
        }
        else
        {
            _characterController.JumpEnd();
        }
    }

    private void OnTimeBend(InputValue input)
    {
        if (Math.Abs(input.Get<float>() - 1f) < 0.5f)
        {
            _timeBend.TimeBendInitiate();
        }
        else
        {
            _timeBend.TimeBendEnd();
        }
    }

    private void OnDash(InputValue input)
    {
        if (Math.Abs(input.Get<float>() - 1f) < 0.5f)
        {
            _characterController.DashInitiate();
            //if(!characterController.JumpMaxed)
            //SquishStart();
        }
        else
        {
            _characterController.DashEnd();
        }
    }

    private void OnPowerA(InputValue input)
    {
        if (Math.Abs(input.Get<float>() - 1f) < 0.5f)
        {
            _powerActive = true;
        }
        else
        {
            _powerActive = false;
        }
        SquishStart();
        CheckForSpriteUpdates();
    }

    void Update()
    {
        if (_characterController.IsStill)
        {
            _spriteRendererTransform.DOLocalRotate(Vector3.zero, 0.4f);
        }
        else if (_characterController.FacingRight)
        {
            //turn into a tweener?
            _spriteRendererTransform.DOLocalRotate(Vector3.forward * -5f, 1.0f);
            _characterSpriteRenderer.flipX = false;
        }
        else
        {
            //turn into a tweener?
            _spriteRendererTransform.DOLocalRotate(Vector3.forward * 5f, 1.0f);
            _characterSpriteRenderer.flipX = true;
        }

        var particleSystemEmission = _particleSystem.emission;
        var rateOverTime = particleSystemEmission.GetBurst(0);
        if (_characterController.OnSteep || _characterController.OnDownwardSlope || _characterController.IsDashing)
        {
            var main = _particleSystem.main;
            main.loop = true;

            rateOverTime.minCount = 1;
            rateOverTime.maxCount = 1;
        }
        else
        {
            var main = _particleSystem.main;
            main.loop = false;

            rateOverTime.minCount = 5;
            rateOverTime.maxCount = 15;
        }

        CheckForSpriteUpdates();
    }

    private void CheckForSpriteUpdates()
    {
        if (_characterController.DashMaxed)
        {
            _characterSpriteRenderer.sprite = !_powerActive ? playerCoreB : playerCoreBBlock;
            _jumpIndicatorSpriteRenderer.sprite = playerCoreBJumpIndicator;
        }
        else if (!_characterController.DashMaxed)
        {
            _characterSpriteRenderer.sprite = !_powerActive ? playerCoreA : playerCoreABlock;
            _jumpIndicatorSpriteRenderer.sprite = playerCoreAJumpIndicator;
        }

        if (_characterController.JumpMaxed)
        {
            _jumpIndicatorSpriteRenderer.DOFade(0f, 0.2f);
        }
        else
        {
            _jumpIndicatorSpriteRenderer.DOFade(255f, 0.2f);
        }
    }

    void SquishStart()
    {
        _spriteRendererTransform.DOScale(new Vector3(.75f, 1.25f, 1.0f), .15f)
            .OnComplete(SquishRelease);
    }

    void SquishRelease()
    {
        _spriteRendererTransform.DOScale(new Vector3(1f, 1f, 1.0f), .15f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var camTransition = other.GetComponent<CinemachineTransitionInfo>();
        if (camTransition != null)
        {
            camTransition.OnEnterCameraTransition?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var camTransition = other.GetComponent<CinemachineTransitionInfo>(); //repetition
        if (camTransition != null)
        {
            camTransition.OnExitCameraTransition?.Invoke();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _particleSystem.gameObject.transform.position = other.contacts[0].point;
        _particleSystem.gameObject.transform.rotation =
            Quaternion.Euler(new Vector3(0f, 0f, other.contacts[0].normal.x * -90f));
        _particleSystem.Play();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        _lastCollisionPoint = other.contacts[0].point;
        _lastCollisionNormal = other.contacts[0].normal.x * -90f;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _particleSystem.gameObject.transform.position = _lastCollisionPoint;
        _particleSystem.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, _lastCollisionNormal));
        _particleSystem.Play();
    }

    public void SetExternalVelocity(Vector2 externalVelocity)
    {
        _characterController.ExternalVelocity = externalVelocity;
    }

    public void GyserExit()
    {
        _characterController.ClampVelocity();
    }

    public enum ControlScheme
    {
        KeyboardAndMouse,
        Gamepad
    }
}