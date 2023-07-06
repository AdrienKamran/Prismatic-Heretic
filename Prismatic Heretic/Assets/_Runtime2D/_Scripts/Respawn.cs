using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    Vector3 originalPosition;
    Rigidbody2D body;
    public bool connectedToMovingPlatform = false;
    public MovingPlatform movingPlatform;
    Vector3 relativeDistance;
    // Start is called before the first frame update
    void Start()
    {
        body = this.GetComponent<Rigidbody2D>();
        originalPosition = this.transform.position;
        relativeDistance = new Vector3(0.0f, 0.0f, 0.0f);
        if (connectedToMovingPlatform)
        {
            if (movingPlatform)
            {
                relativeDistance = (originalPosition - movingPlatform.gameObject.transform.position);
            }
        }
    }

    public void KillAndRespawn()
    {
        if (connectedToMovingPlatform)
        {
            this.transform.position = movingPlatform.gameObject.transform.position + relativeDistance;
         //   body.position = new Vector3(0.0f, 0.0f, 0.0f);
        }
        else
        {
             this.transform.position = originalPosition;
          //  body.position = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
    
}
