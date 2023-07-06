using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Orb : MonoBehaviour
{
    Vector2 getPosition() { return new Vector2(this.transform.position.x, this.transform.position.y); }  
    void setPosition(Vector2 position) { this.transform.position = new Vector3(position.x, position.y, 0); }
}
