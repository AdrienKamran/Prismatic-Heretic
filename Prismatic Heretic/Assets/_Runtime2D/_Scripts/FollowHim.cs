using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHim : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y-1.2f, player.transform.position.z);
    }
}
