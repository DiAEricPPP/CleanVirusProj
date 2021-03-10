using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBase : MonoBehaviour
{
    //实现了设定个体血量/自身可以攻击的话攻击的伤害值/自身有护盾减伤的话护盾的减伤效率
    //封装了个体血量/攻击伤害值/是否有护盾
    //

    //[Range(0f, 20f)]
    //public float specialAttackDamage = 0f;//可战斗单位，用特殊攻击方式的基础伤害，以子弹/粒子做攻击的为
    /* private bool setNameDone = false;
    private string enemyName = "NotEnemy";
    public string EnemyName
    {
        get
        {
            return enemyName;
        }
    } */
    private bool setHPDone = false;
    private float hp = 100f;
    public float HP
    {
        get
        {
            return hp;
        }
    }
    /* private float baseDamage = 0f;
    public float attackDamage
    {//设置、获取可战斗单位的伤害值
        get
        {
            return baseDamage;
        }
        set
        {
            baseDamage = value;
            Mathf.Clamp(baseDamage, 0f, 20f);
        }
    } */
    private bool shield = false;

    public bool useShield
    {//默认false，不使用护盾，如果该单位不使用护盾，则可以忽略此属性，使用护盾的话，要在其他脚本设为true
        get
        {
            return shield;
        }
        set
        {
            shield = value;
        }
    }

    private float multiper = 1f;//如果有伤害减少属性（病毒外壳减伤等），计算受到伤害时应用}
    public float damageMultiper
    {
        get
        {
            return multiper;
        }
        set
        {
            multiper = value;
            Mathf.Clamp01(multiper);
        }
    }

    /* void Start()
    {
    } */

    public void SetHP(float value)
    {//初始化角色HP，仅可使用一次
        if (!setHPDone)
        {
            hp = value;
            Mathf.Clamp(hp, 0, 100f);
            setHPDone = true;
        }
    }
    public void TakeDamage(float damage)
    {
        if (!shield)
        {
            hp -= (damage);
        }
        else
        {
            hp -= (damage * damageMultiper);
        }

        if (hp < 0f)
        {
            hp = 0f;
        }
    }

    private void Update()
    {
        if (hp <= 0 && gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            BattleControl.Instance.EnemyDie(gameObject.name);
        }
    }
}
