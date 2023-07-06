using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowEnemy : BaseEnemy
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    // This is when enemy spotted player and they have to go back
    public float switchPositionTime = 1.0f;
    private float switchPositionTimeToUse = 1.0f;
    // They need to switch position
    public bool switchPosition = false;
    GameObject player;
    public float bulletSpeed = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>().gameObject;
      switchPositionTimeToUse = switchPositionTime;
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

    // Update is called once per frame
    public override void Update() 
    {
        base.Update();
        Vector3 direction = this.gameObject.transform.position - player.transform.position;
        animator.SetFloat("PlayerVertical", -direction.normalized.y);
        animator.SetFloat("PlayerHorizontal", -direction.normalized.x);
        animator.SetBool("Idle", idle);
        if (AIState == AIStateType.ChasingPlayer && switchPosition == true)
        {
            switchPositionTimeToUse -= Time.deltaTime;

            if (switchPositionTimeToUse <= 0)
            {
                switchPosition = false;
                switchPositionTimeToUse = switchPositionTime;
            }
        }


    }

    public override void AttackPlayer()
    {
        if (!attack)
        {
            // fire bullet
            Debug.Log("Attack!");
            StartCoroutine(attackWait());
            animator.Play("Attack");
            Vector3 direction = (player.transform.position - this.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.y) * Mathf.Rad2Deg;

            // GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.AngleAxis(angle+90f, Vector3.forward));
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            BulletScript bulletrb = bullet.GetComponent<BulletScript>();
            bulletrb.rb.velocity = direction * bulletSpeed;
            bulletrb.damage = this.damage;
            
        }
        
    }

    private IEnumerator attackWait()
    {
        attack = true;
        yield return new WaitForSeconds(1.5f);
        attack = false;
        
    }

    public override void AttackBehaviour(Collider2D playerInRange)
    {
        if (AIState != AIStateType.ChasingPlayer)
        {
            AIState = AIStateType.ChasingPlayer;
        }

        // Attack
        if (AIState == AIStateType.ChasingPlayer)
        {
            Debug.Log(attack);
            if (attack == false)
            {
                AttackPlayer();
            }
            else
            {
                
                animator.Play("Mouvement");
            }

            
                //if (!switchPosition)
                //{
                //    RoamingGameObject.transform.position = GetCircumferencePosition(playerInRange);
                //    AIDestinationSetter.target = RoamingGameObject.transform;
                //    switchPosition = true;
                //}

                // Attack

                if (!this.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                if (Vector2.Distance(this.transform.position, playerInRange.transform.position) <= 15.0f && AIState == AIStateType.ChasingPlayer)
                {
                    Vector3 direction = this.transform.position - playerInRange.transform.position;
                    RoamingGameObject.transform.position = this.transform.position + (direction);
                    AIDestinationSetter.target = RoamingGameObject.transform;
                }
            }
            







        }

        
    }

    // We are making the enemy AI surrounds the player within the field of vision
    private Vector3 GetCircumferencePosition(Collider2D player)
    {
        float angleRadian = Random.Range(-Mathf.PI/4, Mathf.PI/4);
        Vector3 position = new Vector2(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian));
        return player.transform.position + (position * fieldOfVision);
    }
}
