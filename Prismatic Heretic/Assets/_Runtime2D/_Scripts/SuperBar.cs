using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[ExecuteInEditMode()]
public class SuperBar : MonoBehaviour
{
    public int minimum;
    public int maximum;
    public int current;
    public Image mask;
    public Image fill;
    public Color color;
    public bool BluePower;
    public bool RedPower;
    public bool YellowPower;

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }

    void GetCurrentFill()
    {
        if (BluePower)
        {
            current = Player.bluePowerDelay;
        }
        if (RedPower)
        {
            current = Player.redPowerDelay;
        }
        if (YellowPower)
        {
            current = Player.yellowPowerDelay;
        }
        float currentOffset = current - minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = currentOffset / maximumOffset;
        mask.fillAmount = Mathf.Clamp(fillAmount, 0, 1);
    }
}
