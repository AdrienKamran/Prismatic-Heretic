using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Movement2D : MonoBehaviour
{

    public float speed;

    Vector3 direction;

    public Sprite up;
    public Sprite right;
    public Sprite left;
    public Sprite down;
    public Sprite norm;

    
    public Animator animator;
    private Vector2 moveDir;
    private Vector2 lastMoveDir;
    Rigidbody2D body;


    public bool isDashing;
    public float dashSpeed;
    public float dashTime;
    public float startDashTime;
    public Vector2 dashDirection;

    public AudioSource walking;
    public AudioSource source;
    public AudioClip dash;
    private void Start()
    {
        isDashing = false;
        body = GetComponentInParent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
        dashTime = startDashTime;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (!Player.GameIsPaused)
        {
            if(Player.inDialog == false)
            {
                direction.x = Input.GetAxis("Horizontal");
                direction.y = Input.GetAxis("Vertical");
            }
            else
            {
                direction.x = 0;
                direction.y = 0;
            }

            if (moveDir.x != 0 || moveDir.y != 0)
            {
                lastMoveDir = moveDir;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
            {
                if (direction.x != 0 || direction.y != 0)
                {
                    if (Player.currentStamina >= 20)
                    {

                        isDashing = true;
                        source.PlayOneShot(dash);
                        dashDirection = direction.normalized;
                        dashTime = startDashTime;
                        animator.SetFloat("DashHorizontal", dashDirection.x);
                        animator.SetFloat("DashVertical", dashDirection.y);
                        animator.SetBool("isDashing", isDashing);
                        this.GetComponent<Player>().TakeStamina(20);
                    }
                }
            }

            if (dashTime <= 0)
            {
                dashDirection = new Vector2(0, 0);
                body.velocity = new Vector2(0, 0);
                isDashing = false;
                animator.SetBool("isDashing", isDashing);
            }
            else
            {
                if (isDashing)
                {
                    dashTime -= Time.deltaTime;
                    body.velocity = dashDirection * dashSpeed;
                }
            }
            
        }
           
        

    }

    private void FixedUpdate()
    {
        if (direction.x != 0 || direction.y != 0)
        {
            if (!walking.isPlaying)
            {
                walking.Play(0);
            }
        }
        else
        {
            walking.Stop();
        }
            moveDir = new Vector2(direction.x, direction.y).normalized;

        animator.SetFloat("LastHorizontal", lastMoveDir.x);
        animator.SetFloat("LastVertical", lastMoveDir.y);

        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Speed", direction.sqrMagnitude);
        

        //Avoid using transform.position. 
       if (!isDashing)
        {
            body.velocity = new Vector3(direction.x, direction.y) * speed;

        }
       
    }
}
