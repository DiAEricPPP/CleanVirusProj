using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBar : MonoBehaviour
{
    // 此脚本为示意脚本 主要展示实现 未整合

    private SpriteRenderer sp;
    private float height = 1.52f;
    private float width = 1.1f;
    // public float width = 0;
    [Range(0,4)]
    public int useOnce = 4;
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        sp.size = new Vector2(width * useOnce, height);
        if (Input.GetKeyDown(KeyCode.Space) && useOnce > 0)
        {
            useOnce -= 1;
        }
    }
}
