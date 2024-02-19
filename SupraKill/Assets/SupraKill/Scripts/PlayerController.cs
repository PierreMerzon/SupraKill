using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D playerRb;
    Animator anim;
    [SerializeField] float horizontalInput;

    [Header("Player Attributes")]
    public float playerSpeed;
    public float jumpForce;

    public float gravityX;
    public float gravityY;

    [Header("Player Direction")]
    [SerializeField] bool isFacingRight = true;

    [Header("Attack")]
    [SerializeField] bool isAttacking;
    [SerializeField] bool AttackingRight;

    [Header("Ground Check")]
    [SerializeField] float groundedAreaLength;
    [SerializeField] float groundedAreaHeight;
    [SerializeField] bool isGrounded;
    [SerializeField] GameObject groundCheck;
    [SerializeField] LayerMask groundLayer;

    private void Awake()
    {
        groundCheck = GameObject.Find("GroundCheck");
        playerRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        Physics2D.gravity = new Vector2(gravityX, gravityY);
    }

    void Update()
    {
        PlayerDirection();
        Movement();
        Jump();
        Atk();
        ConstantKoboldAnim();
        TriggerAnimations();

        ConsoleInputLogs();

        if (!isAttacking & Input.GetKeyDown(KeyCode.F))
            anim.SetTrigger("hit");

        if (!isAttacking & Input.GetKeyDown(KeyCode.G))
            anim.SetTrigger("death");

    }

    void ConsoleInputLogs()
    {
        if (Input.GetKey(KeyCode.F)) { Debug.Log("Tecla F"); }
        if (Input.GetKey(KeyCode.G)) { Debug.Log("Tecla G"); }
        if (Input.GetKey(KeyCode.A)) { Debug.Log("Tecla A"); }
        if (Input.GetKey(KeyCode.D)) { Debug.Log("Tecla D"); }
        if (Input.GetKey(KeyCode.LeftArrow)) { Debug.Log("Flecha izquierda"); }
        if (Input.GetKey(KeyCode.RightArrow)) { Debug.Log("Tecla derecha"); }
        if (Input.GetKey(KeyCode.Space)) { Debug.Log("Barra espaciadora"); }
    }

    void Movement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        playerRb.velocity = new Vector2(horizontalInput * playerSpeed, playerRb.velocity.y);
    }

    void Jump()
    {
        //GROUNDCHECK
        isGrounded = Physics2D.OverlapArea(
                        new Vector2(groundCheck.transform.position.x - (groundedAreaLength / 2),
                                    groundCheck.transform.position.y - groundedAreaHeight),
                        new Vector2(groundCheck.transform.position.x + (groundedAreaLength / 2),
                                    groundCheck.transform.position.y + 0.01f),
                                    groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void Atk()
    {
        if (!isAttacking & Input.GetKeyDown(KeyCode.RightArrow))
        {
            AttackingRight = true;
            anim.SetTrigger("atk");
        }

        if (!isAttacking & Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AttackingRight = false;
            anim.SetTrigger("atk");
        }
    }

    void PlayerDirection()
    {
        //Left Direction
        if (isAttacking && !AttackingRight && isFacingRight) { Flip(); }
        else if (!isAttacking && horizontalInput < 0 && isFacingRight) { Flip(); }
        //Right Direction
        if (isAttacking && AttackingRight && !isFacingRight) { Flip(); }
        else if (!isAttacking && horizontalInput > 0 && !isFacingRight) { Flip(); }
    }

    void Flip()
    {
        Vector2 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }

    void ConstantKoboldAnim()
    {
        //RUN ANIMATION
        if (System.Math.Abs(playerRb.velocity.x) > 1)
        { anim.SetBool("run", true); }
        else { anim.SetBool("run", false); }

        //MIDAIR ANIMATION
        if (!isGrounded)
        { anim.SetBool("midAir", true); }
        else { anim.SetBool("midAir", false); }
    }

    void TriggerAnimations()
    {

    }



    void IsAttackingEvent()
    { isAttacking = true; }

    void IsNotAttackingEvent()
    { isAttacking = false; }

    //Coyote Time & Jump Buffering 

    private float horizontal;
        private float speed = 8f;
        private float jumpingPower = 16f;
        private bool isFacingRight = true;

        private bool isJumping;

        private float coyoteTime = 0.2f;
        private float coyoteTimeCounter;

        private float jumpBufferTime = 0.2f;
        private float jumpBufferCounter;

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;

        private void Update()
        {
            horizontal = Input.GetAxisRaw("Horizontal");

            if (IsGrounded())
            {
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }

            if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

                jumpBufferCounter = 0f;

                StartCoroutine(JumpCooldown());
            }

            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

                coyoteTimeCounter = 0f;
            }

            Flip();
        }

        private void FixedUpdate()
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        }

        private void Flip()
        {
            if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
            {
                Vector3 localScale = transform.localScale;
                isFacingRight = !isFacingRight;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }

        private IEnumerator JumpCooldown()
        {
            isJumping = true;
            yield return new WaitForSeconds(0.4f);
            isJumping = false;
        }
    }