using System;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(CharController))]
public class PlayerManager : MonoBehaviour
{
    private static PlayerManager playerInstance;
    public static PlayerManager PlayerInstance { get { return playerInstance; } }
    
    private SpriteRenderer spriteRenderer;
    private Transform spriteRendererTransform;
    private Rigidbody2D rb;
    private CharController characterController;

    protected Vector2 moveVector;
    public Vector2 PlayerInput { get; private set; }
    
    
    private void Awake()
    {
        playerInstance = this;

        rb = this.GetComponent<Rigidbody2D>();
        characterController = GetComponent<CharController>();

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
        }
        else
        {
            characterController.JumpEnd();
        }
    }

    void Update()
    {
        if (rb.velocity.x > characterController.MaxSpeed * 0.75f)
        {
            //turn into a tweener?
            spriteRendererTransform.DOLocalRotate(Vector3.forward * -18f, 1.0f);
            spriteRenderer.flipX = false;
        }
        else if (rb.velocity.x < characterController.MaxSpeed * -0.75f)
        {
            //turn into a tweener?
            spriteRendererTransform.DOLocalRotate(Vector3.forward * 18f, 1.0f);
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRendererTransform.DOLocalRotate(Vector3.zero, 0.4f);
        }
    }
}
