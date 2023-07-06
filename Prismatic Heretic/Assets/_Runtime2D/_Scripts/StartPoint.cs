using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPoint : MonoBehaviour
{
    public int prevLevel;
    private GameObject camera;
    private GameObject player;
    private WarpSword blue;
    private AttackAOE red;
    private ParalyzerSword yellow;
    private GameObject fade;
    // Start is called before the first frame update
    void Start()
    {
        
        if (MainMenu.first == 0)
        {
            MainMenu.previousLevel = 0;
        }
        Debug.Log(MainMenu.previousLevel + ", " + prevLevel);
        if (MainMenu.previousLevel == prevLevel)
        {
            if (MainMenu.first != 0)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(1)); 
            }
            MainMenu.first++;
            Player.inDialog = false;
        camera = FindObjectOfType<CameraFollow>().gameObject;
        blue = FindObjectOfType<WarpSword>();
        red = FindObjectOfType<AttackAOE>();
        yellow = FindObjectOfType<ParalyzerSword>();
       if (blue != null)
        {
            blue.gameObject.transform.parent.transform.position = this.transform.position;
            blue.prepped = false;
            blue.warping = false;
            WarpSword.isBoosted = false;
        }
        if (red != null)
        {
            red.gameObject.transform.parent.transform.position = this.transform.position;
            red.prepped = false;
            red.activated = false;
        }
       if (yellow != null)
        {
            yellow.gameObject.transform.parent.transform.position = this.transform.position;
            yellow.prepped = false;
            yellow.orbsUsed = 0;
        }

        GameObject[] orbs;
        orbs = GameObject.FindGameObjectsWithTag("Orb");
        for (int i = 0; i < orbs.Length; i++)
        {
            Destroy(orbs[i]);
        }

        
        player = GameObject.Find("Heretic");
        fade= GameObject.Find("Fade");
        player.transform.position = this.transform.position;
        camera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, camera.transform.position.z);
        fade.GetComponent<Animator>().Play("Crossfade_End");

    }
    }
    // Update is called once per frame

}
