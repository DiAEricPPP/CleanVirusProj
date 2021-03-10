using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;



public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigidBody;
    [SerializeField] private Collider2D playerCollider;
    public float normalSpeed = 1.2f;
    public float climbSpeed = 1.2f;
    public GameObject allShip;
    public GameObject terminal;
    public Collider2D ladderFloor;
    public Collider2D shipFloor;
    float walkSpeed = 1.2f;

    public string horizontalAxis, verticalAxis;
    public string crossX, crossY;
    //private bool DPadUpDown = false, DPadDownDown = false, DPadLeftDown = false, DPadRightDown = false;
    private enum DPad { Up, Down, Left, Right, NULL };
    private DPad DPadPressed;
    public KeyCode JoystickA, JoystickB, JoystickX;


    //玩家靠近控制台，先弹出互动提示，之后才可以互动
    //private bool showMenu = false;//是否弹出互动按钮并且可以互动
    private bool canUseTerminal = false;//判断能否使用控制台
    private bool isOnTerminal = false;//是否在使用控制台
    private bool onPusher = false;//判断是否在推进器附近
    private bool canClimb = false;
    private bool onMap = false;
    private bool onLab = false;
    private bool onMissle = false;
    private bool onShield = false;
    private bool onHook = false;
    private bool onCannonUp = false;
    private bool onCannonRight = false;
    private bool onCannonLeft = false;

    private Collider2D busyCollider;//用来存储当前玩家与控制台的交互信息
    //private bool busyColliderWork;
    private ShipController shipController;
    /* private bool freeze = false;
    private Vector2 freezeLocalPos; */
    public AudioClip stepOnFloor;
    public AudioClip stepOnLadder;
    private AudioSource stepAudio;
    [Range(0.1f, 1f)]
    public float playStepOnFloorGap;
    [Range(0.1f, 1f)]
    public float playStepOnLadderGap;
    private float calPlayStepOnFloorGap, calPlayStepOnLadderGap;


    //enum Terminal { Pusher, Map, Missle, Hook, Lab, Shield, CannonLeft, CannonUp, CannonRight };

    // Start is called before the first frame update
    void Start()
    {
        stepAudio = GetComponent<AudioSource>();
        calPlayStepOnFloorGap = 0;
        calPlayStepOnLadderGap = 0;
        walkSpeed = normalSpeed;
        playerRigidBody = this.GetComponent<Rigidbody2D>();
        playerCollider = this.GetComponent<Collider2D>();
        shipController = allShip.GetComponent<ShipController>();
    }

    // Update is called once per frame
    void Update()
    {
        /* if (freeze)
        {
            transform.localPosition = freezeLocalPos;
            //playerRigidBody.MovePosition(transform.parent.TransformPoint(freezeLocalPos));
        } */
        UseTerminal();
        WhichTerminal();

        //Debug.Log(playerCollider.IsTouching(shipFloor));

        //freezeLocalPos = transform.localPosition;
    }

    void FixedUpdate()
    {
        ToMove();
        UseLadder();
        //Debug.Log(playerRigidBody.gravityScale);
        /* UseTerminal();
        WhichTerminal(); */
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Ladder")
        {
            canUseTerminal = true;
            busyCollider = other;
        }
        switch (other.tag)
        {
            case "PushTerminal":
                onPusher = true;
                break;
            case "Map":
                onMap = true;
                break;
            case "Missle":
                onMissle = true;
                break;
            case "Hook":
                onHook = true;
                break;
            case "Lab":
                onLab = true;
                break;
            case "Shield":
                onShield = true;
                break;
            case "CannonLeft":
                onCannonLeft = true;
                break;
            case "CannonUp":
                onCannonUp = true;
                break;
            case "CannonRight":
                onCannonRight = true;
                break;
            case "Ladder":
                canClimb = true;
                //Debug.Log(canClimb);
                break;
            default:
                break;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //if (other.tag != "Ladder") UseTerminal();
    }
    void OnTriggerExit2D(Collider2D other)
    {
        /* if (other.transform.tag == "Terminal")
        {
            showMenu = false;
            this.ShowMenu();
        } */
        canUseTerminal = false;
        busyCollider = null;

        switch (other.tag)
        {
            case "PushTerminal":
                onPusher = false;
                break;
            case "Map":
                onMap = false;
                break;
            case "Missle":
                onMissle = false;
                break;
            case "Hook":
                onHook = false;
                break;
            case "Lab":
                onLab = false;
                break;
            case "Shield":
                onShield = false;
                break;
            case "CannonLeft":
                onCannonLeft = false;
                break;
            case "CannonUp":
                onCannonUp = false;
                break;
            case "CannonRight":
                onCannonRight = false;
                break;
            case "Ladder":
                canClimb = false;
                //Debug.Log(canClimb);
                break;
            default:
                break;
        }

    }

    /* private IEnumerator PlayStepSound(AudioClip ac, float timeGap, bool audioFlag)
    {
        if (audioFlag)
        {
            stepAudio.PlayOneShot(ac);
            //Debug.Log("Play" + ac.name);
            yield return new WaitForSeconds(timeGap);
            StartCoroutine(PlayStepSound(ac, timeGap, audioFlag));
        }
    } */
    void ToMove()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Player"));//忽略玩家层碰撞
        /* if (freeze)
        {
            //transform.localPosition = freezeLocalPos;
            playerRigidBody.MovePosition(transform.parent.TransformPoint(freezeLocalPos));
            return;
        } */
        float horizontal = Input.GetAxis(horizontalAxis);
        //float vertical = Input.GetAxis(verticalAxis);
        if (horizontal != 0f)
        {
            //StartCoroutine(PlayStepSound(stepOnFloor, 0.5f, horizontal != 0 && playerCollider.IsTouching(shipFloor)));
            if (playerCollider.IsTouching(shipFloor))
            {
                calPlayStepOnFloorGap += Time.deltaTime;
                if (calPlayStepOnFloorGap >= playStepOnFloorGap)
                {
                    stepAudio.PlayOneShot(stepOnFloor);
                    calPlayStepOnFloorGap = 0;
                }
            }
            else
            {
                calPlayStepOnFloorGap = 0;
            }
            transform.Translate(Vector3.right * horizontal * walkSpeed * Time.deltaTime);// A D
            //playerRigidBody.MovePosition(playerRigidBody.position + Vector2.right * horizontal * walkSpeed * Time.deltaTime);
        }
        //freezeLocalPos = transform.localPosition;
    }


    /* void ShowMenu()
    {
        if (showMenu == true)
        {
            Debug.Log("Menu On");
        }
        if (showMenu == false)
        {
            Debug.Log("Menu Off");
        }
    } */
    void UseTerminal()
    {
        if (canUseTerminal && Time.timeScale == 1)
        {
            if (Input.GetKeyDown(JoystickX))
            {//进入控制台
             //showMenu = false;
                terminal.SendMessage("TryUseTerminal", busyCollider.name);
                if (terminal.GetComponent<TerminalCheck>().CheckTerminal(busyCollider.name))
                {
                    walkSpeed = 0f;
                    //playerRigidBody.bodyType = RigidbodyType2D.Static;
                    //freeze = true;
                    isOnTerminal = true;
                }//当多人发生冲突时不执行进入控制台操作，并且重置控制台状态
                else
                {
                    terminal.SendMessage("LeaveTerminal", busyCollider.name);
                }
            }
            if (Input.GetKeyDown(JoystickB) && isOnTerminal == true)
            {//离开控制台
                terminal.SendMessage("LeaveTerminal", busyCollider.name);
                if (onLab)
                {
                    shipController.QuitLab();
                }
                isOnTerminal = false;
                walkSpeed = normalSpeed;
                //freeze = false;
                //playerRigidBody.bodyType = RigidbodyType2D.Dynamic;
                //showMenu = true;
            }
        }
    }


    //TODO:将sendmessage方法换用事件方法
    void UsePusher()
    {
        if (onPusher && isOnTerminal)
        {
            //获得摇杆方位
            float horizontal = Input.GetAxis(horizontalAxis);
            float vertical = Input.GetAxis(verticalAxis);
            Vector2 joyDir = new Vector2(horizontal, vertical);

            shipController.MovePusher(joyDir);


            if (Input.GetKeyDown(JoystickA))
            {
                shipController.Push(ShipController.WeaponState.First);
            }
            if (Input.GetKey(JoystickA))
            {
                shipController.Push(ShipController.WeaponState.Busy);
            }
        }
    }

    void UseLadder()
    {
        if (canClimb)
        {
            float vertical = Input.GetAxis(verticalAxis);
            //float horizontal = Input.GetAxis(horizontalAxis);
            playerRigidBody.gravityScale = 0;
            if (vertical != 0)
            {
                //StartCoroutine(PlayStepSound(stepOnLadder, 0.5f, vertical != 0));
                calPlayStepOnLadderGap += Time.deltaTime;
                if (calPlayStepOnLadderGap >= playStepOnLadderGap)
                {
                    stepAudio.PlayOneShot(stepOnLadder);
                    calPlayStepOnLadderGap = 0;
                }
                transform.Translate(Vector3.up * vertical * climbSpeed * Time.deltaTime);
                //playerRigidBody.MovePosition(playerRigidBody.position + Vector2.up * vertical * climbSpeed * Time.deltaTime);
                Physics2D.IgnoreCollision(playerCollider, ladderFloor);
            }

        }
        else
        {
            calPlayStepOnLadderGap = 0;
            playerRigidBody.gravityScale = 1;
            Physics2D.IgnoreCollision(playerCollider, ladderFloor, false);
        }
    }


    //TODO:完善其余控制台方法
    bool GetDPadButtonDown(DPad button)
    {//在Update调用，同官方方法
        switch (button)
        {
            case DPad.Up:
                if (DPadPressed != DPad.Up && Input.GetAxisRaw(crossY) == 1)
                {
                    DPadPressed = DPad.Up;
                    return true;
                }
                break;
            case DPad.Down:
                if (DPadPressed != DPad.Down && Input.GetAxisRaw(crossY) == -1)
                {
                    DPadPressed = DPad.Down;
                    return true;
                }
                break;
            case DPad.Left:
                if (DPadPressed != DPad.Left && Input.GetAxisRaw(crossX) == -1)
                {
                    DPadPressed = DPad.Left;
                    return true;
                }
                break;
            case DPad.Right:
                if (DPadPressed != DPad.Right && Input.GetAxisRaw(crossX) == 1)
                {
                    DPadPressed = DPad.Right;
                    return true;
                }
                break;
        }
        if (Input.GetAxisRaw(crossX) == 0 && Input.GetAxisRaw(crossY) == 0)
        {
            DPadPressed = DPad.NULL;
        }
        return false;
    }
    void UseLab()
    {
        if (onLab && isOnTerminal)
        {//1234上下左右
            shipController.GenerateLabQTE();
            if (GetDPadButtonDown(DPad.Up))
            {
                shipController.ReceiveQTEInput(1);
            }
            if (GetDPadButtonDown(DPad.Down))
            {
                shipController.ReceiveQTEInput(2);
            }
            if (GetDPadButtonDown(DPad.Left))
            {
                shipController.ReceiveQTEInput(3);
            }
            if (GetDPadButtonDown(DPad.Right))
            {
                shipController.ReceiveQTEInput(4);
            }
            if (Input.GetKeyDown(JoystickB))
            {
                shipController.QuitLab();
            }
        }
        //Debug.Log("Using Lab");
    }
    void UseMap()
    {
        if (!onMap || !isOnTerminal) return;
        //Debug.Log("Using Map");
    }
    void UseMissle()
    {
        if (onMissle && isOnTerminal && Time.timeScale == 1)
        {
            float horizontal = Input.GetAxis(horizontalAxis);
            float vertical = Input.GetAxis(verticalAxis);
            Vector2 joyDir = new Vector2(horizontal, vertical);
            allShip.SendMessage("MoveSpecCannon", joyDir);

            if (Input.GetKeyDown(JoystickA))
            {
                shipController.SpecWeapon(ShipController.WeaponState.Idle);
            }
            if (Input.GetKey(JoystickA))
            {
                shipController.SpecWeapon(ShipController.WeaponState.Busy);
            }
            if (Input.GetKeyUp(JoystickA))
            {
                shipController.SpecWeapon(ShipController.WeaponState.Idle);
            }
        }
    }
    void UseShield()
    {
        if (onShield && isOnTerminal)
        {
            shipController.ActivateShield();

            float horizontal = Input.GetAxis(horizontalAxis);
            float vertical = Input.GetAxis(verticalAxis);
            Vector2 joyDir = new Vector2(horizontal, vertical);
            //allShip.SendMessage("MoveShield", joyDir);
            shipController.MoveShield(joyDir);
        }
    }
    void UseHook()
    {
        if (onHook && isOnTerminal && Time.timeScale == 1)
        {
            float horizontal = Input.GetAxis(horizontalAxis);
            float vertical = Input.GetAxis(verticalAxis);
            Vector2 joyDir = new Vector2(horizontal, vertical);
            allShip.SendMessage("MoveHook", joyDir);

            if (Input.GetKeyDown(JoystickA))
            {
                shipController.ShootHook();
            }
        }
    }
    void UseCannonUp()
    {
        if (onCannonUp && isOnTerminal && Time.timeScale == 1)
        {
            float horizontal = Input.GetAxis(horizontalAxis);
            float vertical = Input.GetAxis(verticalAxis);
            Vector2 joyDir = new Vector2(horizontal, vertical);
            allShip.SendMessage("MoveCannonUp", joyDir);

            if (Input.GetKeyDown(JoystickA))
            {
                shipController.PrimaryWeapon(ShipController.WeaponState.First, 0);
            }
            if (Input.GetKey(JoystickA))
            {
                shipController.PrimaryWeapon(ShipController.WeaponState.Busy, 0);
            }
        }
    }
    void UseCannonLeft()
    {
        if (onCannonLeft && isOnTerminal && Time.timeScale == 1)
        {
            float horizontal = Input.GetAxis(horizontalAxis);
            float vertical = Input.GetAxis(verticalAxis);
            Vector2 joyDir = new Vector2(horizontal, vertical);
            allShip.SendMessage("MoveCannonLeft", joyDir);

            if (Input.GetKeyDown(JoystickA))
            {
                shipController.PrimaryWeapon(ShipController.WeaponState.First, 1);
            }
            if (Input.GetKey(JoystickA))
            {
                shipController.PrimaryWeapon(ShipController.WeaponState.Busy, 1);
            }
        }

    }
    void UseCannonRight()
    {
        if (onCannonRight && isOnTerminal && Time.timeScale == 1)
        {
            float horizontal = Input.GetAxis(horizontalAxis);
            float vertical = Input.GetAxis(verticalAxis);
            Vector2 joyDir = new Vector2(horizontal, vertical);
            allShip.SendMessage("MoveCannonRight", joyDir);

            if (Input.GetKeyDown(JoystickA))
            {
                shipController.PrimaryWeapon(ShipController.WeaponState.First, 2);
            }
            if (Input.GetKey(JoystickA))
            {
                shipController.PrimaryWeapon(ShipController.WeaponState.Busy, 2);
            }
        }

    }
    void WhichTerminal()
    {
        UsePusher();
        UseCannonLeft();
        UseCannonRight();
        UseCannonUp();
        UseHook();
        UseLab();
        UseMap();
        UseShield();
        UseMissle();
    }

    /* void SetTerminalState(bool canWork)
    {
        busyColliderWork = canWork;
    } */

    /* void SetControllerMap(string controller)
    {

    } */
}
