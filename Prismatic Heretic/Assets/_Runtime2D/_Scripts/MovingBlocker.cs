using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlocker : MonoBehaviour
{
    /* Moving blocker logic:
     * Platforms with this script attached will move at a given speed
     * bewteen the navpoints given in the array, in the order of the array.
     * The platform will always begin at the navpoint with the startingPoint index.
     */

    public float speed; // Movespeed of the platform
    public int startingPoint; // The index of the first platform to begin the cycle from.
    public Transform[] navPoints; // The array of the navpoints this platform should move between.
    private int i; // Iterator
    public bool frozen;
    public float freezeTime;
    public SpriteRenderer sr;
    private Color magentaRGB = new Color(255, 0, 195);
    private Color greenRGB = new Color(0, 255, 54);


    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        frozen = false;
        transform.position = navPoints[startingPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
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

    public void CallFreeze()
    {
        StartCoroutine("Freeze");
    }

    private IEnumerator Freeze()
    {
        if (!frozen)
        {
            float originalSpeed = speed;
            frozen = true;
            sr.color = greenRGB;
            Debug.Log("Moving block has been frozen!");
            speed = 0;
            yield return new WaitForSeconds(freezeTime);
            speed = originalSpeed;
            frozen = false;
            Debug.Log("Moving block has been unfrozen!");
            sr.color = magentaRGB;
        }
        
    }
}
