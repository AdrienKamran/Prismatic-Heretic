using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GettingSword : MonoBehaviour
{
    // Start is called before the first frame update
    public bool blue;
    public bool red;
    public bool yellow;
    public bool got;
    GameObject player;

    void Start()
    {
        got = false;
        player = GameObject.Find("Heretic");
        if (blue)
        {
            if (SelectSword.hasBlue)
            {
                Destroy(this.gameObject);
            }
        }
        if (red)
        {
            if (SelectSword.hasRed)
            {
                Debug.Log("hasRed: " + SelectSword.hasRed);
                Destroy(this.gameObject);
            }
        }
        if (yellow)
        {
            if (SelectSword.hasYellow)
            {
                Destroy(this.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!got)
        {
            float dist = Vector3.Distance(player.transform.position, this.transform.position);
            if (dist < 1)
            {
                if (blue)
                {
                    SelectSword.hasBlue = true;
                    got = true;
                    Destroy(this.gameObject);
                    

                }
                if (red)
                {
                    SelectSword.hasRed = true;
                    got = true;
                    Destroy(this.gameObject);
                  

                }
                if (yellow)
                {
                    SelectSword.hasYellow = true;
                    got = true;
                    Destroy(this.gameObject);
                    
                }

            }
        }
        

    }
}
