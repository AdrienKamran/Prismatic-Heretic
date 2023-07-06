using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{

    public float speed = 20f;
    public Rigidbody2D rb;
  //  private int damage = 20; //Actually, these don't deal damage
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    void OnTriggerStay2D(Collider2D objectHit)
    {
        Debug.Log(objectHit.name);
        
        if (objectHit.gameObject.layer!=9)
        {
            if (objectHit)
            {
                BaseEnemy enemy = objectHit.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    //      Debug.Log("Dealing damage"); No, not here! But for the other weapons you might do something similar to this
                    //      enemy.DealDamage(damage);
                    if (!enemy.stunned)
                    {

                        enemy.SetMarked(true);

                    }
                }
                else if(objectHit.gameObject.layer == 10)
                {
                    Rigidbody2D body = objectHit.GetComponent<Rigidbody2D>();
                    if (body)
                    {
                        // body.velocity = new Vector2(0.0f,0.0f);
                        //already frozen
                        if (body.constraints == RigidbodyConstraints2D.FreezeAll)
                        {
                            //unfreeze
                            body.constraints = RigidbodyConstraints2D.None;
                            body.constraints = RigidbodyConstraints2D.FreezeRotation;
                        }
                        else
                        {
                            //freeze!
                            body.constraints = RigidbodyConstraints2D.FreezeAll;
                        }
                    }
                }
                
                // Code for freezing a moving blocker
                else if (objectHit.gameObject.CompareTag("MovingBlocker"))
                {
                    objectHit.GetComponent<MovingBlocker>().CallFreeze();
                }
            }
            Destroy(gameObject);

        }
        

    }

}
