using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 螺旋菌 : MonoBehaviour
{
    public GameObject particle;
    public GameObject body;

    private 螺旋菌碰撞 vfx;

 //   private Material material;
    public float life_time;
    private Color color;

    public float speed;
    public int change_speed = 5;
    float result;

    public Transform ship;

    void Start()
    {
      //  speed = 1;
        particle.SetActive(false);
        life_time = 1.0f;
  //      material = GetComponentInChildren<SpriteRenderer>().material;
  //      color = material.color;
        vfx = GetComponentInChildren<螺旋菌碰撞>();
    }

    // 实例化
    // 冲刺一段
    // 3s变成红色
    // 爆炸

    void attack()
    {
        //transform.LookAt(ship);
        Vector3 look_at = ship.position - transform.position;
        body.transform.up = Vector3.Normalize(look_at);

        transform.Translate(Vector3.Normalize(look_at) * speed);
        if( look_at.sqrMagnitude < 1000 && look_at.sqrMagnitude >200)
        {
            transform.Translate(Vector3.Normalize(look_at) * speed * 3);

        }
        else if (look_at.sqrMagnitude < 200)
        {
            speed = 0.01f;
 //           StartCoroutine(Change());
            Invoke("Boom_e_s",1f);
        }
    }

/*IEnumerator Change()
    {
        float delta = (1 - 0) / change_speed;   //delta为速度，每次加的数大小

        result = 0;

        for (float i = 0; i < change_speed; i++)
        {
            result += delta;
            yield return new WaitForSeconds(0.1f);     //每 0.1s 加一次
            material.SetColor("_Color", Color.LerpUnclamped(color, Color.red, i));
        }
        StopCoroutine(Change());
    }*/

    public void Boom_e_s()
    {
        particle.SetActive(true);
        Invoke("destoryGameObject", 0.5f); 
    }

    void destoryGameObject()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        attack();

      if (vfx.isDead)
        {
            Boom_e_s();
            print("击中了螺旋菌");
        }
    }
}
