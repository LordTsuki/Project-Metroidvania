using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Status")]
    public int health;
    public int damage;
    public float speed;
    public float jumpForce;
    private float movement;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    [Header("Checks")]
    private bool isJumping;
    private bool isDashing;
    private bool canDash = true;
    private bool right;
    private bool left;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rig;
    [SerializeField] private TrailRenderer tr;
    private Animator anim;
    public GameObject _cam;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(isDashing)
        {
            return;
        }
        Move();
        DashStart();
        Jump();
    }

    void Move()
    {
        movement = Input.GetAxis("Horizontal");
        rig.velocity = new Vector2(movement * speed, rig.velocity.y);

        if(movement > 0)
        {
            if(!isJumping)
            {
                anim.SetInteger("transition", 2);
            }
            transform.eulerAngles = new Vector3(0, 0, 0);
            right = true;
            left = false;
        }
        if(movement < 0)
        {
            if (!isJumping)
            {
                anim.SetInteger("transition", 2);
            }
            transform.eulerAngles = new Vector3(0, 180, 0);
            left = true;
            right = false;
        }
        if (movement == 0 && !isJumping && !isDashing)
        {
            anim.SetInteger("transition", 1);
        }
    }

    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (!isJumping)
            {
                anim.SetInteger("transition", 3);
                rig.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                isJumping = true;
            }
        }
    }

    void DashStart()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            //anim.SetInteger("Transition", x);
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        if (!isDashing)
        {
            canDash = false;
            isDashing = true;
            float originalGravity = rig.gravityScale;
            rig.gravityScale = 0f;
            if(right && !left)
            {
                rig.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
                tr.emitting = true;
            }
            if(left && !right)
            {
                rig.velocity = new Vector2(transform.localScale.x * -dashingPower, 0f);
                tr.emitting = true;
            }
            yield return new WaitForSeconds(dashingTime);
            tr.emitting = false;
            rig.gravityScale = originalGravity;
            isDashing = false;
            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3)
        {
            if(isJumping)
            {
                anim.SetInteger("transition", 1);
            }
            isJumping = false;
        }

        if(collision.gameObject.layer == 9)
        {
            //GameController.instance.GameOver();
        }
    }
}
