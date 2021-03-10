using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MissleBullet : PrimaryBullet
{

    public string[] enemyTag;//接受enemy的所有类型tags
    [Range(0f, 1f)]
    public float missleStartUpTime;//表示导弹起始固定时间内向上飞行（之后开始跟踪）
    //public GameObject test;
    public float nextWaypointDistance = 3f;//距离下一路径点的距离判断阈值
    [Range(1f, 50f)]
    public float missleRotateSpeed = 10f;//导弹转向目标的速度
    public float startNewPathTime = 0.5f;//如果要持续重新计算path的最小间隔时间
    //private Rigidbody2D rb;
    private float calStartNewPathTime = 0f;
    //public Transform missleSprite;
    private Path path;
    private int currentWaypoint = 0;
    private bool reachEndOfPath = false;
    private Vector3 lastFrameTargetPosition;//计算上一帧目标位置，减少运算

    private Vector3 noTargetPosition = new Vector3(0, 0, 77);//初始化时/目标不存在时【上一帧目标位置】的值
    private Seeker seeker;
    private GameObject target;
    private float calTime = 0f;
    private int allyLayer;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        lastFrameTargetPosition = noTargetPosition;
        seeker = GetComponent<Seeker>();
        calStartNewPathTime = startNewPathTime;
        rb = GetComponent<Rigidbody2D>();
        allyLayer = LayerMask.NameToLayer("Ally");
    }
    void FixedUpdate()
    {
        /* if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            Destroy(test);
        }
        FindTarget(); */
        Physics2D.IgnoreLayerCollision(allyLayer, allyLayer);
        HitTheTarget();
    }

    void FindTarget()
    {
        if (this.target == null)
        {
            foreach (string s in enemyTag)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag(s);
                if (enemies.Length > 0)
                {
                    foreach (GameObject go in enemies)
                    {
                        Vector3 posOnScreen = Camera.main.WorldToViewportPoint(go.transform.position);
                        if (posOnScreen.x >= 0 && posOnScreen.x <= 1 && posOnScreen.y >= 0 && posOnScreen.y <= 1)
                        {
                            this.target = go;
                            return;
                        }
                    }
                }
            }
        }
        //Debug.Log(this.target);
    }

    void HitTheTarget()
    {
        calStartNewPathTime += Time.deltaTime;
        if (calTime < missleStartUpTime)
        {
            calTime += Time.deltaTime;
            //transform.Translate(Vector3.up * speed * Time.deltaTime);
            //rb.MovePosition(rb.position + (Vector2)(Quaternion.AngleAxis(rb.rotation, Vector2.up) * Vector2.up) * speed * Time.deltaTime);
            //rb.AddForce(Vector2.up * speed * Time.deltaTime);
            rb.MovePosition(rb.position + (Vector2)transform.TransformVector(Vector2.up * speed * Time.deltaTime));

        }
        else
        {
            FindTarget();

            if (target == null)
            {
                //transform.Translate(Vector3.up * speed * Time.deltaTime);
                //rb.MovePosition(rb.position + Vector2.up * speed * Time.deltaTime);
                //rb.MovePosition(rb.position + (Vector2)transform.rotation.eulerAngles * speed * Time.deltaTime);
                rb.MovePosition(rb.position + (Vector2)transform.TransformVector(Vector2.up * speed * Time.deltaTime));

            }
            else
            {
                if (lastFrameTargetPosition != target.transform.position && seeker.IsDone() && calStartNewPathTime > startNewPathTime)
                {
                    //Debug.Log("Suc");
                    //transform.hasChanged;
                    seeker.StartPath(transform.position, target.transform.position, OnPathFound);
                }

                if (path != null && !reachEndOfPath)
                {
                    Vector2 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
                    //transform.Translate(dir * speed * Time.deltaTime, Space.World);
                    rb.MovePosition(rb.position + dir * speed * Time.deltaTime);
                    //rb.MovePosition(rb.position + (Vector2)(Quaternion.AngleAxis(rb.rotation, Vector2.up) * Vector2.up) * speed * Time.deltaTime);

                    float distance = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
                    if (distance < nextWaypointDistance)
                    {
                        currentWaypoint++;
                        if (currentWaypoint >= path.vectorPath.Count)
                        {
                            reachEndOfPath = true;
                            //OnEndOfPath();
                            //Destroy(gameObject);
                        }
                        else
                        {
                            reachEndOfPath = false;
                        }
                        //暂时用transform方法转向，rigidbody2d方法出现问题
                        Vector2 direction = (target.transform.position - transform.position).normalized;
                        Quaternion rot = Quaternion.FromToRotation(transform.up, direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rot * transform.rotation,
                                                            Time.deltaTime * missleRotateSpeed);
                        /* float angle = Vector2.Angle(Vector2.up, direction);
                        rb.MoveRotation(Mathf.LerpAngle(rb.rotation, angle, Time.deltaTime * missleRotateSpeed * 5)); */
                    }

                }

            }
        }
        if (target != null)
        {
            lastFrameTargetPosition = target.transform.position;
        }
        else
        {
            lastFrameTargetPosition = Vector2.zero;
        }
    }

    /* void LateUpdate()
    {
        if (target != null)
        {
            lastFrameTargetPosition = target.transform.position;
        }
        else
        {
            lastFrameTargetPosition = Vector2.zero;
        }
    } */


    void OnPathFound(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            reachEndOfPath = false;
            calStartNewPathTime = 0f;
        }
    }

    /* void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "E_S")
        {
            // SendMessage("boom_e_s");
            GameObject.Find("螺旋菌").SendMessageUpwards("boom_e_s");

        }
        Destroy(gameObject);
        Destroy(collision.gameObject);
        
    } */
    /* 
        void OnEndOfPath()
        {
            Destroy(gameObject);
            Instantiate(boom, transform.position, Quaternion.identity);
        } */
}
