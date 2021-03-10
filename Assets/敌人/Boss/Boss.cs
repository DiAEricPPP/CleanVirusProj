using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    //第一个功能：隔一会儿把触手伸出去
    //第二个功能：发射炮弹
    //第三个功能：

    public GameObject go;
    private SpriteRenderer[] sp;
    private BoxCollider2D[] bc;
    public float moveSpeed = 5;
    private int i = 0;
    private int coldTime = 50;
    int startTime;
    bool isLong = true;
    bool isShort = false;
    public float randomLength;

    void Start()
    {
        sp = go.GetComponentsInChildren<SpriteRenderer>();
        bc = go.GetComponentsInChildren<BoxCollider2D>();
        startTime = 0;
        randomLength = Random.Range(10, 30);
    }

    private void Update()
    {
        if (isLong)
        {
            moveSpeed = 5;
            if (sp[ 0 ].size.y >= randomLength)
            {
                isLong = false;
                isShort = true;
            }
        }
        if (isShort)
        {
            moveSpeed = -5;
            if (sp[ 0 ].size.y <= 3)
            {
                isLong = true;
                isShort = false;
                randomLength = Random.Range(5, 15);
            }
        }

        for (i = 0; i < 5; i++)
        {
            sp[i].size += new Vector2(0, Time.deltaTime) * moveSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.gameObject.name);
        if (collision.gameObject.tag == "Player")
        {
            print("1");
            
        }
    }
}
