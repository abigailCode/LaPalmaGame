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
    private float CoyoteTime = 0.1f;
    private float chargeTime = 0f;
    private float ctt;

    Vector2 origin;
    Vector2 target;

    Vector2 origin2;
    Vector2 target2;

    RaycastHit2D raycast;
    RaycastHit2D raycast2;

    [SerializeField] bool isJumping = false;

    private Rigidbody2D rb;


    void Start()
    {
        ctt = CoyoteTime;
        rb = GetComponent<Rigidbody2D>();

        

        



    }

    void Update()
    {

        origin = new Vector2(transform.position.x - 0.55f, transform.position.y - offset);
        target = new Vector2(transform.position.x - 0.55f, transform.position.y - offset - lineLength);

        origin2 = new Vector2(rb.transform.localScale.x * transform.position.x + 0.45f, transform.position.y - offset);
        target2 = new Vector2(rb.transform.localScale.x * transform.position.x + 0.45f, transform.position.y - offset - lineLength);

        raycast = Physics2D.Raycast(origin, Vector2.down, lineLength);
        raycast2 = Physics2D.Raycast(origin2, Vector2.down, lineLength);

        

        ChargeJump();
        float horizontalInput = Input.GetAxis("Horizontal");


        if (horizontalInput > 0) GetComponent<SpriteRenderer>().flipX = false;
        if (horizontalInput < 0) GetComponent<SpriteRenderer>().flipX = true;


        /*
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && !isJumping)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            //AudioManager.instance.PlaySFX("Jump");
            //particles.Play();

        }
        */


        

        Debug.DrawLine(origin, target, Color.red);
        Debug.DrawLine(origin2, target2, Color.red);

        

        if ((raycast.collider == null && raycast2.collider == null))
        {
            rb.sharedMaterial.bounciness = 8f;
            rb.sharedMaterial.friction = 0;
            Debug.Log("Con rebote");
            ctt = ctt - Time.deltaTime;
            //Debug.Log($"Tiempo de salto restante: {ctt}");

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
            rb.sharedMaterial.bounciness = 0;
            rb.sharedMaterial.friction = 10;
            Debug.Log("Con rebote");
            transform.Translate(Vector2.right * horizontalInput * playerSpeed * Time.deltaTime);
            ctt = CoyoteTime;
            //Debug.Log($"Tiempo de salto restante: {ctt}");
            isJumping = false;

            //if (Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space)) Jump();
        }

       

        

        // ------------------------------------------------------------------------------------------
        // APLICACI�N AL JUEGO DE PLATAFORMAS QUE UTILIZA RAYCAST PARA DETECTAR QUE EST� EN EL SUELO
        // ------------------------------------------------------------------------------------------
        // Si el raycast no toca con nada el personaje est� en el aire
        if (raycast.collider == null && raycast2.collider == null)
        {
           
        }
        else
        {
            // Si est� sobre una superficie pero se mueve lateralmente
            // if (rb.velocity.x != 0) SetAnimation("Walk");
            //else SetAnimation("Idle"); // Si est� sobre una superficie pero no se mueve
            ctt = CoyoteTime;
            //Debug.Log($"Tiempo de salto restante: {ctt}");
        }

        ApplyCustomGravity();

        if (!(Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space)) && chargeTime > 0) {

            
            if(chargeTime <0.35f) { chargeTime = 0.35f; Debug.Log("Nv. de salto 1: x0.35"); }
            /*if (chargeTime < 0.7f && chargeTime > 0.3f) { chargeTime = 0.7f; Debug.Log("Nv. de salto 2: x0.7"); }
            if (chargeTime < 1.0f && chargeTime > 0.7f) { chargeTime = 1.0f; Debug.Log("Nv. de salto 3: x1.0"); }
            if (chargeTime < 1.2f && chargeTime > 1.0f) { chargeTime = 1.0f; Debug.Log("Nv. de salto 4: x1.2"); }
            */
            
            
            Jump();
        
        }

    }

    //void Jump()
    //{
    //    // Establecer la velocidad vertical instant�neamente al valor de salto
    //    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    //    isJumping = true;
    //}
    void Jump()
    {
        // Establecer la velocidad vertical instant�neamente al valor de salto

        
        if((Input.GetAxisRaw("Horizontal")!=0))rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal")*playerSpeed, jumpForce*chargeTime);
        else rb.velocity = new Vector2(0, jumpForce * chargeTime);
        isJumping = true;
        chargeTime = 0;
        Debug.Log("chargeTime reseteado a 0");
    }

    void ChargeJump() {

        if ((Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space)) && !isJumping) {

            
            chargeTime = chargeTime + Time.deltaTime;
            if (chargeTime >= 1.25f) { chargeTime = 1.25f;}
            Debug.Log($"El tiempo de carga es de: {chargeTime}");

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
    void SetAnimation(string name)
    {

        // Obtenemos todos los par�metros del Animator
        AnimatorControllerParameter[] parametros = GetComponent<Animator>().parameters;

        // Recorremos todos los par�metros y los ponemos a false
        foreach (var item in parametros) GetComponent<Animator>().SetBool(item.name, false);

        // Activamos el pasado por par�metro
        GetComponent<Animator>().SetBool(name, true);

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.collider.CompareTag("Plat") && (raycast.collider == null && raycast2.collider == null)) {



                if (rb.transform.position.x < collision.gameObject.transform.position.x) { rb.velocity = new Vector2(-playerSpeed + 2, rb.velocity.y); }
                else if ((rb.transform.position.x > collision.gameObject.transform.position.x)){ rb.velocity = new Vector2(playerSpeed - 2, rb.velocity.y); }
                else if(rb.velocity.x == 0) rb.velocity = new Vector2(0, rb.velocity.y);
                else rb.velocity = new Vector2(0, rb.velocity.y);

                /* if(rb.velocity.x >0) rb.velocity = new Vector2(-playerSpeed+2, rb.velocity.y);
                 else if (rb.velocity.x < 0) rb.velocity = new Vector2(playerSpeed - 2, rb.velocity.y);
                 
                */
            } 

        }
    }
}
