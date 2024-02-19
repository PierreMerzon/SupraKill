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

    //Buffer jump inputs

    public class Fall : BaseState
    {
        [Export] public float jump_buffer_time = 0.1f;
        [Export] public float move_speed = 60f;
        [Export] public NodePath run_node;
        [Export] public NodePath idle_node;
        [Export] public NodePath jump_node;

        private BaseState run_state;
        private BaseState idle_state;
        private BaseState jump_state;
        private float jump_buffer_timer = 0f;

        public override void _Ready()
        {
            run_state = GetNode<BaseState>(run_node);
            idle_state = GetNode<BaseState>(idle_node);
            jump_state = GetNode<BaseState>(jump_node);
        }

        public override void _EnterTree()
        {
            base._EnterTree();
            jump_buffer_timer = 0f;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            if (@event.IsActionPressed("jump"))
            {
                jump_buffer_timer = jump_buffer_time;
            }
        }

        public override BaseState _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            jump_buffer_timer -= delta;

            int move = 0;
            if (Input.IsActionPressed("move_left"))
            {
                move = -1;
                // player.animations.flip_h = true;
            }
            else if (Input.IsActionPressed("move_right"))
            {
                move = 1;
                // player.animations.flip_h = false;
            }

            // player.velocity.x = move * move_speed;
            // player.velocity.y += player.gravity;
            // player.velocity = player.MoveAndSlide(player.velocity, Vector2.Up);

            // if (player.IsOnFloor())
            // {
            //     if (jump_buffer_timer > 0)
            //     {
            //         return jump_state;
            //     }
            //     if (move != 0)
            //     {
            //         return run_state;
            //     }
            //     else
            //     {
            //         return idle_state;
            //     }
            // }
            return null;
        }

        //Add coyote time

        public class Fall : BaseState
        {
            [Export] public float coyote_time = 0.2f;
            [Export] public float move_speed = 60f;
            [Export] public NodePath run_node;
            [Export] public NodePath idle_node;
            [Export] public NodePath jump_node;

            private BaseState run_state;
            private BaseState idle_state;
            private BaseState jump_state;
            private float coyote_timer = 0f;

            public override void _Ready()
            {
                run_state = GetNode<BaseState>(run_node);
                idle_state = GetNode<BaseState>(idle_node);
                jump_state = GetNode<BaseState>(jump_node);
            }

            public override void _EnterTree()
            {
                base._EnterTree();
                coyote_timer = coyote_time;
            }

            public override BaseState _Input(InputEvent @event)
            {
                base._Input(@event);
                if (@event.IsActionPressed("jump"))
                {
                    if (coyote_timer > 0)
                    {
                        return jump_state;
                    }
                }
                return null;
            }

            public override BaseState _PhysicsProcess(float delta)
            {
                base._PhysicsProcess(delta);
                coyote_timer -= delta;

                int move = 0;
                if (Input.IsActionPressed("move_left"))
                {
                    move = -1;
                    // player.animations.flip_h = true;
                }
                else if (Input.IsActionPressed("move_right"))
                {
                    move = 1;
                    // player.animations.flip_h = false;
                }

                // player.velocity.x = move * move_speed;
                // player.velocity.y += player.gravity;
                // player.velocity = player.MoveAndSlide(player.velocity, Vector2.Up);

                // if (player.IsOnFloor())
                // {
                //     if (move != 0)
                //     {
                //         return run_state;
                //     }
                //     else
                //     {
                //         return idle_state;
                //     }
                // }
                return null;
            }

            //Push off ledges

            public class Jump : BaseState
            {
                [Export] public float jump_force = 100f;
                [Export] public float move_speed = 60f;
                [Export] public NodePath fall_node;
                [Export] public NodePath run_node;
                [Export] public NodePath idle_node;

                private BaseState fall_state;
                private BaseState run_state;
                private BaseState idle_state;

                public override void _Ready()
                {
                    fall_state = GetNode<BaseState>(fall_node);
                    run_state = GetNode<BaseState>(run_node);
                    idle_state = GetNode<BaseState>(idle_node);
                }

                public override void _EnterTree()
                {
                    base._EnterTree();
                    player.velocity.y = -jump_force;
                }

                public override BaseState _PhysicsProcess(float delta)
                {
                    base._PhysicsProcess(delta);

                    int move = 0;
                    if (Input.IsActionPressed("move_left"))
                    {
                        move = -1;
                        // player.animations.flip_h = true;
                    }
                    else if (Input.IsActionPressed("move_right"))
                    {
                        move = 1;
                        // player.animations.flip_h = false;
                    }

                    // Adjust player position based on collision
                    if (player.right_outer.IsColliding() && !player.right_inner.IsColliding() &&
                        !player.left_inner.IsColliding() && !player.left_outer.IsColliding())
                    {
                        player.GlobalPosition -= new Vector2(5, 0);
                    }
                    else if (player.left_outer.IsColliding() && !player.left_inner.IsColliding() &&
                             !player.right_inner.IsColliding() && !player.right_outer.IsColliding())
                    {
                        player.GlobalPosition += new Vector2(5, 0);
                    }

                    player.velocity.x = move * move_speed;
                    player.velocity.y += player.gravity;
                    player.velocity = player.MoveAndSlide(player.velocity, Vector2.Up);

                    if (player.velocity.y > 0)
                    {
                        return fall_state;
                    }

                    if (player.IsOnFloor())
                    {
                        if (move != 0)
                        {
                            return run_state;
                        }
                        return idle_state;
                    }

                    return null;
                }
            }
