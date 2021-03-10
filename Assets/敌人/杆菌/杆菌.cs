using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 杆菌 : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //如果检测到敌人，就进入攻击动画
        if (Input.GetMouseButton(0))
        {
            anim.SetBool("isAttack", true);
        }
        else
        {
            anim.SetBool("isAttack", false);
        }

    }



}
