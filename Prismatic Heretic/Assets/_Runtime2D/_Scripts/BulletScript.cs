using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D rb;

    public int damage = 10;

    private bool initialize = false;
    //  private int damage = 20; //Actually, these don't deal damage
    // Start is called before the first frame update
    void Start()
    {
        // rb.velocity = transform. * speed;
    }

    void OnTriggerStay2D(Collider2D objectHit)
    {


        if (objectHit)
        {
            Player player = objectHit.GetComponent<Player>();
            BaseEnemy enemy = objectHit.GetComponent<BaseEnemy>();
            if (player != null)
            {
                //      Debug.Log("Dealing damage"); No, not here! But for the other weapons you might do something similar to this
                //      enemy.DealDamage(damage);
                player.TakeDamage(damage);
                Destroy(gameObject);
            }

            
            if (!enemy && !player)
            {
                Destroy(gameObject);   
            }
        }

        

    }
}
