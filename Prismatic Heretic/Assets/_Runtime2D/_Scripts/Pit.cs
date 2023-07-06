using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : MonoBehaviour
{
    // Start is called before the first frame update
 
     public GameObject Player;
    bool collided = false;
    private void Start()
    {
        Player = FindObjectOfType<Player>().gameObject;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            collided = false;
        }
    }


    IEnumerator OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
        if (!MovingPlatform.onPlat)
        {       
  //      Debug.Log("touch");
        if (collision.gameObject.layer == 7)
        {
        //    Debug.Log("in");
            collided = true;
        yield return new WaitForSeconds(0.1f);
            //checking if collided with player
           
            if (collided&& !MovingPlatform.onPlat)
            StartCoroutine(ScaleOverTime(1));
        }
        }

        if (collision.gameObject.layer == 10)
        {
            Respawn o = collision.gameObject.GetComponent<Respawn>();
            o.KillAndRespawn();
        }
        }
    }   
   

    IEnumerator ScaleOverTime(float time)
    {
        
        Vector3 originalScale = Player.transform.localScale;
        Vector3 destinationScale = new Vector3(0.1f, 0.1f, 0.1f);
        Player.GetComponent<Movement2D>().enabled = false;
        Player.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
        Player.GetComponent<BoxCollider2D>().enabled = false;
        float currentTime = 0.0f;

        do
        {
            Player.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);

        Player.GetComponent<Player>().TakeDamage(100);
        

        
    }
    

}
