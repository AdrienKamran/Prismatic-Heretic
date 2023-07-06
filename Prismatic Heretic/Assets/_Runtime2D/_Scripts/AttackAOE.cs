using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAOE : MonoBehaviour
{
    //TODO: We might want to reorganize these to be more parallel with the other sword equivalents (or vice versa).
    public GameObject anchor;
    public GameObject spawnPoint;
    public GameObject explosion;
    public GameObject slash;

    //NEW: (needed to help us keep track of which sword is selected)
    public SelectSword selectionControl;

    float explosionLife = 0.3f;

    // This is the red equivalent of blue's "warping" bool
    public static bool isSwinging=false; 
    //NEW: 
    public bool prepped = false;

    public LayerMask enemyLayer;

    public LayerMask interactableLayer;

    public GameObject player;

    public GameObject circle;

    public GameObject pullEffect;

    public Rigidbody2D body;

    public bool activated=false;

    public static bool berserk=false;

    public ParticleSystem trail;
    public ParticleSystem markedHit;

    public Material purpleMaterial;
    public Material redMaterial;

    public AudioSource playerGeneral;
    public AudioSource playerLoop;
    public AudioClip berserkActivation;

    public AudioSource source;
    public AudioClip ability;
    public AudioClip prep;
    public AudioClip swing;
    

    // Start is called before the first frame update
    void Start()
    {
        isSwinging = false;

        berserk = false;
        
        trail.Stop();
        player = GameObject.Find("Heretic");
        InvokeRepeating("berserkMode", 0f, 1f);
        
    }

    // Update is called once per frame
    void Update()
    {

        if (!Player.GameIsPaused)
        {
            if (Player.inDialog == false)
            {

                //If Red is being held (as opposed to prepped or not present at all)
                if (selectionControl.currentlyHeld == 2)
                {
                    //So I've left this in for now but... What is this animation actually meant for? The initial prep?
                    if (Input.GetKeyDown(KeyCode.G)) //Changed from E to G, since E is for activation (as of writing this at least)
                    {

                        Debug.Log(anchor.transform.rotation.z);

                        Quaternion rotation = anchor.transform.rotation;
                        GameObject exp = Instantiate(explosion, spawnPoint.transform.position, rotation);
                        Destroy(exp, explosionLife);
                    }

                    //Now the standard stufF:

                    //Left mouse to swing sword
                    if (Input.GetKeyDown(KeyCode.Mouse0) && !isSwinging && !prepped) //changed from mouse0 to mouse1 (should be the same as other swords)
                    {
                        this.GetComponent<Animator>().Play("Swing");
                        player.GetComponent<Player>().TakeDamage(5);
                    }



                    if (Input.GetKeyDown(KeyCode.Mouse1) && !prepped) //changed from mouse0 to mouse1 (should be the same as other swords)
                    {
                        if (berserk == false)
                        {
                            if (!WarpSword.isBoosted)
                            {
                                player.GetComponent<SpriteRenderer>().material = redMaterial;
                            }
                            if (!playerLoop.isPlaying)
                            {
                                playerLoop.Play(0);
                            }
                            playerGeneral.PlayOneShot(berserkActivation);
                            trail.Play();
                            berserk = true;
                        }
                        else if (berserk == true)
                        {
                            if (!WarpSword.isBoosted)
                            {
                                player.GetComponent<SpriteRenderer>().material = purpleMaterial;
                            }
                            playerLoop.Stop();
                            trail.Stop();
                            berserk = false;
                        }
                    }


                    //Right mouse to prep sword
                    if (Input.GetKeyDown(KeyCode.Q) && !isSwinging && prepped == false)
                    {
                        float dist = Vector3.Distance(player.transform.position, this.transform.position);
                        if (dist < 4.5)
                        {
                            source.PlayOneShot(prep);
                            Debug.Log("Red is prepped!");
                            prepped = true;
                            selectionControl.redInUse = true;
                            Prep();

                        }
                    }
                }

                //If red is prepped and gets activated
                if (Input.GetKeyDown(KeyCode.E) && prepped && !activated)
                {
                    Debug.Log("Red has been activated!");
                    if (Player.redPowerDelay == 50)
                    {
                        Activate();
                        Player.redPowerDelay = 0;
                    }
                   

                }
                //If red is neither being swung nor prepped, check if it should be active depending on whether it is meant
                //to be the currently held sword
                if (!isSwinging && !prepped)
                {
                    float dist = Vector3.Distance(player.transform.position, this.transform.position);
                    if (dist < 5)
                    {
                        selectionControl.redInUse = false;
                        if (selectionControl.currentlyHeld == 1 || selectionControl.currentlyHeld == 3)
                        {
                            selectionControl.redSword.GetComponent<SpriteRenderer>().enabled = false;
                        }
                    }
                }
            }
            else
            {
                player.GetComponent<SpriteRenderer>().material = purpleMaterial;
                trail.Stop();
                playerLoop.Stop();
                berserk = false;
            }
        }
    }


    void Slash()
    {
        
        slash.gameObject.SetActive(true);
    }

    void RemoveSlash()
    {
        slash.gameObject.SetActive(false);
    }

    void StartSwing()
    {
        source.PlayOneShot(swing);
        isSwinging = true;
    }
    void ResetSwing()
    {
        isSwinging = false;
    }
    void Prep()
    {
        body.velocity = new Vector2(0.0f, 0.0f);
    }

    void Activate()
    {
        StartCoroutine(Wait());
        player.GetComponent<Player>().TakeDamage(10);
        pullEnemies();
    }

    void pushEnemies()
    {
        
            
            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(this.transform.position, 3, enemyLayer);
            
           
            foreach (Collider2D enemy in enemiesInRange)
            {
            enemy.GetComponent<BaseEnemy>().hit();
            if (!berserk)
            {
                Debug.Log("Hit red");
                enemy.GetComponent<BaseEnemy>().DealDamage(25);
                enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(-player.transform.position.x + enemy.transform.position.x, -player.transform.position.y + enemy.transform.position.y) * 20);

            }
            else
            {
                enemy.GetComponent<BaseEnemy>().DealDamage(50);
                enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(-player.transform.position.x + enemy.transform.position.x, -player.transform.position.y + enemy.transform.position.y) * 50);
            }
            if (enemy.GetComponent<BaseEnemy>().marked)
            {
                Instantiate(markedHit, enemy.transform.position, Quaternion.identity, null);
            }
          
                
            }

        


    }


    void pullEnemies()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(this.transform.position, 5, enemyLayer);
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(this.transform.position, 5, interactableLayer);

        source.PlayOneShot(ability);
        GameObject obj = Instantiate(pullEffect, this.transform.position, Quaternion.identity);
        Destroy(obj, 3f);

        //Deal with enemies
        foreach (Collider2D enemy in enemiesInRange)
        {
            enemy.GetComponent<BaseEnemy>().hit();
            
            StartCoroutine(TimerRoutine(enemy));
            enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(-(-this.transform.position.x + enemy.transform.position.x), -(-this.transform.position.y + enemy.transform.position.y)) * 45);
            if (!berserk)
            {
                enemy.GetComponent<BaseEnemy>().DealDamage(20);
            }
            else
            {
                enemy.GetComponent<BaseEnemy>().DealDamage(40);
            }
            if (enemy.GetComponent<BaseEnemy>().marked)
            {
                Instantiate(markedHit, enemy.transform.position, Quaternion.identity, null);
            }
        }

        //Deal with objects
        foreach (Collider2D interactable in objectsInRange)
        {
            StartCoroutine(TimerRoutine(interactable));
            interactable.GetComponent<Rigidbody2D>().AddForce(new Vector2(-(-this.transform.position.x + interactable.transform.position.x), -(-this.transform.position.y + interactable.transform.position.y)) * 45);
        }
    }


    private IEnumerator TimerRoutine(Collider2D enemy)
    {
       
        enemy.GetComponent<Rigidbody2D>().drag = 15;
        yield return new WaitForSeconds(0.5f);
        if (enemy != null)
        {
            enemy.GetComponent<Rigidbody2D>().drag = 3.5f;
        }
       
       

    }

    private IEnumerator Wait()
    {
        activated = true;
        yield return new WaitForSeconds(1.0f);
        prepped = false;
        activated = false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(this.transform.position, 3);
        Gizmos.DrawWireSphere(this.transform.position, 5);
    }

    public void berserkMode()
    {
        if (berserk)
        {

            player.GetComponent<Player>().TakeDamage(2);
        }
        
    }
}


