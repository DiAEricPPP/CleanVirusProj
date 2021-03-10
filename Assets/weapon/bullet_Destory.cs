using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_Destory : MonoBehaviour
{
    public float life_time;
    
    void Start()
    {
        Invoke("DestoryBullet", life_time);
    }

    void DestoryBullet()
    {
        Destroy(gameObject);
    }
}
