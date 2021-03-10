using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 和VFX代码一样  但是如果添加vfx代码 没反应。。。
/// </summary>
public class 螺旋菌碰撞 : MonoBehaviour
{
    Renderer[] father;
    public bool isDead;

    private void Start()
    {
        father = GetComponentsInChildren<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            foreach (var child in father)
            {
                print("子弹击中");
                child.material.SetFloat("_BurnScale", (100) * 0.11f);
            }
        }
    }

    private void Update()
    {
        if (this.GetComponentInChildren<SpriteRenderer>().material.GetFloat("_BurnScale") >= 10)
        {
            Destroy(gameObject);
            isDead = true;
            print(isDead);
        }
    }
}
