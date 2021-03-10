using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{
    //private BattleBase shipBase;
    public Rigidbody2D shipRigidbody;
    public Transform players;
    public Transform wallAndFloor;
    public Transform pusher;
    public Transform cannonUp;
    public Transform cannonLeft;
    public Transform cannonRight;
    public Transform[] cannonShootPos;
    public GameObject bullet;
    public Transform specCannon;
    public GameObject specCannonTerminal;
    public Transform specShootPos;
    public GameObject missle;
    public GameObject thunder;
    public GameObject laserBlade;
    private GameObject laser = null;
    public Transform shield;
    public SpriteRenderer shieldBarFill;
    [SerializeField]
    private const float ShieldBarDefaultSizeX = 4.21f;
    [SerializeField]
    private const float ShieldBarDefaultSizeY = 1.52f;
    private bool shieldActive = false;//护盾是否初次启用
    private bool shieldRechargeable = false;
    //private bool shieldCanProtect = true;//护盾启用后是否能阻挡伤害（电量未空）
    //public BattleBase shieldBase;
    [Range(10f, 90f)]
    public float shieldLastTime;
    private float calShieldLastTime;
    public Transform labTerminal;
    public Text labQTEText;
    public Vector2 labQTETextOffset = new Vector2(0, 0);
    private bool labQTEStart = false;
    private string labQTEString;
    private int QTEProgress = 0;
    private Dictionary<int, string> crossKey = new Dictionary<int, string>();
    public Transform hook;
    public GameObject hookHead;
    public TerminalCheck terminsalCheck;

    private Vector2 pushVector;
    private Vector2 cannonUpStartDir;//记录初始炮台up轴朝向
    private Vector2 cannonLeftStartDir;
    private Vector2 cannonRightStartDir;
    private Vector2 hookDefaultSize; //记录默认hook长度（size）
    private const float CannonLeftStartZ = 116f;//左右两个炮台的倾斜角度
    [Range(0f, 1f)]
    public float pusherRotateSpeed = 1.0f;//给定推进器旋转速度(0--1)
    [Range(0f, 1f)]
    public float cannonRotateSpeed = 1.0f;
    [Range(0f, 180f)]
    public float cannonRotateLimit = 90f;
    [Range(0f, 10f)]
    public float specCannonRotateSpeed, shieldRotateSpeed, hookRotateSpeed, hookShootSpeed, hookShootMaxTime = 1.0f;
    [Range(1f, 20f)]
    public float pushSpeed = 1.0f;//给定标准推进速度(1--10)
    [Range(1f, 10f)]
    public float speedBuffer = 5f;//给定推进速度缓冲值(1--10)
    private float speed = 0f;//计算推进速度中间值
    [Range(1f, 20f)]
    public float cannonShootColdTime, specShootColdTime;//这两个冷却是相应的武器每两发之间的冷却
    private float calCannonShootColdTime, calSpecShootColdTime;
    //Dictionary<string, int> ShipLevel = new Dictionary<string, int>();
    private const string CannonLevel = "CannonLevel";
    private const string SpecCannonLevel = "SpecCannonLevel";
    private float specWeaponShootCount = 0;
    [Range(1, 20)]
    public int specWeaponAmmoCount;//特殊武器弹药量（如 10发后CD）
    [Range(1f, 20f)]
    public float specAmmoRunOutColdTime;//这个是特殊武器弹药耗尽后的冷却
    float calSpecAmmoRunOutColdTime;

    private bool specWeaponAmmoColdDown = false;//bool 判断特殊武器是否在冷却中，默认false未在冷却
    public Transform specWeaponTerminal;
    //public RectTransform specWeaponAmmoText;
    public Text specWeaponAmmoText;
    public Vector2 specWeaponTextOffset = new Vector2(31, 10);

    public enum WeaponState//玩家的操作和武器状态（空闲，初发，持续）
    {
        Idle, First, Busy
    }
    private WeaponState hookState;//public出来，方便之后写敌人交互碰撞判断hook是否出去


    //private int specCannonLevel = 2;
    //private bool canChangeSpecWeaponLevel = true;
    void Start()
    {
        //shipRigidbody = GetComponent<Rigidbody2D>();
        cannonUp.SetPositionAndRotation(cannonUp.position, Quaternion.Euler(0, 0, 0));
        cannonLeft.SetPositionAndRotation(cannonLeft.position, Quaternion.Euler(0, 0, CannonLeftStartZ));
        cannonRight.SetPositionAndRotation(cannonRight.position, Quaternion.Euler(0, 0, -CannonLeftStartZ));
        cannonUpStartDir = new Vector2(0, 1);
        cannonLeftStartDir = Quaternion.AngleAxis(CannonLeftStartZ, Vector3.forward) * cannonUpStartDir;
        cannonRightStartDir = Quaternion.AngleAxis(-CannonLeftStartZ, Vector3.forward) * cannonUpStartDir;

        pushVector = new Vector2(-pusher.up.x, -pusher.up.y);
        pusherRotateSpeed = Mathf.Clamp01(pusherRotateSpeed);
        cannonRotateSpeed = Mathf.Clamp01(cannonRotateSpeed);
        specCannonRotateSpeed = Mathf.Clamp01(specCannonRotateSpeed);
        shieldRotateSpeed = Mathf.Clamp01(shieldRotateSpeed);
        hookRotateSpeed = Mathf.Clamp01(hookRotateSpeed);
        pushSpeed = Mathf.Clamp(pushSpeed, 1f, 20f);
        //calCannonShootColdTime = cannonShootColdTime;
        hookState = WeaponState.Idle;
        hookDefaultSize = hookHead.GetComponent<SpriteRenderer>().size;

        /* ShipLevel.Add("Cannon", 1);
        ShipLevel.Add("SpecCannon", 1); */
        if (!PlayerPrefs.HasKey(CannonLevel) || !PlayerPrefs.HasKey(SpecCannonLevel))
        {
            PlayerPrefs.SetInt(CannonLevel, 1);
            PlayerPrefs.SetInt(SpecCannonLevel, 1);
        }
        SpecWeaponTextUpdate(specWeaponAmmoText, specWeaponAmmoColdDown);
        shieldBarFill.size = new Vector2(ShieldBarDefaultSizeX, ShieldBarDefaultSizeY);
        labQTEText.enabled = false;
        labQTEStart = false;
        crossKey.Add(1, "↑");
        crossKey.Add(2, "↓");
        crossKey.Add(3, "←");
        crossKey.Add(4, "→");
    }

    // Update is called once per frame
    void Update()
    {
        /* if (Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            ShipUpdate("Missle");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button5))
        {

        } */
        /* if (shipRigidbody.transform.hasChanged)
        {
            wallAndFloor.position = shipRigidbody.position;
            players.position = shipRigidbody.position;

            Vector2 specScreenPos = Camera.main.WorldToScreenPoint(specWeaponTerminal.position);
            //specWeaponAmmoText.position = specScreenPos + new Vector2(30, 20);
            specWeaponAmmoText.rectTransform.position = specScreenPos + specWeaponTextOffset;
            Vector2 labScreenPos = Camera.main.WorldToScreenPoint(labTerminal.position);
            labQTEText.rectTransform.position = labScreenPos + labQTETextOffset;
        } */
        wallAndFloor.position = shipRigidbody.position;
        players.position = shipRigidbody.position;

        Vector2 specScreenPos = Camera.main.WorldToScreenPoint(specWeaponTerminal.position);
        //specWeaponAmmoText.position = specScreenPos + new Vector2(30, 20);
        specWeaponAmmoText.rectTransform.position = specScreenPos + specWeaponTextOffset;
        Vector2 labScreenPos = Camera.main.WorldToScreenPoint(labTerminal.position);
        labQTEText.rectTransform.position = labScreenPos + labQTETextOffset;

        //labQTEText.text="TESEEET";

    }

    void LateUpdate()
    {

    }
    //TODO:将类似的转动方法抽象到一起，采取事件体系
    /* void MoveTransform(Transform t, Vector2 joyDir)
    {
        //t.up = Vector2.Lerp(t.up, joyDir, speed * Time.deltaTime);
        
        
    } */
    public void MovePusher(Vector2 joyDir)
    {
        //pusher.up = Vector2.Lerp(pusher.up, joyDir, pusherRotateSpeed * Time.deltaTime*0.5f);
        Quaternion rot = Quaternion.FromToRotation(pusher.up, joyDir);
        pusher.rotation = Quaternion.Slerp(pusher.rotation, rot * pusher.rotation,
                                            pusherRotateSpeed * Time.deltaTime * 0.5f);
        /* pusher.rotation = Quaternion.Slerp(pusher.rotation, Quaternion.LookRotation(Vector3.forward, joyDir),
        pusherRotateSpeed * Time.deltaTime * 0.5f); */
        pushVector = new Vector2(-pusher.up.x, -pusher.up.y);
    }


    public void Push(WeaponState ws)
    {
        if (ws == WeaponState.First)
        {
            speed = 0f;
        }
        if (ws == WeaponState.Busy)
        {
            pushVector.Normalize();
            //transform.Translate(pushVector * speed * Time.deltaTime);
            shipRigidbody.MovePosition(shipRigidbody.position + pushVector * speed * Time.deltaTime);
            //transform.position = shipRigidbody.position;
            /* wallAndFloor.position = shipRigidbody.position;
            players.position = shipRigidbody.position; */
            speedBuffer = Mathf.Clamp(speedBuffer, 1, 10);
            speed = Mathf.Lerp(speed, pushSpeed, (float)(Time.deltaTime / speedBuffer));
        }

    }

    /* void PushEnd()
    {
        speed = 0f;
    } */

    public void MoveSpecCannon(Vector2 joyDir)
    {
        //canChangeSpecWeaponLevel = false;
        Quaternion rot = Quaternion.FromToRotation(specCannon.up, joyDir);
        specCannon.rotation = Quaternion.Slerp(specCannon.rotation, rot * specCannon.rotation,
                                            specCannonRotateSpeed * Time.deltaTime * 0.5f);
    }

    public void ActivateShield()
    {
        if (!shieldActive)
        {
            shieldActive = true;//此bool控制护盾为第一次玩家开启后一直处于激活状态，即如果从未有玩家使用护盾，则护盾不工作，不减少能量
            StartCoroutine(UsingShield(shieldLastTime, shieldBarFill));
        }
    }
    IEnumerator UsingShield(float maxLastTime, SpriteRenderer shieldBarFill)
    {
        //shieldCanProtect = true;
        shield.GetComponent<PolygonCollider2D>().enabled = true;
        calShieldLastTime = maxLastTime;
        while (calShieldLastTime > 0)
        {
            calShieldLastTime -= Time.deltaTime;
            Mathf.Clamp(calShieldLastTime, 0f, maxLastTime);
            shieldBarFill.size = new Vector2(ShieldBarDefaultSizeX * calShieldLastTime / maxLastTime, ShieldBarDefaultSizeY);
            yield return null;
        }
        //shieldCanProtect = false;
        shield.GetComponent<PolygonCollider2D>().enabled = false;
    }
    public void MoveShield(Vector2 joyDir)
    {
        Quaternion rot = Quaternion.FromToRotation(shield.up, joyDir);
        shield.rotation = Quaternion.Slerp(shield.rotation, rot * shield.rotation,
                                            shieldRotateSpeed * Time.deltaTime * 0.5f);
    }

    public void MoveHook(Vector2 joyDir)
    {
        Quaternion rot = Quaternion.FromToRotation(hook.up, joyDir);
        hook.rotation = Quaternion.Slerp(hook.rotation, rot * hook.rotation,
                                            hookRotateSpeed * Time.deltaTime * 0.5f);
    }

    void MoveCannonUp(Vector2 joyDir)
    {
        Vector2 leftLimit = Quaternion.AngleAxis(cannonRotateLimit, Vector3.forward) * cannonUpStartDir;
        Vector2 rightLimit = Quaternion.AngleAxis(-cannonRotateLimit, Vector3.forward) * cannonUpStartDir;
        float angle = Vector2.SignedAngle(cannonUpStartDir, joyDir);
        if (angle >= cannonRotateLimit)
        {
            joyDir = leftLimit;
        }
        else if (angle <= -cannonRotateLimit)
        {
            joyDir = rightLimit;
        }

        Quaternion rot = Quaternion.FromToRotation(cannonUp.up, joyDir);
        cannonUp.rotation = Quaternion.Slerp(cannonUp.rotation, rot * cannonUp.rotation,
                                            cannonRotateSpeed * Time.deltaTime * 0.5f);
    }

    void MoveCannonLeft(Vector2 joyDir)
    {
        Vector2 leftLimit = Quaternion.AngleAxis(cannonRotateLimit, Vector3.forward) * cannonLeftStartDir;
        Vector2 rightLimit = Quaternion.AngleAxis(-cannonRotateLimit, Vector3.forward) * cannonLeftStartDir;
        float angle = Vector2.SignedAngle(cannonLeftStartDir, joyDir);
        if (angle >= cannonRotateLimit)
        {
            joyDir = leftLimit;
        }
        else if (angle <= -cannonRotateLimit)
        {
            joyDir = rightLimit;
        }

        Quaternion rot = Quaternion.FromToRotation(cannonLeft.up, joyDir);
        cannonLeft.rotation = Quaternion.Slerp(cannonLeft.rotation, rot * cannonLeft.rotation,
                                            cannonRotateSpeed * Time.deltaTime * 0.5f);
    }

    void MoveCannonRight(Vector2 joyDir)
    {
        Vector2 leftLimit = Quaternion.AngleAxis(cannonRotateLimit, Vector3.forward) * cannonRightStartDir;
        Vector2 rightLimit = Quaternion.AngleAxis(-cannonRotateLimit, Vector3.forward) * cannonRightStartDir;
        float angle = Vector2.SignedAngle(cannonRightStartDir, joyDir);
        if (angle >= cannonRotateLimit)
        {
            joyDir = leftLimit;
        }
        else if (angle <= -cannonRotateLimit)
        {
            joyDir = rightLimit;
        }

        Quaternion rot = Quaternion.FromToRotation(cannonRight.up, joyDir);
        cannonRight.rotation = Quaternion.Slerp(cannonRight.rotation, rot * cannonRight.rotation,
                                            cannonRotateSpeed * Time.deltaTime * 0.5f);
    }


    public void PrimaryWeapon(WeaponState ws, int cannonNum)
    {

        if (ws == WeaponState.First)
        {
            calCannonShootColdTime = 0f;
        }
        if (ws == WeaponState.Busy)
        {
            float cd = cannonShootColdTime;
            switch (PlayerPrefs.GetInt(CannonLevel))
            {
                case 1:
                    cd = cannonShootColdTime;
                    break;
                case 2:
                    cd = cannonShootColdTime * 2 / 3;
                    break;
                case 3:
                    cd = cannonShootColdTime / 2;
                    break;
                case 4:
                    cd = cannonShootColdTime * 2;
                    break;
                default:
                    break;
            }
            calCannonShootColdTime += Time.deltaTime;
            if (calCannonShootColdTime > cd)
            {
                GameObject go = Instantiate(bullet, cannonShootPos[cannonNum].position, cannonShootPos[cannonNum].rotation);
                switch (PlayerPrefs.GetInt(CannonLevel))
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        go.GetComponent<PrimaryBullet>().speed *= 2;
                        go.GetComponent<PrimaryBullet>().damage *= 6;
                        break;
                    default:
                        break;
                }
                calCannonShootColdTime = 0f;
            }
        }
        //Debug.Log(cannonShootColdTime++);
    }



    //TODO:完善特殊武器升级的逻辑
    //TODO:完善一些UI提示，武器弹药显示etc

    public void SpecWeapon(WeaponState ws)
    {
        //canChangeSpecWeaponLevel = false;
        if (ws == WeaponState.First)
        {
            calSpecShootColdTime = 0;
        }
        if (ws == WeaponState.Idle)
        {
            if (PlayerPrefs.GetInt(SpecCannonLevel) == 3 && laser != null)
            {
                laser.SetActive(false);
            }
        }
        if (ws == WeaponState.Busy)
        {//前两级应用子弹计数模式
            if (specWeaponShootCount >= specWeaponAmmoCount)
            {
                //一些弹药耗尽的提示和UI显示
                if (!specWeaponAmmoColdDown)
                {
                    StartCoroutine(SpecWeaponColdDown());
                    return;
                }
                return;
            }
            if (PlayerPrefs.GetInt(SpecCannonLevel) <= 2)
            {
                calSpecShootColdTime += Time.deltaTime;
                if (calSpecShootColdTime > specShootColdTime)
                {
                    switch (PlayerPrefs.GetInt(SpecCannonLevel))
                    {
                        case 2:
                            GameObject missle = Instantiate(this.missle, specShootPos.position, specShootPos.rotation);
                            ++specWeaponShootCount;
                            break;
                        case 1:
                            GameObject thunder = Instantiate(this.thunder, specShootPos.position, specShootPos.rotation);  //这里需要闪电预制体、多功能炮台的位置
                            ++specWeaponShootCount;
                            //print("发射闪电");
                            break;
                        /* case 3:
                            specShootColdTime = 1f;//第三等级为无限长光柱，冷却机制改为计时，然后冷却，因此相当于每秒跳一次计数
                            ++specWeaponShootCount;
                            break; */
                        default:
                            break;
                    }
                    calSpecShootColdTime = 0;
                }
            }
            else
            {
                if (laser == null)
                {
                    laser = Instantiate(this.laserBlade, specShootPos.position, specShootPos.rotation);  //这里需要闪电预制体、多功能炮台的位置

                }
                laser.SetActive(true);
                laser.transform.position = specShootPos.position;
                laser.transform.rotation = specShootPos.rotation;

                specWeaponShootCount += Time.deltaTime;
                //specWeaponShootCount = (int)specWeaponShootCount;
            }
            //specWeaponAmmoText.text = (specWeaponAmmoCount - specWeaponShootCount) + "/" + specWeaponAmmoCount;
            SpecWeaponTextUpdate(specWeaponAmmoText, specWeaponAmmoColdDown);
        }

    }

    public IEnumerator SpecWeaponColdDown()
    {
        if (laser != null)
        {
            //如果laser物体，即第三等级特殊武器发挥作用，则冷却时销毁该物体
            Destroy(laser);
        }
        specWeaponAmmoColdDown = true;
        //specWeaponAmmoText.color = Color.red;
        SpecWeaponTextUpdate(specWeaponAmmoText, specWeaponAmmoColdDown);
        calSpecAmmoRunOutColdTime = specAmmoRunOutColdTime;
        while (calSpecAmmoRunOutColdTime > 0)
        {
            calSpecAmmoRunOutColdTime -= Time.deltaTime;
            yield return null;
        }
        specWeaponShootCount = 0;
        /* specWeaponAmmoText.text = (specWeaponAmmoCount - specWeaponShootCount) + "/" + specWeaponAmmoCount;
        specWeaponAmmoText.color = Color.yellow; */
        specWeaponAmmoColdDown = false;
        SpecWeaponTextUpdate(specWeaponAmmoText, specWeaponAmmoColdDown);
    }
    private void SpecWeaponTextUpdate(Text text, bool coldDown)
    {
        int cal = (int)specWeaponShootCount;
        specWeaponAmmoText.text = (specWeaponAmmoCount - cal) + "/" + specWeaponAmmoCount;
        if (coldDown)
        {
            specWeaponAmmoText.color = Color.red;
        }
        else
        {
            specWeaponAmmoText.color = Color.yellow;
        }
    }
    private IEnumerator HookOut()
    {
        hookState = WeaponState.Busy;
        SpriteRenderer sr = hookHead.GetComponent<SpriteRenderer>();
        Collider2D co = hookHead.GetComponent<Collider2D>();
        ContactFilter2D cf = new ContactFilter2D();
        Collider2D[] result = new Collider2D[1];
        /* Rect rect = sr.sprite.rect;
        Debug.Log(sr.sprite.border);
        Debug.Log(sr.sprite.bounds);
        //Debug.Log(rect.position);
        Debug.Log(hookHead.transform.position); */

        float calTime = 0f;
        cf.layerMask = LayerMask.GetMask("Gem");
        cf.useLayerMask = true;
        while (calTime < hookShootMaxTime)
        {
            calTime += Time.deltaTime;
            co.OverlapCollider(cf, result);
            if (result[0] != null)
            {
                Debug.Log(result[0].name);
                result[0].transform.position = co.bounds.center;
                yield return new WaitForSeconds(0.2f);
                yield return StartCoroutine(HookBack(result[0]));
                ReceiveHookBack(result[0]);
                yield break;
            }

            sr.size += new Vector2(0f, Time.deltaTime * hookShootSpeed * 5f);
            /* Debug.Log(sr.size);
            Debug.Log(Time.deltaTime);  */
            yield return new WaitWhile(() => Time.timeScale == 0);//此处方便在游戏暂停时暂停协程
        }
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(HookBack(null));
    }

    private IEnumerator HookBack(Collider2D target)
    {
        SpriteRenderer sr = hookHead.GetComponent<SpriteRenderer>();
        Collider2D co = hookHead.GetComponent<Collider2D>();
        while (sr.size.y > hookDefaultSize.y)
        {
            sr.size -= new Vector2(0f, Time.deltaTime * hookShootSpeed * 5f);
            if (target != null)
            {
                target.transform.position = co.bounds.center;
            }
            /* Debug.Log(Time.deltaTime);
            Debug.Log(sr.size); */
            yield return new WaitWhile(() => Time.timeScale == 0);
        }
        hookState = WeaponState.Idle;
    }
    public void ShootHook()
    {
        if (hookState == WeaponState.Idle)
        {
            StartCoroutine(HookOut());
        }
    }

    private void ReceiveHookBack(Collider2D co)
    {
        //TODO:完善抓取后的分析函数
        if (co == null)
        {
            return;
        }
        /* if (co.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            return;
        } */
        else if (co.gameObject.layer == LayerMask.NameToLayer("Gem"))
        {
            ShipUpdate(co.tag);
        }
        Destroy(co.gameObject);
    }

    /* private float PanelAngle(float value)
        {
            if (value == 0)
            {
                return 0f;
            }
            if (value > 180f)
            {
                return value - 360f;
            }
            return value;
        }*/
    private string RandomQTEString()
    {
        string result = null, part = null;
        for (int i = 0; i < 4; i++)
        {
            int ran = UnityEngine.Random.Range(1, 4);
            part = crossKey[ran];
            result = string.Concat(result, part);
        }
        return result;
    }
    public void GenerateLabQTE()
    {
        if (shieldActive && shieldRechargeable && !labQTEStart)
        {//shieldRechargeable 表示是否抓取到电池gem
            labQTEStart = true;//由于此函数会被player update执行，所以判断qte是否正在进行，防止重复
            labQTEString = RandomQTEString();
            /* Dictionary<string, bool> check = new Dictionary<string, bool>();
            foreach (char c in labQTEString)
            {
                check.Add(c.ToString(), false);
            } */
            labQTEText.text = labQTEString;
            labQTEText.color = Color.white;
            /* if (!labQTEText.isActiveAndEnabled)
            {
                labQTEText.enabled = true;
            } */
            labQTEText.enabled = true;
            /* if (true)
            {
                Debug.Log(labQTEString);
                Debug.Log(labQTEText.enabled);
            } */
            //return true;
        }
        //return false;
    }
    private IEnumerator RegenerateQTE()
    {
        QTEProgress = 0;
        labQTEStart = false;
        labQTEText.color = Color.red;
        yield return new WaitForSeconds(1);
        GenerateLabQTE();
    }
    public void ReceiveQTEInput(int key)
    {
        if (shieldRechargeable && labQTEStart)
        {
            if (crossKey[key] == labQTEString.Substring(QTEProgress, 1))
            {
                //Debug.Log(QTEProgress);
                if (QTEProgress == labQTEString.Length - 1)
                {
                    QTEProgress = 0;
                    labQTEText.color = Color.green;

                    PolygonCollider2D pc = shield.GetComponent<PolygonCollider2D>();
                    if (pc.isActiveAndEnabled)
                    {//如果护盾正在起作用，就填满充能条
                        calShieldLastTime = shieldLastTime;
                    }
                    else
                    {//如果护盾已经耗尽，就重新启用
                        StartCoroutine(UsingShield(shieldLastTime, shieldBarFill));
                    }
                    shieldRechargeable = false;
                    /* if (labQTEText.isActiveAndEnabled)
                    {
                        labQTEStart = false;
                        labQTEText.enabled = false;
                    } */
                }
                else
                {
                    ++QTEProgress;
                }
            }
            else
            {
                StartCoroutine(RegenerateQTE());
            }
        }
    }
    public void QuitLab()
    {
        QTEProgress = 0;
        labQTEStart = false;
        labQTEText.enabled = false;
    }
    //升级功能与一个存储了等级信息的字典有关，所以无论何种方法升级，都要考虑传入string来区分选择升级的武器
    private void ShipUpdate(string toUpdate)
    {
        switch (toUpdate)
        {
            //传入的string考虑用tag，和控制台一致的tag
            case "CannonUp":
            case "CannonRight":
            case "CannonLeft":
                int cannonLevel = PlayerPrefs.GetInt(CannonLevel);
                int nextCannonLevel = ++cannonLevel;
                Mathf.Clamp(nextCannonLevel, 1, 4);
                PlayerPrefs.SetInt(CannonLevel, nextCannonLevel);
                break;
            case "Missle":
                int specLevel = PlayerPrefs.GetInt(SpecCannonLevel);
                int nextSpecLevel = ++specLevel;
                Mathf.Clamp(nextSpecLevel, 1, 4);
                PlayerPrefs.SetInt(SpecCannonLevel, nextSpecLevel);
                if (specWeaponAmmoColdDown)
                {
                    calSpecAmmoRunOutColdTime = 0;//升级时立刻完成冷却
                }
                else
                {
                    specWeaponShootCount = 0;
                }
                //specWeaponAmmoText.text = (specWeaponAmmoCount - specWeaponShootCount) + "/" + specWeaponAmmoCount;
                SpecWeaponTextUpdate(specWeaponAmmoText, specWeaponAmmoColdDown);
                break;
            case "Shield":
                if (shieldActive)
                {
                    if (!shieldRechargeable)
                    {
                        shieldRechargeable = true;
                    }
                }
                break;
            default:
                break;
        }
    }
    //IEnumerator LevelUp()
}
