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
    public float dashingPower = 30f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;

    [Header("Checks")]
    private bool right;
    private bool left;
    //Controle do jogador respectivamente 'Movimento' 'Pulo' 'Permitido' 'Dash' 'Can Dash'
#pragma warning disable IDE0044 // Add readonly modifier
    private char[] control = {'M', 'F', 'O', 'S', 'T'};
#pragma warning restore IDE0044 // Add readonly modifier

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
        if (control[3] == 'D')
        {
            return;
        }
        Move();
        DashStart();
        Jump();
    }

    void Move()
    {
        if (control[2] == 'O')
        {
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                control[0] = 'B';
            }

            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                control[0] = 'F';
            }

            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A) || !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                control[0] = 'M';
            }

            if (control[0] == 'M')
            {
                movement = 0;
            }

            if (control[0] == 'B')
            {
                movement = -1;
            }

            if (control[0] == 'F')
            {
                movement = 1;
            }

            rig.velocity = new Vector2(movement * speed, rig.velocity.y);

            if (movement > 0)
            {
                if (control[1] == 'F')
                {
                    anim.SetInteger("transition", 2);
                }
                transform.eulerAngles = new Vector3(0, 0, 0);
                right = true;
                left = false;
            }

            if (movement < 0)
            {
                if (control[1] == 'F')
                {
                    anim.SetInteger("transition", 2);
                }
                transform.eulerAngles = new Vector3(0, 180, 0);
                left = true;
                right = false;
            }

            if (movement == 0 && control[1] == 'F' && control[3] == 'S')
            {
                anim.SetInteger("transition", 1);
            }
        }
    }

    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (control[1] == 'F')
            {
                anim.SetInteger("transition", 3);
                rig.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                control[1] = 'J';
            }
        }
    }

    void DashStart()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && control[4] == 'T')
        {
            //anim.SetInteger("Transition", x);
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        if (control[3] == 'S')
        {
            control[4] = 'F';
            control[3] = 'D';
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
            control[3] = 'S';
            yield return new WaitForSeconds(dashingCooldown);
            control[4] = 'T';
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3)
        {
            control[1] = 'F';
        }

        if(collision.gameObject.layer == 9)
        {
            //GameController.instance.GameOver();
        }
    }
}
