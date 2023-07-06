using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseEnemy : MonoBehaviour
{
    public GameObject slashUp;
    public GameObject slashDown;
    public GameObject slashRight;
    public GameObject slashLeft;
    public Slider healthEnemyBar;

    public enum AIStateType
    {
        Roaming, GoingBack, ChasingPlayer
    }

    public enum EnemyType
    {
        BlueEnemy, RedEnemy, YellowEnemy
    }
    public LayerMask playerMask;

    // Set this stuff in the level designer
    public Animator animator;
    public Rigidbody2D rigidBody;
    public int damage = 20;
    public Seeker seeker;
    public AIPath aIPath;
    public AIDestinationSetter AIDestinationSetter;
    public bool initialized = false;

    // For AI
    internal AIStateType AIState;
    public float restingTime = 3.0f;
    public GameObject stunParticles;
    private bool emitingParticles = false;
    public bool stunned = false;
    public bool marked = false; //for extra damage, if first hit by yellow
    public float fieldOfVision = 5;

    public EnemyType enemyType; // Enemy Type either Blue Enemy or Red Enemy or Green Enemy

    // Keeping initial position
    public Vector3 StartingPosition;
    public GameObject StartingPositionGameObject; // Storing starting transform since the AI Destination Setter only accepts transform

    // This is for keeping transform when enemy is patrolling
    public GameObject RoamingGameObject; // It is dumb because AIDestinationSetter only accepts transform...., super not optimized

    public float HealthBar = 100; // float so we can link it to UI

    // For Containing Player Position

    // For Movement
    float delayTimer;
    Vector2 movement;
    internal Vector2 directions;
    public float speed = 100;
    public bool idle = false;

    // for Attack
    public bool attack = false;

    public float attackTimer = 3f;
    internal float attackTimerToUse = 2f;
    public float attackRange = 3f;

    public GameObject potion;
    public GameObject staminaPotion;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>().gameObject;
        StartingPosition = rigidBody.transform.position;
        RoamingGameObject = new GameObject();
        StartingPositionGameObject = new GameObject();
        StartingPositionGameObject.transform.position = StartingPosition;
        AIState = AIStateType.Roaming;
        rigidBody = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
        AIDestinationSetter = this.GetComponent<AIDestinationSetter>();
        aIPath = this.GetComponent<AIPath>();

        aIPath.maxSpeed = speed;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position + new Vector3(2, 0, 0), attackRange);
        Gizmos.DrawWireSphere(this.transform.position + new Vector3(-2, 0, 0), attackRange);
        Gizmos.DrawWireSphere(this.transform.position + new Vector3(0, 1.5f, 0), attackRange);
        Gizmos.DrawWireSphere(this.transform.position + new Vector3(0, -1.5f, 0), attackRange);
    }
    private void OnDrawGizmosSelected()
    {
        //  Gizmos.DrawWireSphere(this.transform.position, fieldOfVision);

        // if (attack)
        //   {
        // Gizmos.DrawWireSphere(this.transform.position, attackRange);
        // }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (enemyType != EnemyType.YellowEnemy)
        {
            animator.SetBool("Idle", idle);
        }
        animator.SetFloat("MovementHorizontal", movement.x);
        animator.SetFloat("MovementVertical", movement.y);




        // The initialization if start doesn't work.
        // Cause my A2 the Start doesn't work for starting position smh
        if (!initialized)
        {
            RoamingGameObject.transform.position = GetRoamingPosition();
            StartingPosition = rigidBody.transform.position;
            initialized = true;
            aIPath.maxSpeed = speed;
        }

        delayTimer += Time.deltaTime;

        Movement();


        healthEnemyBar.value = HealthBar;
    }

    // Responsible for direction AI is moving and changing animations when moving in direction
    public virtual void Movement()
    {
        if (AIState == AIStateType.Roaming)
        {
            // chooses random direction every three seconds
            if (delayTimer > 3.0f)
            {
                // moving when the delayTimer is up
                RoamingGameObject.transform.position = GetRoamingPosition();
                AIDestinationSetter.target = RoamingGameObject.transform;
                delayTimer = 0;
            }

            movement = aIPath.velocity;

        }
        else if (AIState == AIStateType.GoingBack)
        {
            // going out
            // trying to find the momvement
            Vector3 direction = StartingPosition - rigidBody.transform.position;
            direction /= Time.fixedDeltaTime;

            direction = Vector3.ClampMagnitude(direction, speed);
            movement = direction;
        }
        // AIChasing after player
        else if (AIState == AIStateType.ChasingPlayer)
        {
            // setting the movement stats with AIPath velocity so animation works properly

            movement = aIPath.velocity;


        }

        // Moving
        if (movement.y != 0 || movement.x != 0)
        {
            idle = false;
        }
        else
        {
            idle = true;
        }

        // For the animator 

    }

    public virtual void FixedUpdate()
    {
        Collider2D playerInRange = Physics2D.OverlapCircle(this.transform.position, fieldOfVision, playerMask);

        if (playerInRange)
        {
            AttackBehaviour(playerInRange);
        }
        else
        {
            if (AIState == AIStateType.ChasingPlayer)
            {
                AIState = AIStateType.GoingBack;
                AIDestinationSetter.target = StartingPositionGameObject.transform;
            }
        }

        // When NPC dies
        if (HealthBar <= 0)
        {
            Vector3 pos = this.transform.position;
            Quaternion rotation = this.transform.rotation;
            int randomizer = Random.Range(0, 2);
            Debug.Log(randomizer);
            if (randomizer == 0)
            {
                Instantiate(potion, pos, rotation);
                Destroy(this.gameObject.gameObject);
            }
            randomizer = Random.Range(0, 2);
            if (randomizer == 0)
            {
                Instantiate(staminaPotion, pos, rotation);
                Destroy(this.gameObject.gameObject);

            }
            else
            {
                Destroy(this.gameObject.gameObject);
            }

        }

        // Reaches Starting position
        if (Vector2.Distance(this.transform.position, StartingPosition) <= 0.5f && AIState == AIStateType.GoingBack)
        {
            AIState = AIStateType.Roaming;
            AIDestinationSetter.target = null;
        }
    }

    public virtual void AttackBehaviour(Collider2D playerInRange)
    {
        if (AIState != AIStateType.ChasingPlayer)
        {
            AIState = AIStateType.ChasingPlayer;
            AIDestinationSetter.target = playerInRange.transform;
        }

        // Attack
        if (Vector2.Distance(this.transform.position, playerInRange.transform.position) <= 6.0f && AIState == AIStateType.ChasingPlayer)
        {

            if (!attack && !stunned)
            {
                Vector3 dir = player.transform.position - this.gameObject.transform.TransformPoint(Vector3.zero);
                dir = dir.normalized;
                // Debug.Log("Vertical"+ -direction.normalized.y);
                //  Debug.Log("Horizontal" + -direction.normalized.x);
                if (dir.x > 0.3f)
                {
                    dir.x = 1;
                }
                else if (dir.x < -0.3f)
                {
                    dir.x = -1;
                }
                else
                {
                    dir.x = 0;
                }
                if (dir.y > 0.3f)
                {
                    dir.y = 1;
                }
                else if (dir.y < -0.3f)
                {
                    dir.y = -1;
                }
                else
                {
                    dir.y = 0;
                }


                animator.SetFloat("PlayerVertical", dir.y);
                animator.SetFloat("PlayerHorizontal", dir.x);
                StartCoroutine(attackWait());
                animator.Play("Attack");
                animator.SetBool("Attack", false);
            }


        }
    }

    private Vector3 GetRoamingPosition()
    {
        // The Range Can be change, is simply their 
        return StartingPosition + chooseRandomMoveDirection() * Random.Range(1f, 5f);
    }

    // For Patrolling
    private Vector3 chooseRandomMoveDirection()
    {
        movement.x = Random.Range(-1f, 1f);
        movement.y = Random.Range(-1f, 1f);

        Vector2 returnVector = new Vector2(movement.x, movement.y);

        return returnVector;
    }

    // Attack Player
    public virtual void AttackPlayer()
    {
        if (!attack && !stunned)
        {

            Collider2D attackCollider = Physics2D.OverlapCircle(this.transform.position, attackRange, playerMask);

            // Found Player
            if (attackCollider)
            {
                Player player = attackCollider.GetComponent<Player>();
                if (player)
                {
                    player.TakeDamage(damage);
                }
            }
        }
    }

    public virtual void AttackPlayerRight()
    {
        if (!stunned)
        {
            Debug.Log("Right");
            Collider2D attackCollider = Physics2D.OverlapCircle(this.transform.position + new Vector3(2, 0, 0), attackRange, playerMask);

            // Found Player
            if (attackCollider)
            {
                Player player = attackCollider.GetComponent<Player>();
                if (player)
                {
                    player.TakeDamage(damage);
                }
            }

        }
    }
    public void AttackPlayerLeft()
    {
        if (!stunned)
        {
            Debug.Log("Left");
            Collider2D attackCollider = Physics2D.OverlapCircle(this.transform.position + new Vector3(-2, 0, 0), attackRange, playerMask);

            // Found Player
            if (attackCollider)
            {
                Player player = attackCollider.GetComponent<Player>();
                if (player)
                {
                    player.TakeDamage(damage);
                }
            }

        }
    }
    public void AttackPlayerUp()
    {
        if (!stunned)
        {
            Debug.Log("Up");
            Collider2D attackCollider = Physics2D.OverlapCircle(this.transform.position + new Vector3(0, 1.5f, 0), attackRange, playerMask);

            // Found Player
            if (attackCollider)
            {
                Player player = attackCollider.GetComponent<Player>();
                if (player)
                {
                    player.TakeDamage(damage);
                }
            }

        }
    }


    public void AttackPlayerDown()
    {
        if (!stunned)
        {
            Debug.Log("Down");
            Collider2D attackCollider = Physics2D.OverlapCircle(this.transform.position + new Vector3(0, -1.5f, 0), attackRange, playerMask);

            // Found Player
            if (attackCollider)
            {
                Player player = attackCollider.GetComponent<Player>();
                if (player)
                {
                    player.TakeDamage(damage);
                }
            }

        }
    }
    public void SlashUpA()
    {

        slashUp.SetActive(true);

    }
    public void SlashDownA()
    {


        slashDown.SetActive(true);


    }
    public void SlashRightA()
    {

        slashRight.SetActive(true);


    }
    public void SlashLeftA()
    {
        slashLeft.SetActive(true);
    }
    public void SlashUpD()
    {
        slashUp.SetActive(false);
        slashDown.SetActive(false);
        slashRight.SetActive(false);
        slashLeft.SetActive(false);
    }
    public void SlashDownD()
    {
        slashUp.SetActive(false);
        slashDown.SetActive(false);
        slashRight.SetActive(false);
        slashLeft.SetActive(false);
    }
    public void SlashRightD()
    {
        slashUp.SetActive(false);
        slashDown.SetActive(false);
        slashRight.SetActive(false);
        slashLeft.SetActive(false);
    }
    public void SlashLeftD()
    {
        slashUp.SetActive(false);
        slashDown.SetActive(false);
        slashRight.SetActive(false);
        slashLeft.SetActive(false);
    }



    private IEnumerator attackWait()
    {
        attack = true;
        yield return new WaitForSeconds(2.0f);
        attack = false;


    }

    public void DealDamage(int damage)
    {
        HealthBar -= damage;
    }

    public void SetStunned(bool setting)
    {
        stunned = setting;
        if (stunned == true)
            StartCoroutine(Stun());
    }

    public void SetMarked(bool status)
    {
        marked = status;
        if (marked == true)
            StartCoroutine(Mark());
    }

    //Should be overwritten by subclasses
    public virtual void Move()
    {
        //Extremely basic movement for generic enemy
        float xPos = Mathf.Cos(Time.time);
        //   Debug.Log(xPos);
        //   float yPos = body.velocity.y;
        //   body.velocity = -(new Vector2(xPos, yPos)) * 5.0f;
        movement.x = xPos;
    }

    IEnumerator Stun()
    {
        rigidBody.velocity = new Vector2(0.0f, 0.0f);
        aIPath.canMove = false;
        yield return new WaitForSeconds(8.0f);
        aIPath.canMove = true;
        //   Debug.Log("Unstunned!");
        stunned = false;
    }

    IEnumerator Mark()
    {
        if (!emitingParticles)
        {
            var particle = Instantiate(stunParticles, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            particle.transform.parent = this.gameObject.transform;
            Destroy(particle, 8.0f);
            emitingParticles = true;
        }
        yield return new WaitForSeconds(8.0f);
        //   Debug.Log("Unstunned!");
        emitingParticles = false;
        marked = false;
    }

    public void hit()
    {
        if (aIPath.canMove)
        {
            StartCoroutine(HitStun());
        }
    }




    IEnumerator HitStun()
    {
        aIPath.canMove = false;
        yield return new WaitForSeconds(1.0f);
        aIPath.canMove = true;
    }
}
