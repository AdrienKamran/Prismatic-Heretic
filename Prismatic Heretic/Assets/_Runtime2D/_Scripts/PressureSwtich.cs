using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureSwtich : MonoBehaviour
{
    public Sprite switchOnSprite;
    public Sprite switchOffSprite;
    public GameObject linkedObject;
    public GameObject[] linkedSwitches;
    private SpriteRenderer sr;
    public bool switched;
    // Start is called before the first frame update
    void Start()
    {
        switched = false;
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Beam"))
        {
            if (!switched)
            {
                //switched = !switched;
                switched = true;
                sr.sprite = switchOnSprite;
                if (linkedObject.CompareTag("TriggerDoor"))
                {
                    linkedObject.SetActive(false);
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Beam"))
        {
            if (!switched)
            {
                //switched = !switched;
                switched = true;
                sr.sprite = switchOnSprite;
                if (linkedObject.CompareTag("TriggerDoor"))
                {
                    linkedObject.SetActive(false);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Beam"))
        {
            if (switched)
            {
                switched = false;
                sr.sprite = switchOffSprite;
                if (!AnySwitched())
                {
                    if (linkedObject.CompareTag("TriggerDoor"))
                    {
                        linkedObject.SetActive(true);
                    }
                }
            }
        }
    }

    public bool AnySwitched()
    {
        bool any = false;
        if (linkedSwitches != null && linkedSwitches.Length != 0)
        {
            foreach (GameObject go in linkedSwitches)
            {
                if (go.GetComponent<PressureSwtich>().switched == true)
                {
                    any = true;
                }
            }
        }
        return any;
    }
}
