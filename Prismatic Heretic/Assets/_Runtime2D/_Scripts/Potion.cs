using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public static GameObject player;

    private void Start()
    {
        player = FindObjectOfType<Player>().gameObject;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player== collision.gameObject)
        {
            player.GetComponent<Player>().GainHealth(10);
            Destroy(this.gameObject);
        }
    }
}
