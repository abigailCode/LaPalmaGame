using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;
    [SerializeField] float lineLength = 1f;
    [SerializeField] float offset = 1f;
    [SerializeField] ParticleSystem particles;
    private float CoyoteTime = 0.2f;
    private float ctt;

    [SerializeField] bool isJumping = false;

    private Rigidbody2D rb;


    void Start()
    {
        ctt = CoyoteTime;
        rb = GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector2.right * horizontalInput * playerSpeed * Time.deltaTime);

        if (horizontalInput == 1) GetComponent<SpriteRenderer>().flipX = true;
        if (horizontalInput == -1) GetComponent<SpriteRenderer>().flipX = false;


        /*
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && !isJumping)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            //AudioManager.instance.PlaySFX("Jump");
            //particles.Play();

        }
        */


        Vector2 origin = new Vector2(transform.position.x, transform.position.y - offset);
        Vector2 target = new Vector2(transform.position.x, transform.position.y - offset - lineLength);
        Debug.DrawLine(origin, target, Color.black);

        RaycastHit2D raycast = Physics2D.Raycast(origin, Vector2.down, lineLength);

        if (raycast.collider == null)
        {
            ctt = ctt - Time.deltaTime;
            Debug.Log($"Tiempo de salto restante: {ctt}");

            if (ctt <= 0 || rb.velocity.y > 0)
            {
                isJumping = true;
                // SetAnimation("Jump");
            }
            else if (ctt > 0 && (!isJumping && (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))))
            {
                Jump();
                //rb.AddForce(Vector2.up*jumpForce, ForceMode2D.Impulse);
                isJumping = true;
            }
        }
        else
        {
            ctt = CoyoteTime;
            Debug.Log($"Tiempo de salto restante: {ctt}");
            isJumping = false;

            if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) Jump();
            if ((Input.GetButtonDown("Fire1") || Input.GetKey(KeyCode.Space)) && rb.velocity.y<0) rb.velocity = new Vector2(rb.velocity.x, jumpForce+2);
        }

        

        // ------------------------------------------------------------------------------------------
        // APLICACI�N AL JUEGO DE PLATAFORMAS QUE UTILIZA RAYCAST PARA DETECTAR QUE EST� EN EL SUELO
        // ------------------------------------------------------------------------------------------
        // Si el raycast no toca con nada el personaje est� en el aire
        if (raycast.collider == null)
        {
           
        }
        else
        {
            // Si est� sobre una superficie pero se mueve lateralmente
           // if (rb.velocity.x != 0) SetAnimation("Walk");
            //else SetAnimation("Idle"); // Si est� sobre una superficie pero no se mueve
        }

        ApplyCustomGravity();

    }

    void Jump()
    {
        // Establecer la velocidad vertical instant�neamente al valor de salto
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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
    void SetAnimation(string name)
    {

        // Obtenemos todos los par�metros del Animator
        AnimatorControllerParameter[] parametros = GetComponent<Animator>().parameters;

        // Recorremos todos los par�metros y los ponemos a false
        foreach (var item in parametros) GetComponent<Animator>().SetBool(item.name, false);

        // Activamos el pasado por par�metro
        GetComponent<Animator>().SetBool(name, true);

    }

    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision != null)
    //    {
    //        if (collision.collider.CompareTag("Enemy"))
    //        {
    //            AudioManager.instance.PlaySFX("Hit");
    //            AudioManager.instance.PlayMusic("LoseALife");
    //            SceneController.instance.LoadScene("Gameover");

    //        }
    //    }
    //}
}