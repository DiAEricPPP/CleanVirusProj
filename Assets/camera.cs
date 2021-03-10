using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public Rigidbody2D ship;

    // Update is called once per frame

    void Update()
    {
        transform.position = new Vector3(ship.position.x, ship.position.y, transform.position.z);
    }
}
