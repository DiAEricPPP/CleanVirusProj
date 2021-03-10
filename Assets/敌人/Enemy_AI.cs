using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy_AI : MonoBehaviour
{
    //这个是每个AI都装的基础脚本， 内涵寻路，寻到路后进入玩家视野之后的行为
    public Transform target;
    public float speed = 200.0f;
    public float nextWaypointDistance = 3f;

    Path path;
    int currentWayPoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    //    public PhysicsMaterial2D pm;

    Vector2 playerEnemyDirection;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        //     pm = GetComponent<PhysicMaterial>();
        InvokeRepeating("UpdatePath", 0f, 5f);

        //     pm.bounciness = 0.0f;
    }

    Vector3 RandomTargrtPos(Vector2 targrtPos)
    {
        playerEnemyDirection = (rb.position - targrtPos);
        float x = playerEnemyDirection.x > 0 ? 1 : -1;
        float y = playerEnemyDirection.y > 0 ? 1 : -1;
        //   print(x + " " + y);
        Vector2 randomTargetPos = targrtPos + new Vector2(Random.Range(10, 40) * x, Random.Range(10, 40) * y);
        //   playerEnemyDirection.x > 0 ? 1 : -1, Random.Range(-50, 50) * playerEnemyDirection.y > 0 ? 1 : -1);
        if (playerEnemyDirection.magnitude < 100)
        {
            randomTargetPos = targrtPos;
        }

        return randomTargetPos;
    }


    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            Vector3 targetPoint = RandomTargrtPos(target.position);

            seeker.StartPath(rb.position, targetPoint, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }

        if (currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        if (playerEnemyDirection.magnitude > 35)
        {
            rb.velocity = force;
        }
        else
        {
            rb.velocity = force * 0.1f;
            //      rb.gameObject.transform.RotateAround(target.position, new Vector3(0,0,1), 3);
        }

        //     print(playerEnemyDirection.magnitude);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWaypointDistance)
        {
            currentWayPoint++;
        }
    }

    //转圈转行为，达到一定距离，开始绕着转

    //

    //目前是敌人碰到任何物体都回弹 应该是碰到ship才回弹
    /*    private void OnCollisionEnter2D(Collision2D collision)
        {
           if(collision.gameObject.tag == "Ship")
            {
                print("撞到了");
                pm.bounciness = 3.0f;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Ship")
            {
                print("弹走了");
                pm.bounciness = 0.0f;
            }
        }*/



}