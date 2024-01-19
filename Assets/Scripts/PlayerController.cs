using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed = 4f;
    [SerializeField] float minJumpForce = 5f;
    [SerializeField] float maxJumpForce = 20f;
    [SerializeField] float wallBounceForce = 5f;
    [SerializeField] float chargeSpeed = 5f;
    [SerializeField] float maxChargeTime = 2f;

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

    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentJumpForce = minJumpForce;
        maxChargeTime = 0f;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

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


        

        ChargeJump();


        if(!isJumping)
        {
            if (horizontalInput > 0) GetComponent<SpriteRenderer>().flipX = false;
            else if (horizontalInput < 0) GetComponent<SpriteRenderer>().flipX = true;
        }

        if (raycast1.collider == null && raycast2.collider == null)
        {
            rb.sharedMaterial.bounciness = 5f;
            rb.sharedMaterial.friction = 0;
            Debug.Log("Con rebote");
            //Debug.Log($"Tiempo de salto restante: {ctt}");

            if (rb.velocity.y > 0)
            {
                isJumping = true;
                SetAnimation("Jump");
            }
            else if (!isJumping && (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)))
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
                Debug.Log("Sin rebote");

                if (!isJumping) rb.velocity = new Vector2(playerSpeed * horizontalInput, rb.velocity.y);
                else rb.velocity = new Vector2(0, rb.velocity.y);
                isJumping = false;
        }

        ApplyCustomGravity();

        if (!(Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space)) && chargeTime > 0)
        {
            if(chargeTime <0.35f) { chargeTime = 0.35f; Debug.Log("Nv. de salto 1: x0.35"); }            
            
            Jump();
        
        }
    }
    void Jump()
    {
        // Establecer la velocidad vertical instant�neamente al valor de salto
        if(Input.GetAxisRaw("Horizontal")!=0) rb.AddForce(new Vector2(Input.GetAxisRaw("Horizontal")*(playerSpeed/1.5f), minJumpForce*chargeTime), ForceMode2D.Impulse);
        else rb.velocity = new Vector2(0, minJumpForce * chargeTime);
        isJumping = true;
        chargeTime = 0;
        Debug.Log("chargeTime reseteado a 0");
        isJumping = true;
    }

    void ChargeJump() {
        if ((Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space)) && !isJumping)
        {
            isJumping = false;
            chargeTime = chargeTime + (Time.deltaTime*2);
            if (chargeTime >= 1.25f) { chargeTime = 1.25f;}
            Debug.Log($"El tiempo de carga es de: {chargeTime}");
            SetAnimation("ChargingJump");
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
        AnimatorControllerParameter[] parametros = GetComponent<Animator>().parameters;
        foreach (var item in parametros) GetComponent<Animator>().SetBool(item.name, false);
        GetComponent<Animator>().SetBool(name, true);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.collider.CompareTag("Plat")) {


                if ((collision.collider.transform.position.x < rb.transform.position.x) && rb.velocity.x >0){
                    //rb.velocity = new Vector2(-playerSpeed/2 + 2, rb.velocity.y);
                    rb.AddForce(new Vector2( (playerSpeed / 2f) + 2, rb.velocity.y), ForceMode2D.Impulse);
                }
                else if ((collision.collider.transform.position.x > rb.transform.position.x) && rb.velocity.x < 0) { 
                    //rb.velocity = new Vector2(playerSpeed / 2 + 2, rb.velocity.y);
                    rb.AddForce(new Vector2((-1 * playerSpeed / 2f) + 2, rb.velocity.y), ForceMode2D.Impulse);
                }
                else rb.velocity = new Vector2(0, rb.velocity.y);

                /*
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
