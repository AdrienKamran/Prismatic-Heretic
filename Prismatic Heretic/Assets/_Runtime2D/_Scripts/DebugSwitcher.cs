using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSwitcher : MonoBehaviour
{
    public GameObject DebugLevelSwitcher;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            DebugLevelSwitcher.SetActive(!DebugLevelSwitcher.activeSelf);
        }
    }
}
