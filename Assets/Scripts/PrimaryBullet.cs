using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class PrimaryBullet : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float damage;
    public GameObject boom;
    protected Rigidbody2D rb;
    private BattleControl bc;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        rb = GetComponent<Rigidbody2D>();
        bc = BattleControl.Instance;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + (Vector2)transform.TransformVector(Vector2.up * speed * Time.deltaTime));
    }



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            bc.DamageTo(collision.gameObject, damage);
        }
        Destroy(gameObject);
        Instantiate(boom, transform.position, Quaternion.identity);
    }
}
