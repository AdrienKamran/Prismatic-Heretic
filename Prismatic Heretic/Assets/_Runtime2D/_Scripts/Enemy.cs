using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ideally, enemy classes for the blue, red, and yellow enemies will be subclasses of this Enemy class
//and the functions below will be overwritten where necessary.
public class Enemy : MonoBehaviour
{
    public GameObject stunParticles;
    int life = 50;
    Rigidbody2D body;
    bool emitingParticles=false;
    public bool stunned = false;
    public bool marked = false; //for extra damage, if first hit by yellow
    // Start is called before the first frame update
    void Start()
    {
        body = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if(life <= 0) 
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (!stunned)
        { 
                Move();
        }
       
    }

    public void DealDamage(int damage)
    {
        life -= damage;
    }

    public void SetStunned(bool setting) 
    {
        stunned = setting;
        if(stunned == true)
            StartCoroutine(Stun());
    }

    public void SetMarked(bool status)
    {
        marked = status;
        if (marked == true)
            StartCoroutine(Mark());
    }

    //Should be overwritten by subclasses
    protected void Move()
    {
        //Extremely basic movement for generic enemy
        float xPos = Mathf.Cos(Time.time);
    //    Debug.Log(xPos);
      //  float yPos = body.velocity.y;
        //     body.velocity = -(new Vector2(xPos, yPos)) * 5.0f;
        body.AddForce(new Vector2(xPos, 0));
    }

    IEnumerator Stun()
    {
       
        body.velocity = new Vector2(0.0f, 0.0f);
        yield return new WaitForSeconds(3.0f);
       
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
}
