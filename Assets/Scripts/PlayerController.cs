using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed = 4f;
    [SerializeField] float minJumpForce = 5f;
    [SerializeField] float maxJumpForce = 20f;
    [SerializeField] float wallBounceForce = 5f;
    [SerializeField] float chargeSpeed = 5f;
    [SerializeField] float maxChargeTime = 2f;
    [SerializeField] float moveSpeed = 5f;

    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;

    [SerializeField] float lineLength = 0.1f;   // Longitud de la l�nea de detecci�n de suelo
    [SerializeField] float offSetY = 0.66f;       // Desplazamiento vertical de la l�nea de detecci�n
    [SerializeField] float offSetX = 0.47f;       // Desplazamiento vertical de la l�nea de detecci�n
    [SerializeField] ParticleSystem particles;

    private Rigidbody2D rb;
    [SerializeField] bool isJumping = false;
    [SerializeField] float currentJumpForce = 0f;
    [SerializeField] float chargeTime = 0f;
    private float horizontalInput;
    [SerializeField] bool isFacingRight = true;
    [SerializeField] bool isGrounded = false;

    public Slider jumpSlider;
    private Animator animator;

    //SLIDER
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentJumpForce = minJumpForce;
        chargeTime = 0f;

        animator = GetComponent<Animator>();
    }

    void Update()
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

        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && !isJumping)
        {
            StartCharging();
        }
        
        if ((Input.GetButtonUp("Fire1") || Input.GetKeyUp(KeyCode.Space)) && isJumping)
        {
            Jump();
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        if (!isJumping) Move(horizontalInput);        

        UpdateJumpSlider();

        UpdateAnimatorState();

        ApplyCustomGravity();

        // if(!isJumping)
        // {
        //     if (horizontalInput > 0) GetComponent<SpriteRenderer>().flipX = false;
        //     else if (horizontalInput < 0) GetComponent<SpriteRenderer>().flipX = true;
        // }

        // if (raycast1.collider == null && raycast2.collider == null)
        // {
        //     rb.sharedMaterial.bounciness = 5f;
        //     rb.sharedMaterial.friction = 0;
        //     Debug.Log("Con rebote");
        //     //Debug.Log($"Tiempo de salto restante: {ctt}");

        //     if (rb.velocity.y > 0)
        //     {
        //         isJumping = true;
        //         SetAnimation("Jump");
        //     }
        //     else if (!isJumping && (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)))
        //     {
        //         Jump();
        //         //rb.AddForce(Vector2.up*jumpForce, ForceMode2D.Impulse);
        //         isJumping = true;
        //     }
        // }
        // else
        // {

           

        //         rb.sharedMaterial.bounciness = 0;
        //         rb.sharedMaterial.friction = 10;
        //         Debug.Log("Sin rebote");
             

        //     if (!isJumping) rb.velocity = new Vector2(playerSpeed * horizontalInput, rb.velocity.y);
        //         else rb.velocity = new Vector2(0, rb.velocity.y);
        //         isJumping = false;
        //         sliderValue = 0;
        // }


        // if (!(Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space)) && chargeTime > 0)
        // {
        //     if(chargeTime <0.35f) { chargeTime = 0.35f; Debug.Log("Nv. de salto 1: x0.35"); }            
            
        //     Jump();
        
        // }
    }

    void Move(float horizontalInput)
    {
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        rb.velocity = movement;

        // Flip del personaje según la dirección
        if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        // Cambiar la dirección del personaje
        isFacingRight = !isFacingRight;

        // Invertir la escala en el eje X para voltear el sprite
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void StartCharging()
    {
        isJumping = true;
        currentJumpForce = minJumpForce;
        chargeTime = 0f;

        Debug.Log("Cargando salto");
    }
    void Jump()
    {
        // Establecer la velocidad vertical instant�neamente al valor de salto
        // if(Input.GetAxisRaw("Horizontal")!=0) rb.AddForce(new Vector2(Input.GetAxisRaw("Horizontal")*(playerSpeed/1.5f), minJumpForce*chargeTime), ForceMode2D.Impulse);
        rb.velocity = new Vector2(rb.velocity.x, currentJumpForce);        
        isJumping = false;
        currentJumpForce = minJumpForce;
        chargeTime = 0f;

        animator.SetTrigger("Jump");
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
                jumpSlider.value = (currentJumpForce - minJumpForce) / (maxJumpForce - minJumpForce);
            }
        }
    }
    void UpdateAnimatorState()
    {
        // Actualizar el estado del Animator basado en la velocidad vertical
        float verticalVelocity = rb.velocity.y;
        float horizontalVelocity = rb.velocity.x;

        if (verticalVelocity > 0.1 && !isJumping)
        {
            // Activar la animación de subida si la velocidad es positiva
            animator.SetTrigger("Jump");
        }
        else if (verticalVelocity < -0.1 && !isJumping)
        {
            // Activar la animación de bajada si la velocidad es negativa
            animator.SetTrigger("Fall");
        }
        else if (Math.Abs(horizontalVelocity) > 0 && isGrounded && !isJumping)
        {
            // Activar la animación de movimiento si la velocidad horizontal es mayor que 0
            animator.SetTrigger("Run");
        }
        else if (!isJumping && isGrounded)
        {
            // Activar la animación de reposo si la velocidad horizontal es 0
            animator.SetTrigger("Idle");
        }
        else if (isJumping)
        {
            animator.SetTrigger("ChargingJump");
        }
    }
    void ApplyCustomGravity()
    {
        if (rb.velocity.y < 0)
        {
            // Aplicar gravedad adicional durante la ca�da para que se sienta m�s natural
            rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            // Aplicar gravedad reducida durante el salto mantenido para un control m�s preciso
            rb.velocity += Vector2.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Detectar colisiones con paredes y aplicar rebote
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Invertir la dirección horizontal
            rb.velocity = new Vector2(-rb.velocity.x * wallBounceForce, rb.velocity.y);
        }

        // Restablecer el estado de salto al tocar el suelo
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            currentJumpForce = minJumpForce;
            chargeTime = 0f;

            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false; // Ya no está en el suelo
        }
    }
}
