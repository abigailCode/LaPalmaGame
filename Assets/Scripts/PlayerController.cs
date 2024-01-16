using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int playerSpeed;
    [SerializeField] int jumpForce;
    [SerializeField] float lineLength = 1f;
    [SerializeField] float offset = 1f;
    [SerializeField] ParticleSystem particles;
    private float CoyoteTime = 0.8f;
    private float ctt;

    [SerializeField] bool isJumping = false;

    void Start()
    {
        ctt = CoyoteTime;
    }

    void Update()
    {

        GetComponent<Rigidbody2D>().velocity = new Vector2(playerSpeed * Input.GetAxisRaw("Horizontal"), GetComponent<Rigidbody2D>().velocity.y);
        if (Input.GetAxisRaw("Horizontal") == 1) GetComponent<SpriteRenderer>().flipX = true;
        if (Input.GetAxisRaw("Horizontal") == -1) GetComponent<SpriteRenderer>().flipX = false;


        /*
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && !isJumping)
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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

            if (ctt <= 0 || GetComponent<Rigidbody2D>().velocity.y > 0)
            {
               
                isJumping = true;
                // SetAnimation("Jump");
            }
            else if (ctt > 0 && (!isJumping && (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))))
            {

                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpForce);
                //GetComponent<Rigidbody2D>().AddForce(Vector2.up*jumpForce, ForceMode2D.Impulse);
                isJumping = true;

            }

            
            
               
               
            
        }

        else {

            ctt = CoyoteTime;
            Debug.Log($"Tiempo de salto restante: {ctt}");
            isJumping = false;

            if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) {

                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpForce);
               

            }

            if ((Input.GetButtonDown("Fire1") || Input.GetKey(KeyCode.Space)) && GetComponent<Rigidbody2D>().velocity.y<0)
            {

                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpForce+2);


            }



        }

        

        // ------------------------------------------------------------------------------------------
        // APLICACIÓN AL JUEGO DE PLATAFORMAS QUE UTILIZA RAYCAST PARA DETECTAR QUE ESTÁ EN EL SUELO
        // ------------------------------------------------------------------------------------------
        // Si el raycast no toca con nada el personaje está en el aire
        if (raycast.collider == null)
        {
           
        }
        else
        {
            // Si está sobre una superficie pero se mueve lateralmente
           // if (GetComponent<Rigidbody2D>().velocity.x != 0) SetAnimation("Walk");
            //else SetAnimation("Idle"); // Si está sobre una superficie pero no se mueve
        }

    }

    void SetAnimation(string name)
    {

        // Obtenemos todos los parámetros del Animator
        AnimatorControllerParameter[] parametros = GetComponent<Animator>().parameters;

        // Recorremos todos los parámetros y los ponemos a false
        foreach (var item in parametros) GetComponent<Animator>().SetBool(item.name, false);

        // Activamos el pasado por parámetro
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
