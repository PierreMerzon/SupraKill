using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prueba : MonoBehaviour
{
    //Variables de referencia
    private Rigidbody2D playerRb;
    private Animator anim;
    private float horizontalInput;

    //Variables de estadísticas del player
    public float speed;
    public float jumpForce;
    private bool isFacingRight = true;
    [SerializeField] bool isGrounded;
    [SerializeField] GameObject groundCheck; //Estas últimas tres las necesito para el groundCheck
    [SerializeField] LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        playerRb =  GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();             //Estas para referenciarlas una vez las he creado como variables
    }


    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, groundLayer);
        Movement();
        Jump();
    }

    void Movement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        playerRb.velocity = new Vector2(horizontalInput * speed, playerRb.velocity.y);

        //Flip: si el valor de input es diferente a 0
        if (horizontalInput > 0)
        {
            anim.SetBool("run", true);
            if (!isFacingRight) 
            { 
            Flip();
            }
        }
        if (horizontalInput < 0)
        {
            anim.SetBool("run", true);
            if (isFacingRight)
            {
                Flip();
            }
        }
        if (horizontalInput == 0) 
        {
            anim.SetBool("run", false);
        }
    }

    void Jump()
    {
        anim.SetBool ("jump", !isGrounded);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) 
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void Flip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }
}
