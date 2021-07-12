using System;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(CharController))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Sprite playerCoreA;
    [SerializeField] private Sprite playerCoreB;

    private static PlayerManager playerInstance;

    public static PlayerManager PlayerInstance
    {
        get { return playerInstance; }
    }

    private SpriteRenderer spriteRenderer;
    private Transform spriteRendererTransform;
    private Rigidbody2D rb;
    private CharController characterController;
    private TimeBend timeBend;

    protected Vector2 moveVector;
    public Vector2 PlayerInput { get; private set; }


    private void Awake()
    {
        playerInstance = this;

        rb = this.GetComponent<Rigidbody2D>();
        characterController = GetComponent<CharController>();
        timeBend = GetComponent<TimeBend>();

        spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        spriteRendererTransform = spriteRenderer.transform;
    }

    private void OnMove(InputValue input)
    {
        //Gets Raw Axis Values : 1, 0, or -1
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
        spriteRenderer.sprite = playerCoreA;
    }

    private void OnPowerB(InputValue input)
    {
        spriteRenderer.sprite = playerCoreB;
    }

    void Update()
    {
        if (rb.velocity.x > characterController.MaxSpeed * 0.75f)
        {
            //turn into a tweener?
            spriteRendererTransform.DOLocalRotate(Vector3.forward * -5f, 1.0f);
            spriteRenderer.flipX = false;
        }
        else if (rb.velocity.x < characterController.MaxSpeed * -0.75f)
        {
            //turn into a tweener?
            spriteRendererTransform.DOLocalRotate(Vector3.forward * 5f, 1.0f);
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRendererTransform.DOLocalRotate(Vector3.zero, 0.4f);
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
}