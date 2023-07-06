using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    public GameObject player;
    private Vector3 vPos;
    public float distance=0.5f;
    public float angle;

    //set in the editor
    public bool isRed = false;  
    public bool isBlue = false;
    public bool isYellow = false;

    public WarpSword warp;
    public AttackAOE red; //"AttackAOE" should recieve a different name, since it basically refers to everything pertaining to red more or less
                   //(it can be thought of as the red equivalent of the WarpSword script)
    public ParalyzerSword yellow;

    // Update is called once per frame
    void Update()
    {
        currentlyHeld();
    }

    //These checks are necessary to ensure that a given sword is not rotating around the player when it shouldn't be
    void currentlyHeld() {
        if (Player.inDialog == false)
        {
        if (isBlue == true && warp.warping == false && warp.prepped == false)
            applyRotate();
        if (isRed == true && red.prepped == false)
            applyRotate();
        if (isYellow == true && yellow.shooting == false && yellow.prepped == false)
            applyRotate();
        }
    }

    void applyRotate()
    {
        if (!Player.GameIsPaused)
        {
            //Grab mouse position
            vPos = Input.mousePosition;
            //Relation to Camera and player position
            vPos.z = (player.transform.position.z - Camera.main.transform.position.z);
            vPos = Camera.main.ScreenToWorldPoint(vPos);
            vPos = vPos - player.transform.position;
            //Calculated angle
            angle = Mathf.Atan2(vPos.y, vPos.x) * Mathf.Rad2Deg;
            //Reset angle if under 0
            if (angle < 0)
            {
                angle = angle + 360;
            }
            transform.localEulerAngles = new Vector3(0, 0, angle);
            //Calculate position of sword related to angle and distance
            float xPos = Mathf.Cos(Mathf.Deg2Rad * angle) * distance;
            float yPos = (Mathf.Sin(Mathf.Deg2Rad * angle) * distance) * 1.5f;
            //Place sword via calculated position in relation to the player

            float dist = Vector3.Distance(player.transform.position, this.transform.position);
            if (dist < 5)
            {
                transform.position = new Vector3(player.transform.position.x + xPos * 4, player.transform.position.y + yPos * 4, 0);
            }
            else
            {
                transform.position = new Vector3(Mathf.Lerp(this.transform.position.x, player.transform.position.x + xPos * 4, Time.fixedDeltaTime * 5), Mathf.Lerp(this.transform.position.y, player.transform.position.y + yPos * 4, Time.fixedDeltaTime * 5), 0);
            }

            //transform.localPosition = new Vector3(player.transform.position.x + xPos * 4, player.transform.position.y + yPos * 4, 0);

        }

    }
}
