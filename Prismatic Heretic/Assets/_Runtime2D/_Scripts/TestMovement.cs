using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    Rigidbody2D body;
    // Start is called before the first frame update
    void Start()
    {
        body = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        body.velocity = new Vector2(Mathf.Sin(Time.time), Mathf.Cos(Time.time));
    }
}
