using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AllParticleCollision : MonoBehaviour
{
    public float lifeTime;
    public float damage;
    //public LayerMask layerMask;
    //private List<ParticleCollisionEvent> collisionEvents;
    private ParticleSystem ps;
    private int enemyLayer;
    private int allyLayer;
    BattleControl battleControl;


    void Start()
    {
        Destroy(gameObject, lifeTime);
        //collisionEvents = new List<ParticleCollisionEvent>();
        ps = GetComponent<ParticleSystem>();
        enemyLayer = LayerMask.NameToLayer("Enemy");
        allyLayer = LayerMask.NameToLayer("Ally");
        battleControl = BattleControl.Instance;
        //bb=GetComponent<BattleBase>();
    }

    void OnParticleCollision(GameObject other)
    {
        //Debug.Log("Suc");
        if (other.tag == "Bullet")
        {
            Destroy(other);
            return;
        }
        battleControl.DamageTo(other, damage);
    }
}
