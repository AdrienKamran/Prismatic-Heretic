using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSword : MonoBehaviour
{
    public GameObject blueSword;
    public GameObject redSword;
    public GameObject yellowSword;
    private GameObject prevSword;

    //A sword must stay "active" if it is being used
    public bool blueInUse = false;
    public bool redInUse = false;
    public bool yellowInUse = false;

    public GameObject blueBar;
    public GameObject redBar;
    public GameObject yellowBar;

    public GameObject bluePrep;
    public GameObject redPrep;
    public GameObject yellowPrep;

    public static bool hasBlue;
    public static bool hasRed;
    public static bool hasYellow;

    public int currentlyHeld = 1; // 1 for blue, 2 for red, 3 for yellow

    private void Start()
    {
        if (hasBlue)
        {
            currentlyHeld = 1;
            blueSword.GetComponent<SpriteRenderer>().enabled = true;
            redSword.GetComponent<SpriteRenderer>().enabled = false;
            yellowSword.GetComponent<SpriteRenderer>().enabled = false;
            foreach (Transform child in blueSword.transform)
                child.gameObject.SetActive(true);
            foreach (Transform child in yellowSword.transform)
                child.gameObject.SetActive(false);
            foreach (Transform child in redSword.transform)
                child.gameObject.SetActive(false);
        }
        else
        {
            currentlyHeld = 0;
            blueSword.GetComponent<SpriteRenderer>().enabled = false;
            redSword.GetComponent<SpriteRenderer>().enabled = false;
            yellowSword.GetComponent<SpriteRenderer>().enabled = false;
            foreach (Transform child in blueSword.transform)
                child.gameObject.SetActive(false);
            foreach (Transform child in yellowSword.transform)
                child.gameObject.SetActive(false);
            foreach (Transform child in redSword.transform)
                child.gameObject.SetActive(false);
        }
       
        
        prevSword = blueSword;
    }
    // Update is called once per frame
    void Update()
    {
        if (hasBlue)
        {
            blueBar.SetActive(true);
            bluePrep.SetActive(true);
        }
        else
        {
            blueBar.SetActive(false);
            bluePrep.SetActive(false);
        }

        if (hasRed)
        {
            redBar.SetActive(true);
            redPrep.SetActive(true);
        }
        else
        {
            redBar.SetActive(false);
            redPrep.SetActive(false);
        }
        if (hasYellow)
        {
            yellowBar.SetActive(true);
            yellowPrep.SetActive(true);
        }
        else
        {
            yellowBar.SetActive(false);
            yellowPrep.SetActive(false);
        }
        if (!Player.GameIsPaused)
        {
            if (Player.inDialog == false)
            {
                if (!AttackAOE.isSwinging && !WarpSword.isSwinging)
                {

                    if (Input.GetKeyDown(KeyCode.Alpha1)&&hasBlue)
                    {
                        blueSword.GetComponent<SpriteRenderer>().enabled = true;
                        if (redInUse == false)
                            redSword.GetComponent<SpriteRenderer>().enabled = false;
                        if (yellowInUse == false)
                            yellowSword.GetComponent<SpriteRenderer>().enabled = false;
                        currentlyHeld = 1;
                        foreach (Transform child in blueSword.transform)
                            child.gameObject.SetActive(true);
                        foreach (Transform child in yellowSword.transform)
                            child.gameObject.SetActive(false);
                        foreach (Transform child in redSword.transform)
                            child.gameObject.SetActive(false);
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha2)&&hasRed)
                    {

                        if (blueInUse == false)
                            blueSword.GetComponent<SpriteRenderer>().enabled = false;
                        redSword.GetComponent<SpriteRenderer>().enabled = true;
                        if (yellowInUse == false)
                            yellowSword.GetComponent<SpriteRenderer>().enabled = false;
                        currentlyHeld = 2;
                        foreach (Transform child in blueSword.transform)
                            child.gameObject.SetActive(false);
                        foreach (Transform child in yellowSword.transform)
                            child.gameObject.SetActive(false);
                        foreach (Transform child in redSword.transform)
                            child.gameObject.SetActive(true);
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha3)&&hasYellow)
                    {
                        if (blueInUse == false)
                            blueSword.GetComponent<SpriteRenderer>().enabled = false;
                        if (redInUse == false)
                            redSword.GetComponent<SpriteRenderer>().enabled = false;
                        yellowSword.GetComponent<SpriteRenderer>().enabled = true;
                        currentlyHeld = 3;
                        foreach (Transform child in blueSword.transform)
                            child.gameObject.SetActive(false);
                        foreach (Transform child in yellowSword.transform)
                            child.gameObject.SetActive(true);
                        foreach (Transform child in redSword.transform)
                            child.gameObject.SetActive(false);
                    }

                }
            }
        }
        
        
    }
}
