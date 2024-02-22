using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float range;
    [SerializeField] private int atkDmg;
    [SerializeField] private bool playerInRange;
    [SerializeField] bool isAttacking;

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;


    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Components")]
    [SerializeField] private Behaviour[] components;
    private bool invulnerable;


    [SerializeField] Vector2 min;
    [SerializeField] Vector2 max;

    //References
    private Animator anim;
    private MeleeEnemy playerHealth;
    private EnemyPatrol enemyPatrol;
    [SerializeField] PlayerController pController;

    private void Awake()
    {
        pController = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();

        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {

        min = boxCollider.bounds.min;
        max = boxCollider.bounds.max;
        if (Physics2D.OverlapArea(new Vector2(transform.position.x - .5f, boxCollider.bounds.min.y), new Vector2(transform.position.x + range, boxCollider.bounds.max.y), playerLayer))
        { playerInRange = true; }
        else { playerInRange = false; }

        if (playerInRange && !isAttacking)
        {
            anim.SetTrigger("attack");
        }

        //if (enemyPatrol != null)
            //enemyPatrol.enabled = !PlayerInSight();
    }

    void Attack()
    {
        Debug.Log("attack done");
        if (playerInRange)
        { 
            pController.TakeDamage(atkDmg);
        }
        
    }

    public void TakeDamage(float _damage)
    {
        if (invulnerable) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            StartCoroutine(Invunerability());
        }
        else
        {
            if (!dead)
            {
                anim.SetTrigger("death");

                //Deactivate all attached component classes
                foreach (Behaviour component in components)
                    component.enabled = false;

                dead = true;
            }
        }
    }
    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }
    private IEnumerator Invunerability()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }

    void isAttackingTrue()
    {
        isAttacking = true;
    }
    void isAttackingFalse()
    {
        isAttacking = false;
    }
}
