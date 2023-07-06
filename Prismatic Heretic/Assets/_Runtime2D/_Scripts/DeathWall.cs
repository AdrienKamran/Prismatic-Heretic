using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    private GameObject player;
    private void Start()
    {
        player = GameObject.Find("Heretic");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            player.GetComponent<Player>().TakeDamage(100);
        }

    }
}
