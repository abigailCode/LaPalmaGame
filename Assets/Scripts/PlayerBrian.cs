using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrian : MonoBehaviour
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
    private bool canMove = true;
    private bool isJumping = false;

    private Rigidbody2D rb;

    Vector2 origin;
    Vector2 target;

    Vector2 origin2;
    Vector2 target2;

    RaycastHit2D raycast;
    RaycastHit2D raycast2;

    void Start()
    {
        ctt = CoyoteTime;
        rb = GetComponent<Rigidbody2D>();
        canMove = true;
    }

    void Update()
    {
        HandleMovementInput();

        Debug.DrawLine(origin, target, Color.red);
        Debug.DrawLine(origin2, target2, Color.red);

        CheckGround();

        ApplyCustomGravity();

        if (!(Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space)) && chargeTime > 0)
        {
            Jump();
        }
    }

    void HandleMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (!isJumping)
        {
            if (horizontalInput > 0) GetComponent<SpriteRenderer>().flipX = false;
            if (horizontalInput < 0) GetComponent<SpriteRenderer>().flipX = true;
        }

        if (canMove)
        {
            transform.Translate(Vector2.right * horizontalInput * playerSpeed * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (Input.GetAxisRaw("Horizontal") != 0)
            rb.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * (playerSpeed / 1.5f), jumpForce * chargeTime), ForceMode2D.Impulse);
        else
            rb.velocity = new Vector2(0, jumpForce * chargeTime);

        isJumping = true;
        chargeTime = 0;
        Debug.Log("chargeTime reseteado a 0");
        canMove = true;
    }

    void ChargeJump()
    {
        if ((Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space)) && !isJumping)
        {
            canMove = false;
            chargeTime = chargeTime + (Time.deltaTime * 2);
            if (chargeTime >= 1.25f) { chargeTime = 1.25f; }
            Debug.Log($"El tiempo de carga es de: {chargeTime}");
        }
    }

    void CheckGround()
    {
        origin = new Vector2(transform.position.x - 0.55f, transform.position.y - offset);
        target = new Vector2(transform.position.x - 0.55f, transform.position.y - offset - lineLength);

        origin2 = new Vector2(rb.transform.localScale.x * transform.position.x + 0.45f, transform.position.y - offset);
        target2 = new Vector2(rb.transform.localScale.x * transform.position.x + 0.45f, transform.position.y - offset - lineLength);

        raycast = Physics2D.Raycast(origin, Vector2.down, lineLength);
        raycast2 = Physics2D.Raycast(origin2, Vector2.down, lineLength);

        if ((raycast.collider == null && raycast2.collider == null))
        {
            // Lógica cuando el jugador está en el aire
            rb.sharedMaterial.bounciness = 5f;
            rb.sharedMaterial.friction = 0;
            Debug.Log("Con rebote");
            ctt = ctt - Time.deltaTime;

            if (ctt <= 0 || rb.velocity.y > 0)
            {
                isJumping = true;
            }
            else if (ctt > 0 && (!isJumping && (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))))
            {
                Jump();
                isJumping = true;
            }
        }
        else
        {
            // Lógica cuando el jugador está en el suelo
            rb.sharedMaterial.bounciness = 0;
            rb.sharedMaterial.friction = 10;
            Debug.Log("Sin rebote");

            if (canMove)
            {
                transform.Translate(Vector2.right * Input.GetAxis("Horizontal") * playerSpeed * Time.deltaTime);
            }

            ctt = CoyoteTime;
            isJumping = false;
        }
    }

    void ApplyCustomGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
