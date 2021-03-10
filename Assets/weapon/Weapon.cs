using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    float Joystick_Left_x;
    float Joystick_Left_y;

    public float speed;

    /// <summary>
    /// 所有均没有设置进入控制室的条件、、
    /// </summary>

    //Weapon
    public Transform[] shoot_position; //三个发射位置 三个空的预制体 为了获取位置
    public Transform battery_up; // 会转动的炮筒(上炮)
    public Transform battery_left; // 会转动的炮筒(上炮)
    public Transform battery_right; // 会转动的炮筒(上炮)

    public GameObject bullet;  //预制体子弹
    public float cold_time;  //冷却时间

    public GameObject Lightning; //闪电预制体
    public Transform versatile_Weapon; //多功能炮台位置
    public Transform versatile_Weapon_shoot_pos;

    Vector2 current_joystick_postion;

    //EatGremWeapon
    public Transform EatGremBody;  //噬菌体的身子
    public Transform EatGremHead;  //噬菌体的头
    public float ropeLength = 0.0f;  //身子伸长量
    public enum RopeState  //定义了三个状态 因为单纯修改绳子的localScale会出现bug
    {
        idle, elongation, shorten
    }
    private RopeState ropeState; //声明状态

    void Start()
    {
        speed = 1.0f;
        cold_time = 1.5f;
    }


    void Update()
    {
        Joystick_Left_x = Input.GetAxis("Horizontal");
        Joystick_Left_y = Input.GetAxis("Vertical");


        PrimaryWeapon();

        EatGermWeapon();

        versatileWeapon();

    }

    void PrimaryWeapon()   // 你的moveCannonUp函数里有角度限制，这里我主要是冷却时间、发射的子弹交互有专门的脚本，我存成预制体了
    {
        current_joystick_postion = new Vector2(Joystick_Left_x, Joystick_Left_y);
        Vector2 intermediate_joystick_postion = Vector2.Lerp(battery_up.up, current_joystick_postion, Time.deltaTime * speed);
        battery_up.up = intermediate_joystick_postion;


        if (Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            cold_time = 1.5f;
        }
        if (Input.GetKey(KeyCode.Joystick1Button2))
        {
            cold_time += 0.1f;
            if (cold_time > 1.5f)
            {
                Instantiate(bullet, shoot_position[0].position, shoot_position[0].rotation); // 这里有两个预制体，一个是子弹的、一个是子弹发射位置，是个空物体，我每个放在了炮筒的炮口位置。
                cold_time = 0.0f;
            }
        }


    }

    void versatileWeapon()    //同基础炮台、这里主要是冷却时间、闪电的预制体我已经保存好。
    {

        current_joystick_postion = new Vector2(Joystick_Left_x, Joystick_Left_y);
        Vector2 intermediate_joystick_postion = Vector2.Lerp(versatile_Weapon.up, current_joystick_postion, Time.deltaTime * speed);
        versatile_Weapon.up = intermediate_joystick_postion;


        if (Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            cold_time = 5f;
        }
        if (Input.GetKey(KeyCode.Joystick1Button3))
        {

            cold_time += 0.1f;
            if (cold_time > 5f)
            {
                Instantiate(Lightning, versatile_Weapon_shoot_pos.position, versatile_Weapon_shoot_pos.rotation);  //这里需要闪电预制体、多功能炮台的位置
                print("发射闪电");
                cold_time = 0.0f;
            }
        }
    }

    void EatGermWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button4))   //这里测试用的鼠标 忽略
        {
            ropeState = RopeState.elongation;
        }


        if (ropeState == RopeState.idle)
        {
            EatGremBody.localScale = new Vector3(EatGremBody.transform.localScale.x, 0, EatGremBody.transform.localScale.z);
        }
        else if (ropeState == RopeState.elongation)
        {
            Elongation();
        }
        else if (ropeState == RopeState.shorten)
        {
            Shorten();
        }

    }

    void Elongation()  //伸长函数 单纯用localscale 会有bug 这个是解决的办法   
    {
        if (ropeLength >= 10f)
        {
            ropeState = RopeState.shorten;
            return;
        }
        print("!!!!");
        ropeLength += Time.deltaTime * 20;
        EatGremBody.localScale = new Vector3(EatGremBody.transform.localScale.x, ropeLength, EatGremBody.transform.localScale.z);
        EatGremHead.localScale = new Vector3(EatGremHead.transform.localScale.x, 1 / ropeLength, EatGremHead.transform.localScale.z);
    }

    void Shorten()
    {
        if (ropeLength <= 1f)
        {
            ropeState = RopeState.idle;
            return;
        }
        ropeLength -= Time.deltaTime * 20;
        EatGremBody.localScale = new Vector3(EatGremBody.transform.localScale.x, ropeLength, EatGremBody.transform.localScale.z);
        EatGremHead.localScale = new Vector3(EatGremHead.transform.localScale.x, 1 / ropeLength, EatGremHead.transform.localScale.z);
    }


}

