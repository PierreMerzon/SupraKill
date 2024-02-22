using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private Animator anim;
    [SerializeField] float horizontalInput;
    [SerializeField] private AudioSource jumpSoundEffect;
    [SerializeField] private AudioSource deathSoundEffect;

    [Header("Player Attributes")]
    [SerializeField] float velocityY;
    [SerializeField] float playerBaseSpeed;
    public float playerSpeed;
 
    public float jumpForce;

    public float gravityX;
    public float gravityY;

    public float maxHealth = 100;
    public float currentHealth;


    [Header("Player Direction")]
    [SerializeField] bool isFacingRight = true;

    [Header("Ground Check")]
    [SerializeField] float groundedAreaLength;
    [SerializeField] float groundedAreaHeight;
    [SerializeField] bool isGrounded;
    [SerializeField] GameObject groundCheck;
    [SerializeField] LayerMask groundLayer;

    [Header("Attack")]
    [SerializeField] bool isAttacking;
    [SerializeField] bool AttackingRight;

    [SerializeField] private Transform atkController;
    [SerializeField] private float atkRadius;
    [SerializeField] private float atkDmg;
    [SerializeField] private float atkCD;
    [SerializeField] private float timeUntilAttack;

    [SerializeField] private AudioSource attackSoundEffect;

    [Header("Mouse Position")]
    [SerializeField] Vector3 mousePos;

    private void Awake()
    {
        groundCheck = GameObject.Find("GroundCheck");
        playerRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        Physics2D.gravity = new Vector2(gravityX, gravityY);
    }

    //TRAMPA

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            Death();
        }
    }

    void Update()
    {
        PlayerAttributes();
        PlayerDirection();
        Movement();
        //TakeDamage();
        Jump();

        Vector3 screenPosition = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(screenPosition);
        mousePos.z = 0;

        if (isAttacking)
        {
            playerSpeed = .3f;
        }
        else
        {
            playerSpeed = 1;
            if (Input.GetButtonDown("Fire1"))
            {
                anim.SetTrigger("attack");
                if (mousePos.x > groundCheck.transform.position.x)
                {
                    AttackingRight = true;
                    Atk();
                }
                else if (mousePos.x < groundCheck.transform.position.x)
                {
                    AttackingRight = false;
                    Atk();
                }
            }
        }

        ConstantSaiAnim();
        TriggerAnimations();

    }

    private void Atk()
    {
        attackSoundEffect.Play();
        Collider2D[] objects = Physics2D.OverlapCircleAll(atkController.position, atkRadius);

        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Enemigo"))
            {
                collider.transform.GetComponent<Health>().TakeDamage(atkDmg);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(atkController.position, atkRadius);

    }

    void Movement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        playerRb.velocity = new Vector2(horizontalInput * playerSpeed * playerBaseSpeed, playerRb.velocity.y);
    }

    void Jump()
    {
        if (velocityY < 0.5)
        {
            playerRb.gravityScale = 2.5f;
        }
        else
        {
            playerRb.gravityScale = 1;
        }
        //GROUNDCHECK
        isGrounded = Physics2D.OverlapArea(
                        new Vector2(groundCheck.transform.position.x - (groundedAreaLength / 2),
                                    groundCheck.transform.position.y - groundedAreaHeight),
                        new Vector2(groundCheck.transform.position.x + (groundedAreaLength / 2),
                                    groundCheck.transform.position.y + 0.01f),
                                    groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpSoundEffect.Play();
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }

    }

    void PlayerAttributes()
    {
        velocityY = playerRb.velocity.y;
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

    void ConstantSaiAnim()
    {
        //run ANIMATION
        if (System.Math.Abs(playerRb.velocity.x) > 1)
        { anim.SetBool("run", true); }
        else { anim.SetBool("run", false); }

        //MIDAIR ANIMATION
        if (!isGrounded)
        { anim.SetBool("jump", true); }
        else { anim.SetBool("jump", false); }
    }

    void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0) 
        {
            Death();
        }
    }

    private void Death()
    {
        deathSoundEffect.Play();
        playerRb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
        //Then show death screen
    }

    //RESTART

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void TriggerAnimations()
    {

    }

    void IsAttackingEvent()
    { isAttacking = true; }

    void IsNotAttackingEvent()
    { isAttacking = false; }

}

    