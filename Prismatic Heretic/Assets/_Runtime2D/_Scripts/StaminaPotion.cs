using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaPotion : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameObject player;

    private void Start()
    {
        player = FindObjectOfType<Player>().gameObject;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player == collision.gameObject)
        {
            player.GetComponent<Player>().GainStamina(30);
            Destroy(this.gameObject);
        }
    }
}
