using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    BattleBase bb;
    Renderer[] father;
    public bool isDead;


    void Start()
    {
        bb = GetComponent<BattleBase>();
        father = GetComponentsInChildren<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Bullet" && collision.gameObject.layer != 9 && collision.gameObject.layer == 11)
        {
            foreach (var child in father)
            {
                print("子弹击中");
                child.material.SetFloat("_BurnScale", (100 - bb.HP) * 0.5f);
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if(other.tag == "Bullet" && other.layer != 9)
        {
            foreach (var child in father)
            {
                print("粒子击中");
                child.material.SetFloat("_BurnScale", (100 - bb.HP) * 0.11f);
            }
        }
    }

    private void Update()
    {
        if(this.GetComponentInChildren<SpriteRenderer>().material.GetFloat("_BurnScale") >= 10)
        {
            Destroy(gameObject);
           isDead = true;
           print(isDead);
        }
    }
}
