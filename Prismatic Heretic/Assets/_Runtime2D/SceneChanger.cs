using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    // Start is called before the first frame update

    public int levelNumber;
    MainMenu levelControl;
    bool triggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.layer == 7)
        {
            levelControl = FindObjectOfType<MainMenu>();
            if (!triggered)
            {
                Player.inDialog = true;
                StartCoroutine(levelControl.LoadLevel(levelNumber));
                triggered = true;
            }
          
           
            
        }
    }
}
