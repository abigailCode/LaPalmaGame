using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float playerSpeed = 4f;
    [SerializeField] float moveSpeed = 5f;
    
    [Header("Jump Settings")]
    [SerializeField] float minJumpForce = 5f;
    [SerializeField] float maxJumpForce = 20f;
    [SerializeField] float chargeSpeed = 5f;
    [SerializeField] float maxChargeTime = 2f;

    [Header("Gravity Settings")]
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;

    [Header("Visuals")]
    [SerializeField] ParticleSystem particles;
    public Slider jumpSlider;
    public Image slider;

    [Header("Ground Detection")]
    [SerializeField] private float offSetX = 0.47f;
    [SerializeField] private float offSetY = 0.67f;
    [SerializeField] private float lineLength = 0.15f;

    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] bool isJumping = false;
    [SerializeField] float currentJumpForce = 0f;
    [SerializeField] float chargeTime = 0f;
    [SerializeField] bool isFacingRight = true;
    [SerializeField] bool isGrounded = true;
    [SerializeField] bool isFalling = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentJumpForce = minJumpForce;
        chargeTime = 0f;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();

        float horizontalInput = Input.GetAxis("Horizontal");
        if (!isJumping && isGrounded && !isFalling) Move(horizontalInput);

        CheckGrounded();
        // GravityController();  
        UpdateJumpSlider();
        UpdateAnimatorState();
        ApplyCustomGravity();
    }

    private void HandleInput()
    {
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && !isJumping && isGrounded)
            StartCharging();

        if ((Input.GetButtonUp("Fire1") || Input.GetKeyUp(KeyCode.Space)) && isJumping)
            Jump();
    }
    void Move(float horizontalInput)
    {
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        rb.velocity = movement;

        // Flip del personaje según la dirección
        if (horizontalInput > 0 && !isFacingRight) Flip();
        else if (horizontalInput < 0 && isFacingRight) Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        gameObject.GetComponent<SpriteRenderer>().flipX = !isFacingRight;
    }

    void StartCharging()
    {
        if (!isFalling)
        {
            isJumping = true;
            currentJumpForce = minJumpForce;
            chargeTime = 0f;
            rb.velocity = Vector2.zero;
        }
    }
    void Jump()
    {
        rb.AddForce(Vector2.up * currentJumpForce, ForceMode2D.Impulse);
        isJumping = false;
        currentJumpForce = minJumpForce;
        chargeTime = 0f;
        animator.SetTrigger("Jump");

        if (jumpSlider != null)
        {
            jumpSlider.value = 0f;
            slider.fillAmount = 0f;
        }
    }

    void UpdateJumpSlider()
    {
        if (isJumping)
        {
            chargeTime += Time.deltaTime;
            currentJumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeTime / maxChargeTime);
            currentJumpForce = Mathf.Clamp(currentJumpForce, minJumpForce, maxJumpForce);

           if (jumpSlider != null)
            {
                jumpSlider.value = Mathf.Clamp01(chargeTime / maxChargeTime);
                slider.fillAmount = jumpSlider.value;
            }
        }
    }
    void UpdateAnimatorState()
    {
        float verticalVelocity = rb.velocity.y;
        float horizontalVelocity = rb.velocity.x;

        if (verticalVelocity > 0.1 && !isJumping && !isGrounded) animator.SetTrigger("Jump");
        else if (verticalVelocity < -0.1 && !isJumping && !isGrounded && !isFalling) animator.SetTrigger("Fall");
        else if (Math.Abs(horizontalVelocity) > 0 && isGrounded && !isJumping && !isFalling) animator.SetTrigger("Run");
        else if (!isJumping && isGrounded && !isFalling) animator.SetTrigger("Idle");
        else if (isJumping) animator.SetTrigger("ChargingJump");
    }
    void ApplyCustomGravity()
    {
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
            rb.velocity += Vector2.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }
    void CheckGrounded()
    {
        // Dibujamos la l�nea de detecci�n de suelo
        Vector2 origin1 = new Vector2(transform.position.x - offSetX, transform.position.y - offSetY);
        Vector2 target1 = new Vector2(transform.position.x - offSetX, transform.position.y - offSetY - lineLength);
        Debug.DrawLine(origin1, target1, Color.blue);
        // Dibujamos la l�nea de detecci�n de suelo
        Vector2 origin2 = new Vector2(transform.position.x + offSetX, transform.position.y - offSetY);
        Vector2 target2 = new Vector2(transform.position.x + offSetX, transform.position.y - offSetY - lineLength);
        Debug.DrawLine(origin2, target2, Color.blue);

        // Trazamos el Raycast2D para detectar el suelo
        RaycastHit2D raycast1 = Physics2D.Raycast(origin1, Vector2.down, lineLength);
        RaycastHit2D raycast2 = Physics2D.Raycast(origin2, Vector2.down, lineLength);

        isGrounded = (raycast1.collider != null || raycast2.collider != null);
    }

    // void GravityController()
    // {
    //     if (isGrounded)
    //         // rb.gravityScale = 0f;
    //     else if (!isGrounded && !isFalling)
    //         // rb.gravityScale = 1f;
    // }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !isJumping)
        {
            isGrounded = true;
            isJumping = false;
            currentJumpForce = minJumpForce;
            chargeTime = 0f;

            if (isFalling)
                rb.velocity = Vector2.zero;
                animator.ResetTrigger("Falling");
                animator.SetTrigger("Suelo");
                StartCoroutine(WaitAndIdleAfterFalling());  

        }
        else if (collision.gameObject.CompareTag("Wall") && !isGrounded)
        {
            isFalling = true;
            animator.SetTrigger("Falling");
        }
    }

    IEnumerator WaitAndIdleAfterFalling()
    {
        yield return new WaitForSeconds(1.0f);
        isFalling = false;
        animator.ResetTrigger("Suelo");
        animator.SetTrigger("Idle");
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // isGrounded = false;
            isFalling = false;
        }
    }

}
