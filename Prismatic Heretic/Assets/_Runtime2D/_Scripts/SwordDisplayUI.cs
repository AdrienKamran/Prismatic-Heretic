using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordDisplayUI : MonoBehaviour
{
    public Image blueSword;
    public RawImage blueParticle;
    public Color blueColor;

    public Image redSword;
    public RawImage redParticle;
    public Color redColor;

    public Image yellowSword;
    public RawImage yellowParticle;
    public Color yellowColor;

    public GameObject PlayerCheck;
    private SelectSword swordCheck;

    // Start is called before the first frame update
    void Start()
    {
        blueColor = blueSword.color;
        redColor = redSword.color;
        yellowColor = yellowSword.color;

        swordCheck = PlayerCheck.GetComponent<SelectSword>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (swordCheck.blueInUse)
        {
            blueSword.color = new Color(0.2f, 0.2f, 0.2f);
            blueParticle.enabled = false;
        }
        else
        {
            blueSword.color = blueColor;
            blueParticle.enabled = true;
        }

        if (swordCheck.redInUse)
        {
            redSword.color = new Color(0.2f, 0.2f, 0.2f);
            redParticle.enabled = false;
        }
        else
        {
            redSword.color = redColor;
            redParticle.enabled = true;
        }

        if (swordCheck.yellowInUse)
        {
            yellowSword.color = new Color(0.2f, 0.2f, 0.2f);
            yellowParticle.enabled = false;
        }
        else
        {
            yellowSword.color = yellowColor;
            yellowParticle.enabled = true;
        }
    }
}
