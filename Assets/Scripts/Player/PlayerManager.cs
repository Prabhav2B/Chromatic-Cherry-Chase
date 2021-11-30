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

    [SerializeField] private float _dashTimerMax = 1.5f;

    private SpriteRenderer _characterSpriteRenderer;
    private ParticleSystem _particleSystem;
    private SpriteRenderer _jumpIndicatorSpriteRenderer;
    private Transform _spriteRendererTransform;
    private CharController _characterController;
    private DashTimerCanvas _dashTimerCanvas;
    private TimeBend _timeBend;
    private Vector3 _lastCollisionPoint;
    private float _lastCollisionNormal;
    private bool _powerActive, _powerInputLock;
    private int _movementTweenFlag, _dotIndicatorTweenFlag;
    private PlayerInput _input;
    private ScreenFadeManager _screenFadeManager;
    private MainMenu _mainMenu;
    private Vector3 _initalPosition;
    private bool _dashInputHeld, _dashTimerExceeded;
    private float _dashTimer;

    protected Vector2 MoveVector;
    public Vector2 ReceivedInput { get; private set; }
    public bool PowerActive => _powerActive;

    public bool DashInputHeld => _dashInputHeld;

    public bool DashViable => !_dashTimerExceeded || _characterController.OnGround;

    public Action OnLand;
    public Action OnSwitch;

    public ControlScheme CurrentControlScheme { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        _input = GetComponent<PlayerInput>();

        _characterController = GetComponent<CharController>();
        _timeBend = GetComponent<TimeBend>();
        _dashTimerCanvas = GetComponentInChildren<DashTimerCanvas>();
        _screenFadeManager = FindObjectOfType<ScreenFadeManager>();
        _mainMenu = FindObjectOfType<MainMenu>();
        

        _characterSpriteRenderer = this.GetComponentsInChildren<SpriteRenderer>()[0];
        _jumpIndicatorSpriteRenderer = this.GetComponentsInChildren<SpriteRenderer>()[1];
        _spriteRendererTransform = _characterSpriteRenderer.transform;

        _particleSystem = this.GetComponentInChildren<ParticleSystem>();

        Deactivate();
    }

    private void Start()
    {
        InputSystem.onActionChange += (obj, change) =>
        {
            if (change == InputActionChange.ActionPerformed)
            {
                var inputAction = (InputAction) obj;
                var lastControl = inputAction.activeControl;
                var lastDevice = lastControl.device;

                CurrentControlScheme = lastDevice.displayName.Equals("Xbox Controller")
                    ? ControlScheme.Gamepad
                    : ControlScheme.KeyboardAndMouse;
            }
        };

        _initalPosition = transform.position;
        _movementTweenFlag = -1;
        _dashTimer = 0f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _levelResetHandler.onLevelReload += _characterController.ClearState;
        _levelResetHandler.onLevelReload += ResetPlayerPosition;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _levelResetHandler.onLevelReload -= _characterController.ClearState;
        _levelResetHandler.onLevelReload -= ResetPlayerPosition;
    }

    public void ResetScene()
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


    public void OnMainMenu(InputAction.CallbackContext context)
    {
        _mainMenu.TriggerMainMenu();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        ReceivedInput = context.ReadValue<Vector2>();
        //PlayerInput = Vector2.ClampMagnitude(PlayerInput, 1f);
        _characterController.Move(ReceivedInput);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (Math.Abs(context.ReadValue<float>() - 1f) < 0.5f)
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

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && !_characterController.DashMaxed)
        {
            _dashTimer = 0f;
            _dashInputHeld = true;
            _dashTimerExceeded = false;
            if (!_characterController.OnGround)
            {
                _dashTimerCanvas.Activate();
                _timeBend.TimeBendInitiate();
            }
        }

        if (context.canceled && _dashInputHeld)
        {
            _dashTimerCanvas.Deactivate();
            _dashInputHeld = false;
            _dashTimer = 0f;
            _timeBend.TimeBendEnd();
            if(DashViable)
                _characterController.DashInitiate();
        }

    }

    public void OnPowerA(InputAction.CallbackContext context)
    {
        _powerActive = Math.Abs(context.ReadValue<float>() - 1f) < 0.5f;

        if (!_powerInputLock && _powerActive)
        {
            OnSwitch();
            _powerInputLock = true;
        }
        else if (_powerInputLock && !_powerActive)
        {
            OnSwitch();
            _powerInputLock = false;
        }

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

        if (DashInputHeld)
        {
            _dashTimerCanvas.DashFillAmount = 1.0f - (_dashTimer / _dashTimerMax);
            _dashTimer += Time.unscaledDeltaTime;
            if (_dashTimer > _dashTimerMax)
            {
                _dashTimerCanvas.Deactivate();
                _dashTimerExceeded = true;
                _dashTimer = 0f;
                _timeBend.TimeBendEnd();
            }
        }

        CheckForSpriteUpdates();
    }

    private void CheckForSpriteUpdates()
    {
        switch (_characterController.DashMaxed)
        {
            case true:
                _characterSpriteRenderer.sprite = !_powerActive ? playerCoreB : playerCoreBBlock;
                _jumpIndicatorSpriteRenderer.sprite = playerCoreBJumpIndicator;
                break;
            case false:
                _characterSpriteRenderer.sprite = !_powerActive ? playerCoreA : playerCoreABlock;
                _jumpIndicatorSpriteRenderer.sprite = playerCoreAJumpIndicator;
                break;
        }

        switch (_characterController.JumpMaxed)
        {
            case true when _dotIndicatorTweenFlag != 1:
                _jumpIndicatorSpriteRenderer.DOFade(0f, 0.2f);
                _dotIndicatorTweenFlag = 1;
                break;
            case false when _dotIndicatorTweenFlag != 0:
                _jumpIndicatorSpriteRenderer.DOFade(255f, 0.2f);
                _dotIndicatorTweenFlag = 0;
                break;
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

        OnLand?.Invoke();
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