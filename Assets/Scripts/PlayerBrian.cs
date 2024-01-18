using UnityEngine;
using UnityEngine.UI;

public class JumpKingController : MonoBehaviour
{
    public float minJumpForce = 5f;
    public float maxJumpForce = 20f;
    public float wallBounceForce = 5f;
    public float chargeSpeed = 5f;
    public float maxChargeTime = 2f;

    private Rigidbody2D rb;
    private bool isJumping = false;
    private float currentJumpForce = 0f;
    private float chargeTime = 0f;


    public Slider jumpSlider; // Asigna un objeto Slider desde el editor
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentJumpForce = minJumpForce;
        chargeTime = 0f;
        // Obtén el componente Animator
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Detectar el input de salto
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            StartCharging();
        }

        // Detectar la liberación del botón de salto
        if (Input.GetButtonUp("Jump") && isJumping)
        {
            Jump();
        }

        // Actualizar la barra de carga
        UpdateJumpSlider();

        // Actualizar el estado del Animator
        UpdateAnimatorState();
    }

    void StartCharging()
    {
        isJumping = true;
        currentJumpForce = minJumpForce;
        chargeTime = 0f;
        
        // Activar el Trigger de carga en el Animator
        animator.SetTrigger("ChargingJump");
    }

    void Jump()
    {
        // Aplicar la fuerza acumulada al realizar un salto
        rb.velocity = new Vector2(rb.velocity.x, currentJumpForce);
        isJumping = false;
        currentJumpForce = minJumpForce;
        chargeTime = 0f;

        // Reiniciar la animación y desactivar el Trigger de carga en el Animator
        animator.SetTrigger("JumpingDown");
    }

    void UpdateJumpSlider()
    {
         // Actualizar la barra de carga
        if (isJumping)
        {
            chargeTime += Time.deltaTime;
            currentJumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeTime / maxChargeTime);

            // Limitar la fuerza máxima
            currentJumpForce = Mathf.Clamp(currentJumpForce, minJumpForce, maxJumpForce);

            // Actualizar el valor de la barra de carga
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

        if (verticalVelocity > 0)
        {
            // Activar la animación de subida si la velocidad es positiva
            animator.SetTrigger("JumpingUp");
        }
        else if (verticalVelocity < 0)
        {
            // Activar la animación de bajada si la velocidad es negativa
            animator.SetTrigger("JumpingDown");
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
        if (collision.gameObject.CompareTag("Floor"))
        {
            isJumping = false;
            currentJumpForce = minJumpForce;
            chargeTime = 0f;

            // Reiniciar la animación al tocar el suelo
            animator.SetTrigger("Idle");
        }
    }
}
