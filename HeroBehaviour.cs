using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iJump
{
    void Jump(HeroBehaviour hero);
}

public class NormalJump : iJump
{
    public void Jump(HeroBehaviour hero)
    {
        if (hero.IsGrounded())
        {
            hero.rb.velocity = new Vector2(hero.rb.velocity.x, hero.jump);
            hero.animator.SetTrigger("Jump");
        }
    }
}

public class DobleJump : iJump
{
    public void Jump(HeroBehaviour hero)
    {
        if (hero.jumpsLeft > 0)
        {
            hero.rb.velocity = new Vector2(hero.rb.velocity.x, hero.jump);
            hero.animator.SetTrigger("Jump");
            hero.jumpsLeft--;
        }
    }
}

public class HeroBehaviour : MonoBehaviour
{
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;

    public Animator animator;
    private SpriteRenderer spriteRenderer;
    public float nextAttack = 0.2f;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public float attackRange = 0.5f;
    public Rigidbody2D rb;
    public Vector2 movement;
    public float jump = 16f;
    public float speed = 8f;
    public float inputX;
    public bool facingRight = true;
    // Doble Jump
    private iJump jumpSys = new NormalJump();
    public int maxJumps = 1;
    public int jumpsLeft = 0;
    //Dash
    private float horizontal = 0;
    private bool canDash = true;
    private bool isDashing = false;
    public float dashForce = 24;
    public float dashTime = 0.2f;
    public float dashCooldown = 1;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        //jumpsLeft = maxJumps;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }

        animator.SetFloat("AirSpeedY", rb.velocity.y);
        animator.SetBool("Grounded", IsGrounded());

        horizontal = Input.GetAxis("Horizontal");
        inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        if (IsGrounded())
        {
            jumpsLeft = maxJumps;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpSys.Jump(this);
            //rb.velocity = new Vector2(rb.velocity.x, jump);
            //animator.SetTrigger("Jump");
        }

        if (Input.GetButtonDown("Fire1") && Time.time > nextAttack)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
            {
                animator.SetTrigger("Attack3");
                nextAttack = Time.time + 0.5f;
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
            {
                animator.SetTrigger("Attack2");
                nextAttack = Time.time + 0.2f;
            }
            else
            {
                animator.SetTrigger("Attack1");
                nextAttack = Time.time + 0.2f;
            }
            Attack();
        }

        if (Input.GetButtonDown("Fire2") && canDash)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            animator.SetInteger("AnimState", 1);
        }

        FlipCharacter();
        //Vector3 movement = new Vector3(speed.x * inputX, 0, 0);

        if (Input.GetAxis("Horizontal") == 0)
        {
            animator.SetInteger("AnimState", 0);
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void FlipCharacter()
    {
        if (facingRight && inputX < 0f || !facingRight && inputX > 0f)
        {
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        //anim.SetBool("isDashing", true);
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(dashForce * horizontal, 0);
        tr.emitting = true;

        yield return new WaitForSeconds(dashTime);

        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        //anim.SetBool("isDashing", false);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        rb.velocity = new Vector2(inputX * speed, rb.velocity.y);
    }

    public void Attack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in enemiesHit)
        {
            Debug.Log("enemy hit boi");
            GameObject enemyObject = enemy.gameObject;
            BringerOfDeath bringerOfDeath = enemy.gameObject.GetComponent<BringerOfDeath>();
            if(enemyObject.tag=="BringerOfDeath")
            {
                if (bringerOfDeath.alive == true)
                {
                    Animator animator = enemy.GetComponent<Animator>();
                    animator.SetTrigger("Hurt");
                    bringerOfDeath.health--;
                }
            }
        }
    }

    public void UnlockDobleJump()
    {
        jumpSys = new DobleJump();
    }

    public void LockDobleJump()
    {
        jumpSys = new NormalJump();
    }
}
