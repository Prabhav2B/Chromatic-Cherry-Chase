using System;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharController))]
public class PlayerManager : SingleInstance<PlayerManager>
{
    [SerializeField] private Sprite playerCoreA;
    [SerializeField] private Sprite playerCoreABlock;
    [SerializeField] private Sprite playerCoreAJumpIndicator;
    [SerializeField] private Sprite playerCoreB;
    [SerializeField] private Sprite playerCoreBBlock;
    [SerializeField] private Sprite playerCoreBJumpIndicator;

    private SpriteRenderer _characterSpriteRenderer;
    private ParticleSystem _particleSystem;
    private SpriteRenderer _jumpIndicatorSpriteRenderer;
    private Transform _spriteRendererTransform;
    private CharController _characterController;
    private TimeBend _timeBend;
    private Vector3 _lastCollisionPoint;
    private float _lastCollisionNormal;
    private bool _powerActive, _powerInputLock;
    private int _movementTweenFlag;
    private PlayerInput _input;
    private ScreenFadeManager _screenFadeManager;
    private Vector3 _initalPosition;

    protected Vector2 MoveVector;
    public Vector2 ReceivedInput { get; private set; }
    public bool PowerActive => _powerActive;
    

    public ControlScheme CurrentControlScheme { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        _input = GetComponent<PlayerInput>();

        _characterController = GetComponent<CharController>();
        _timeBend = GetComponent<TimeBend>();
        _screenFadeManager = FindObjectOfType<ScreenFadeManager>();

        _characterSpriteRenderer = this.GetComponentsInChildren<SpriteRenderer>()[0];
        _jumpIndicatorSpriteRenderer = this.GetComponentsInChildren<SpriteRenderer>()[1];
        _spriteRendererTransform = _characterSpriteRenderer.transform;

        _particleSystem = this.GetComponentInChildren<ParticleSystem>();
        UpdateCurrentScheme(_input.currentControlScheme);

        Deactivate();
    }

    private void Start()
    {
        _initalPosition = transform.position;
        _movementTweenFlag = -1;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        InputUser.onChange += onInputDeviceChange;
        _levelResetHandler.onLevelReload += _characterController.ClearState;
        _levelResetHandler.onLevelReload += ResetPlayerPosition;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        InputUser.onChange -= onInputDeviceChange;
        _levelResetHandler.onLevelReload -= _characterController.ClearState;
        _levelResetHandler.onLevelReload -= ResetPlayerPosition;
    }
    
    private void OnResetScene()
    {
        Deactivate(); //Turn of input from user
        ScreenFadeManager.PostFadeOut fadeOutAction = _levelResetHandler.ExecuteLevelReload;
        fadeOutAction += _screenFadeManager.FadeIn;
        fadeOutAction += Activate;
        _screenFadeManager.FadeOut(fadeOutAction);
        fadeOutAction = null;

    }


    private void ResetPlayerPosition()
    {
        transform.position = _initalPosition;
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
        //PlayerInput = Vector2.ClampMagnitude(PlayerInput, 1f);
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

    // private void OnTimeBend(InputValue input)
    // {
    //     if (Math.Abs(input.Get<float>() - 1f) < 0.5f)
    //     {
    //         _timeBend.TimeBendInitiate();
    //     }
    //     else
    //     {
    //         _timeBend.TimeBendEnd();
    //     }
    // }

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
        // if (input.Get<float>() > 0.8f && !_powerInputLock)
        // {
        //     _powerActive = !_powerActive;
        //     _powerInputLock = true;
        //     SquishStart();
        //     CheckForSpriteUpdates();
        // }
        // else if ( input.Get<float>() < 0.8f )
        // {
        //     _powerInputLock = false;
        //     // SquishStart();
        //     //CheckForSpriteUpdates();
        // }

        _powerActive = Math.Abs(input.Get<float>() - 1f) < 0.5f;
         SquishStart();
         CheckForSpriteUpdates();
    }

    void Update()
    {
        if (_characterController.IsStill)
        {
            if (_movementTweenFlag != 0)
            {
                _movementTweenFlag = 0;
                _spriteRendererTransform.DOLocalRotate(Vector3.zero, 0.2f);
            }
        }
        else if (_characterController.FacingRight)
        {
            if (_movementTweenFlag != 1)
            {
                _movementTweenFlag = 1;
                _spriteRendererTransform.DOLocalRotate(Vector3.forward * -5f, 0.1f);
                _characterSpriteRenderer.flipX = false;
            }
        }
        else
        {
            if (_movementTweenFlag != 2)
            {
                _movementTweenFlag = 2;
                _spriteRendererTransform.DOLocalRotate(Vector3.forward * 5f, 0.1f);
                _characterSpriteRenderer.flipX = true;
            }
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

    public void Deactivate()
    {
        GetComponent<PlayerInput>().enabled = false;
    }

    public void Activate()
    {
        GetComponent<PlayerInput>().enabled = true;
    }
}