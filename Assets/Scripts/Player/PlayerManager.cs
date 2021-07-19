using System;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(CharController))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Sprite playerCoreA;
    [SerializeField] private Sprite playerCoreABlock;
    [SerializeField] private Sprite playerCoreAJumpIndicator;
    [SerializeField] private Sprite playerCoreB;
    [SerializeField] private Sprite playerCoreBBlock;
    [SerializeField] private Sprite playerCoreBJumpIndicator;

    private static PlayerManager playerInstance;

    public static PlayerManager PlayerInstance
    {
        get { return playerInstance; }
    }

    private SpriteRenderer characterSpriteRenderer;
    private SpriteRenderer jumpIndicatorSpriteRenderer;
    private Transform spriteRendererTransform;
    private Rigidbody2D rb;
    private CharController characterController;
    private TimeBend timeBend;
    private bool powerActive;

    protected Vector2 moveVector;
    public Vector2 PlayerInput { get; private set; }
    public bool PowerActive => powerActive;


    private void Awake()
    {
        playerInstance = this;

        rb = this.GetComponent<Rigidbody2D>();
        characterController = GetComponent<CharController>();
        timeBend = GetComponent<TimeBend>();

        characterSpriteRenderer = this.GetComponentsInChildren<SpriteRenderer>()[0];
        jumpIndicatorSpriteRenderer = this.GetComponentsInChildren<SpriteRenderer>()[1];
        spriteRendererTransform = characterSpriteRenderer.transform;
    }

    private void OnMove(InputValue input)
    {
        PlayerInput = input.Get<Vector2>();
        // PlayerInput = Vector2.ClampMagnitude(PlayerInput, 1f);
        characterController.Move(PlayerInput);
    }

    private void OnJump(InputValue input)
    {
        if (Math.Abs(input.Get<float>() - 1f) < 0.5f)
        {
            characterController.JumpInitiate();
            if (!characterController.JumpMaxed)
                SquishStart();
        }
        else
        {
            characterController.JumpEnd();
        }
    }

    private void OnTimeBend(InputValue input)
    {
        if (Math.Abs(input.Get<float>() - 1f) < 0.5f)
        {
            timeBend.TimeBendInitiate();
        }
        else
        {
            timeBend.TimeBendEnd();
        }
    }

    private void OnDash(InputValue input)
    {
        if (Math.Abs(input.Get<float>() - 1f) < 0.5f)
        {
            characterController.DashInitiate();
            //if(!characterController.JumpMaxed)
            //SquishStart();
        }
        else
        {
            characterController.DashEnd();
        }
    }

    private void OnPowerA(InputValue input)
    {
        if (Math.Abs(input.Get<float>() - 1f) < 0.5f)
        {
            powerActive = true;
        }
        else
        {
            powerActive = false;
        }
        CheckForSpriteUpdates();
    }

    void Update()
    {
        if(characterController.IsStill)
        {
            spriteRendererTransform.DOLocalRotate(Vector3.zero, 0.4f);
        }
        else if (characterController.FacingRight)
        {
            //turn into a tweener?
            spriteRendererTransform.DOLocalRotate(Vector3.forward * -5f, 1.0f);
            characterSpriteRenderer.flipX = false;
        }
        else
        {
            //turn into a tweener?
            spriteRendererTransform.DOLocalRotate(Vector3.forward * 5f, 1.0f);
            characterSpriteRenderer.flipX = true;
        }

        CheckForSpriteUpdates();
    }

    private void CheckForSpriteUpdates()
    {
        if (characterController.DashMaxed)
        {
            characterSpriteRenderer.sprite = !powerActive?playerCoreB:playerCoreBBlock;
            jumpIndicatorSpriteRenderer.sprite = playerCoreBJumpIndicator;
        }
        else if (!characterController.DashMaxed)
        {
            characterSpriteRenderer.sprite = !powerActive?playerCoreA:playerCoreABlock;
            jumpIndicatorSpriteRenderer.sprite = playerCoreAJumpIndicator;
        }

        if (characterController.JumpMaxed)
        {
            jumpIndicatorSpriteRenderer.DOFade(0f, 0.2f);
        }
        else
        {
            jumpIndicatorSpriteRenderer.DOFade(255f, 0.2f);
        }
    }

    void SquishStart()
    {
        spriteRendererTransform.DOScale(new Vector3(.75f, 1.25f, 1.0f), .15f)
            .OnComplete(SquishRelease);
    }

    void SquishRelease()
    {
        spriteRendererTransform.DOScale(new Vector3(1f, 1f, 1.0f), .15f);
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
        this.GetComponentInChildren<ParticleSystem>().gameObject.transform.position = other.contacts[0].point;
        this.GetComponentInChildren<ParticleSystem>().Play();
    }

    public void SetExternalVelocity(Vector2 externalVelocity)
    {
        characterController.ExternalVelocity = externalVelocity;
    }

    public void GyserExit()
    {
        characterController.ClampVelocity();
    }
}