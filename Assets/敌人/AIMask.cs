using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMask : MonoBehaviour
{
    public GameObject Area_All;

    public float find_radius = 80;

    void Start()
    {

    }
    void Update()
    {
        foreach (Transform child in Area_All.GetComponentInChildren<Transform>())
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(child.position.x, child.position.y)) > find_radius)
            {
                child.gameObject.SetActive(false);
            }
            else if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(child.position.x, child.position.y)) < find_radius)
            {
                child.gameObject.SetActive(true);
            }
        }

    }
}
