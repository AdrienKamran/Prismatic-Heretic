using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarpSword : MonoBehaviour
{
    public GameObject playerFeet;
    public GameObject player;
    public Rigidbody2D player_body;
    public GameObject teleportPoint;

    public SelectSword selectionControl;

    public Rigidbody2D body;
    public RotateAround rotate;
    public BoxCollider2D boxcollider;

    public bool warping = false;
    public bool prepped = false;
    public static bool isBoosted = false;

    public static bool isSwinging = false;
    public GameObject slash;
    public LayerMask enemyLayer;

    public Vector3 initPosition;

    //for slight parabolic effect (not active currently)
    public float acceleration = 5f;
    private float distance;
    public Material purpleMaterial;
    public Material blueMaterial;
    public ParticleSystem markedHit;

    public AudioSource source;
    public AudioClip launch;
    public AudioClip ability;
    public AudioClip prep;
    public AudioClip swing;

    public GameObject SwordPositionControl;

    public void Start()
    {
        isSwinging = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Player.GameIsPaused)
        {
            if (Player.inDialog == false)
            {
                //If blue is being held (not prepped or tucked away)
                if (selectionControl.currentlyHeld == 1)
                {
                    //Throw to Warp
                    if (Input.GetMouseButtonDown(1) == true && warping == false && prepped == false)
                    {
                        warping = true;
                        source.PlayOneShot(launch);
                        selectionControl.blueInUse = true;
                        Warp();
                    }
                    //Prep
                    if (Input.GetKeyDown(KeyCode.Q) && warping == false && prepped == false)
                    {
                        prepped = true;
                        source.PlayOneShot(prep);
                        selectionControl.blueInUse = true;
                        Prep();
                    }

                    if (Input.GetMouseButtonDown(0) == true && warping == false && prepped == false && isSwinging == false)
                    {
                        this.GetComponent<Animator>().Play("Swing Blue");
                    }

                }

                //If blue is prepped (and is being activated)
                if (Input.GetKeyDown(KeyCode.E) == true && prepped == true)
                {
                    if (Player.bluePowerDelay==50)
                    {
                        Activate();
                        Player.bluePowerDelay = 0;
                        prepped = false;
                        SwordPositionControl.transform.parent = null;
                        SceneManager.MoveGameObjectToScene(SwordPositionControl, SceneManager.GetSceneByName("Player"));

                    }
                    
                }

                //If blue is neither being thrown/warping nor prepped, check if it should be active depending on whether it is currently meant to be held.
                if (warping == false && prepped == false)
                {
                    selectionControl.blueInUse = false;
                    if (selectionControl.currentlyHeld == 2 || selectionControl.currentlyHeld == 3)
                        selectionControl.blueSword.GetComponent<SpriteRenderer>().enabled = false;
                   
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (warping == true)
        {
            body.velocity = (new Vector2(body.velocity.x, body.velocity.y));
            float newAngle = Mathf.Atan2(body.velocity.x, body.velocity.y) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.LookRotation
            ((transform.position + (Vector3)body.velocity) - transform.position, transform.TransformDirection(Vector3.up));
            transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
        }
    }

    void Warp() {

        initPosition = this.transform.position;
        float xPos = Mathf.Cos(Mathf.Deg2Rad * rotate.angle);
        float yPos = Mathf.Sin(Mathf.Deg2Rad * rotate.angle);
        body.velocity += ((new Vector2(xPos, yPos)) * 45.0f);

    }
    void Prep()
    {
        body.velocity = new Vector2(0.0f,0.0f);
    }

    void Activate() 
    {
        //DONT TOUCH THIS, SUPER VOLATILE, MONSTER BAD
        source.PlayOneShot(ability);
        playerFeet.transform.position = teleportPoint.transform.position;
        player.transform.position = playerFeet.transform.position;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        //TODO: Modify behaviour depending on whether we have hit an enemy or a surface
        //What currently exists below would apply to enemies being hit (teleport)
        if (warping)
        {
            float distance = Vector3.Distance(initPosition, this.transform.position);
            if (collision.gameObject.layer == 3 || collision.gameObject.layer == 6|| collision.gameObject.layer == 9 || collision.gameObject.layer == 13
                                                                                                                     || collision.gameObject.layer == 10)
            {
                if (collision.gameObject.layer == 13 || collision.gameObject.tag == "MovingBlocker")
                {
                    warping = false;
                    body.velocity = (new Vector2(0.0f, 0.0f));
                    return;
                }
                if (collision.gameObject.layer == 3)
                {
                    if (distance > 5)
                    {
                        collision.gameObject.GetComponent<BaseEnemy>().DealDamage(50);
                        if (collision.gameObject.GetComponent<BaseEnemy>().marked)
                        {

                            Instantiate(markedHit, collision.gameObject.transform.position, Quaternion.identity, null);
                        }
                    }
                   
                   
                }

                if (warping == true)
                {
                    body.velocity = (new Vector2(0.0f, 0.0f));
                    //Debug.Log(teleportPoint.transform.position);

                    //DONT TOUCH THIS, SUPER VOLATILE, MONSTER BAD
                    if (distance > 1)
                    {
                        source.PlayOneShot(ability);
                        playerFeet.transform.position = teleportPoint.transform.position;
                        player.transform.position = playerFeet.transform.position;
                    }
                    warping = false;


                }


            }
        }
       
        
    }

    

    void Slash()
    {
        source.PlayOneShot(swing);
        slash.gameObject.SetActive(true);
    }

    void RemoveSlash()
    {
        slash.gameObject.SetActive(false);
    }

    void StartSwing()
    {
        isSwinging = true;
    }
    void ResetSwing()
    {
        isSwinging = false;
    }

    void attack()
    {

        Collider2D enemy = Physics2D.OverlapCircle(this.transform.position, 2, enemyLayer);

        if (enemy != null)
        {
            
            enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(-player.transform.position.x + enemy.transform.position.x, -player.transform.position.y + enemy.transform.position.y) * 5);
            enemy.GetComponent<BaseEnemy>().hit();
            enemy.GetComponent<BaseEnemy>().DealDamage(15);
            if (enemy.GetComponent<BaseEnemy>().marked)
            {
                Instantiate(markedHit, enemy.transform.position, Quaternion.identity, null);
            }
            if (!isBoosted)
            {
                isBoosted = true;
            }
        }
        else
        {
            return;
        }
            

        

    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(this.transform.position, 2);
      
    }

}
