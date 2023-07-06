using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingPlatform : MonoBehaviour
{
    /* Moving platform logic:
     * Platforms with this script attached will move at a given speed
     * bewteen the navpoints given in the array, in the order of the array.
     * The platform will always begin at the navpoint with the startingPoint index.
     */

    public float speed; // Movespeed of the platform
    public int startingPoint; // The index of the first platform to begin the cycle from.
    public Transform[] navPoints; // The array of the navpoints this platform should move between.
    private int i; // Iterator
    private string originalSceneName;
    public GameObject blue;
    public GameObject red;
    public GameObject yellow;
    public bool onPlatformPlayer=false;
    public bool onPlatformSword=false;
    public static bool onPlat=false;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = navPoints[startingPoint].position;
        originalSceneName = SceneManager.GetActiveScene().name;
        blue = GameObject.Find("Blue").gameObject;
        red= GameObject.Find("Red").gameObject;
        yellow= GameObject.Find("Yellow").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
   //     Debug.Log("On platform: "+onPlat);
        if (Vector2.Distance(transform.position, navPoints[i].position) < 0.02f)
        {
            i++;
            if (i == navPoints.Length)
            {
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, navPoints[i].position, speed * Time.deltaTime);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            if (collision.gameObject.layer == 7)
            {
                onPlat = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.layer == 7)
        {
            if (!collision.isTrigger)
            {
                // Debug.Log("Player standing on platform");
                SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetSceneByName("Player"));
                collision.transform.SetParent(transform);
                onPlatformPlayer = true;
                onPlat = true;
            }
        }
        if (collision.gameObject.layer == 14)
        {
            onPlatformSword = true;
            if (collision.gameObject.tag == "BlueSword")
            {
                if (collision.gameObject.GetComponent<WarpSword>().prepped)
                {
                    //Debug.Log("Parent is: " + collision.gameObject.transform.parent.gameObject.name);
                   // Debug.Log("Child is: " + collision.gameObject.name);
                    
                    collision.transform.parent.SetParent(transform);
                }
                else if (!collision.gameObject.GetComponent<WarpSword>().prepped)
                {
                   // Debug.Log("NOT PREPPED, Parent is: " + collision.gameObject.transform.parent.gameObject.name);
                   // Debug.Log("NOT PREPPED, Child is: " + collision.gameObject.name);
                    
                    collision.transform.parent.SetParent(null);
                }
            }
            if (collision.gameObject.tag == "RedSword")
            {
                if (collision.gameObject.GetComponent<AttackAOE>().prepped)
                {
                    //Debug.Log("Parent is: " + collision.gameObject.transform.parent.gameObject.name);
                   // Debug.Log("Child is: " + collision.gameObject.name);

                    collision.transform.parent.SetParent(transform);
                }
                else if (!collision.gameObject.GetComponent<AttackAOE>().prepped)
                {
                   // Debug.Log("NOT PREPPED, Parent is: " + collision.gameObject.transform.parent.gameObject.name);
                    //Debug.Log("NOT PREPPED, Child is: " + collision.gameObject.name);

                    collision.transform.parent.SetParent(null);
                }
            }
            if (collision.gameObject.tag == "YellowSword")
            {
                if (collision.gameObject.GetComponent<ParalyzerSword>().prepped)
                {
                    //Debug.Log("Parent is: " + collision.gameObject.transform.parent.gameObject.name);
                    //Debug.Log("Child is: " + collision.gameObject.name);

                    collision.transform.parent.SetParent(transform);
                }
                else if (!collision.gameObject.GetComponent<ParalyzerSword>().prepped)
                {
                   // Debug.Log("NOT PREPPED, Parent is: " + collision.gameObject.transform.parent.gameObject.name);
                   // Debug.Log("NOT PREPPED, Child is: " + collision.gameObject.name);

                    collision.transform.parent.SetParent(null);
                }
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            if (!collision.isTrigger)
            {
                onPlatformPlayer = false;
                StartCoroutine(Wait());
                collision.transform.SetParent(null);
            }
        }
        if (collision.gameObject.layer == 14)
        {
            onPlatformSword = false;
            if (collision.gameObject.tag == "BlueSword")
            {
                collision.transform.SetParent(blue.transform);
            }
            if (collision.gameObject.tag == "RedSword")
            {
                collision.transform.SetParent(red.transform);
                
            }
            if (collision.gameObject.tag == "YellowSword")
            {
                collision.transform.SetParent(yellow.transform);
               
            }
        }
        if (!onPlatformPlayer && !onPlatformSword)
        {
            Debug.Log("Left the platform");
            SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetSceneByName(originalSceneName));
        }

    }
    private IEnumerator Wait()
    {
        onPlat = false;
        yield return new WaitForSeconds(0.5f);
        if (onPlatformPlayer)
        {
            onPlat = true;
        }
        
    }
}
